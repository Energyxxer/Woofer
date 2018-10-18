using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;
using GameInterfaces.Input;
using WooferGame.Common;
using WooferGame.Input;
using WooferGame.Meta.LevelEditor.Systems.ComponentView;
using WooferGame.Meta.LevelEditor.Systems.EntityView;
using WooferGame.Meta.LevelEditor.Systems.InputModes;
using WooferGame.Systems.Visual;
using Point = System.Drawing.Point;
using Rectangle = EntityComponentSystem.Util.Rectangle;

namespace WooferGame.Meta.LevelEditor.Systems
{
    [ComponentSystem("entity_view", ProcessingCycles.Input | ProcessingCycles.Render, ProcessingFlags.Pause),
        Listening(typeof(EntitySelectEvent))]
    class EntityViewSystem : ComponentSystem
    {
        private bool ModalActive = false;
        private bool ModalVisible = false;
        private long Selected;

        private bool ComponentLocked = false;

        private int SelectedComponentIndex = 0;
        private int SelectedPropertyIndex = 0;

        private int ListFromIndex = 0;
        private List<int> ComponentRenderOffsets = new List<int>();

        private ComponentSummary SelectedComponent = null;

        private readonly EntityViewHelper Helper;

        private int RemoveTimer = 0;

        public override bool ShouldSave => false;

        public EntityViewSystem()
        {
            Helper = new EntityViewHelper(this);
        }

        public override void Input()
        {
            if (!ModalActive) return;
            if (!Owner.Entities.ContainsId(Selected)) return;
            Entity entity = Owner.Entities[Selected];
            if(entity == null)
            {
                Owner.Events.InvokeEvent(new RequestModalChangeEvent(null));
                return;
            }

            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            Vector2D movement = inputMap.Movement;

            if (movement.Magnitude > 1e-5 && Editor.MoveTimeframe.Execute())
            {
                if (movement.Y > 0)
                {
                    if(ComponentLocked)
                    {
                        if (SelectedPropertyIndex - 1 >= 0) SelectedPropertyIndex--;
                    } else
                    {
                        if (SelectedComponentIndex - 1 >= -2) SelectedComponentIndex--;
                    }
                }
                else if (movement.Y < 0)
                {
                    if(ComponentLocked)
                    {
                        if (SelectedPropertyIndex + 1 < SelectedComponent.Members.Count) SelectedPropertyIndex++;
                    } else
                    {
                        if (SelectedComponentIndex + 1 <= entity.Components.Count+1) SelectedComponentIndex++;
                    }
                }

                if (movement.Y != 0)
                {
                    RemoveTimer = 0;
                }
                /*if (SelectedComponentIndex < StartOffset)
                {
                    StartOffset = SelectedComponentIndex;
                }
                if (SelectedComponentIndex > StartOffset + AmountVisible)
                {
                    StartOffset = SelectedComponentIndex - AmountVisible;
                }*/
            }
            
            if (inputMap.Jump.Consume())
            {
                if(SelectedComponentIndex == -2)
                {
                    new PropertySummary(Owner, typeof(Entity).GetProperty("Name"), entity).TriggerEdit(inputMap.Interact.Pressed);
                    ModalActive = false;
                }
                else if (SelectedComponentIndex == -1)
                {
                    entity.Active = !entity.Active;
                }
                else if (SelectedComponentIndex == Helper.Components.Count)
                {
                    Owner.Events.InvokeEvent(new StartEnumSelectEvent("Available components", Component.GetAllIdentifiers().Where(s => !Helper.Components.ContainsKey(s)).ToList(), AddComponent, null));
                    Owner.Events.InvokeEvent(new ForceModalChangeEvent("enum_select", null));
                    ModalActive = false;
                    ModalVisible = false;
                }
                else if(SelectedComponentIndex == Helper.Components.Count + 1)
                {
                    Owner.Events.InvokeEvent(new StartTextInputEvent(entity.Name, n => TagIOUtils.SavePrefab(entity, n), null) { Label = "Prefab Name" });
                    Owner.Events.InvokeEvent(new ForceModalChangeEvent("text_input", null));
                    ModalActive = false;
                }
                else if (!ComponentLocked)
                {
                    SelectedComponent = Helper.Components.Values.ElementAt(SelectedComponentIndex);
                    SelectedPropertyIndex = 0;
                    ComponentLocked = true;
                }
                else
                {
                    if (SelectedPropertyIndex >= Helper.Components.Values.ElementAt(SelectedComponentIndex).Members.Count) return;
                    IMemberSummary member = Helper.Components.Values.ElementAt(SelectedComponentIndex).Members.Values.ElementAt(SelectedPropertyIndex);
                    if (member.CanSet)
                    {
                        bool modalNeedsChange = member.TriggerEdit(inputMap.Interact.Pressed);
                        if (modalNeedsChange)
                        {
                            ModalActive = false;
                        }

                    }
                }
            }

            if (RemoveTimer > 0) RemoveTimer--;
            if (inputMap.Pulse.Pressed && SelectedComponentIndex >= 0 && !ComponentLocked)
            {
                RemoveTimer += 2;
                if (RemoveTimer / 25 > 3)
                {
                    Owner.Entities[Selected].Components.Remove(Helper.Components.Values.ElementAt(SelectedComponentIndex).ComponentName);
                    RemoveTimer = 0;
                    Helper.Update(Selected);
                    if (SelectedComponentIndex >= Helper.Components.Count) SelectedComponentIndex = Helper.Components.Count - 1;
                }
            }
            else RemoveTimer = 0;

            if (SelectedComponentIndex >= 0 && SelectedComponentIndex < ComponentRenderOffsets.Count)
            {
                int y = ComponentRenderOffsets[SelectedComponentIndex];
                if (y < 0)
                {
                    ListFromIndex--;
                }
                else if (y > 720)
                {
                    ListFromIndex = Math.Max(0, SelectedComponentIndex - 2);
                }
            }
        }

        private void AddComponent(string identifier)
        {
            Entity entity = Owner.Entities[Selected];
            if (entity == null) return;
            
            Type type = Component.TypeForIdentifier(identifier);
            Component component = (Component)type.GetConstructor(new Type[0]).Invoke(new object[0]);
            entity.Components.Add(component);

            Helper.Update(Selected);
        }

        internal static readonly Sprite ObjectIcon = new Sprite("editor", new Rectangle(0, 0, 16, 16), new Rectangle(0, 16, 16, 16));
        internal static readonly Sprite CheckboxIcon = new Sprite("editor", new Rectangle(0, 0, 16, 16), new Rectangle(0, 48, 8, 8));

        private static void SetCheckboxChecked(bool value)
        {
            CheckboxIcon.Source.X = value ? 8 : 0;
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!ModalVisible) return;
            if (!Owner.Entities.ContainsId(Selected)) return;

            var layer = r.GetLayerGraphics("hi_res_overlay");

            Entity entity = Owner.Entities[Selected];

            int x = EditorRendering.SidebarX + 2*EditorRendering.SidebarMargin;
            int y = EditorRendering.SidebarMargin + 4;

            new TextUnit(ObjectIcon, entity.Name, SelectedComponentIndex == -2 ? Color.CornflowerBlue : Color.White).Render(r, layer, new Point(x, y), 2);
            y += 20;
            new TextUnit("ID: " + Selected, Color.DarkGray).Render(r, layer, new Point(x, y), 1);
            y += 22;

            if(SelectedComponentIndex == -1) layer.FillRect(new System.Drawing.Rectangle(x - 4, y - 2, EditorRendering.SidebarWidth - 2 * EditorRendering.SidebarMargin, 20), Color.CornflowerBlue);
            SetCheckboxChecked(entity.Active);
            new TextUnit(CheckboxIcon, "Active").Render(r, layer, new Point(x, y), 2);

            y += 20;

            layer.FillRect(new System.Drawing.Rectangle(x-2*EditorRendering.SidebarMargin, y, EditorRendering.SidebarWidth - 2 * EditorRendering.SidebarMargin, 3), Color.FromArgb(45, 45, 48));
            y += 8;

            ComponentRenderOffsets.Clear();

            int startY = 0;

            int index = 0;
            foreach(ComponentSummary component in Helper.Components.Values)
            {
                ComponentRenderOffsets.Add(y);

                bool doRender = index >= ListFromIndex;
                if (index == ListFromIndex)
                {
                    startY = y;
                    y = 2*EditorRendering.SidebarMargin + 20 + 22 + 20 + 8;
                }

                if(index == SelectedComponentIndex && doRender)
                {
                    layer.FillRect(new System.Drawing.Rectangle(x - 4, y - 2, EditorRendering.SidebarWidth - 2 * EditorRendering.SidebarMargin, 20), ComponentLocked ? Color.FromArgb(63, 63, 70) : Color.CornflowerBlue);
                }
                if(doRender) new TextUnit(
                    //new Sprite("editor", new Rectangle(0, 0, 16, 16), new Rectangle(0, 32, 16, 16)), 
                    component.ComponentName)
                    .Render(r, layer, new Point(x, y), 2);
                y += 24;

                int memberIndex = 0;
                foreach(IMemberSummary member in component.Members.Values)
                {
                    if (doRender)
                    {
                        TextUnit label = member.Label;
                        label.Color = ComponentLocked && SelectedComponentIndex == index ? (memberIndex == SelectedPropertyIndex ? Color.CornflowerBlue : Color.White) : Color.Gray;
                        label.Render(r, layer, new Point(x + 8, y), 1);
                    }
                    y += 16;
                    memberIndex++;
                }

                y += 16;

                index++;
            }

            {
                ComponentRenderOffsets.Add(y);
                GUIButton button = new GUIButton(new Vector2D(x, y), "Add Component", new Rectangle(0, 0, EditorRendering.SidebarWidth - 4 * EditorRendering.SidebarMargin, 24)) { TextSize = 2 };
                button.Highlighted = SelectedComponentIndex == Helper.Components.Count;
                button.Render(r, layer, Vector2D.Empty);

                for (int i = 0; i < ListFromIndex; i++)
                {
                    ComponentRenderOffsets[i] -= startY;
                }
                index++;
                y += 32;
            }
            {
                ComponentRenderOffsets.Add(y);
                GUIButton button = new GUIButton(new Vector2D(x, y), "Save Prefab", new Rectangle(0, 0, EditorRendering.SidebarWidth - 4 * EditorRendering.SidebarMargin, 24)) { TextSize = 2 };
                button.Highlighted = SelectedComponentIndex == Helper.Components.Count+1;
                button.Render(r, layer, Vector2D.Empty);

                for (int i = 0; i < ListFromIndex; i++)
                {
                    ComponentRenderOffsets[i] -= startY;
                }
                index++;
                y += 32;
            }

            if (RemoveTimer > 0 && SelectedComponentIndex >= 0 && SelectedComponentIndex < Helper.Components.Count)
            {
                TextUnit removingLabel = new TextUnit("Removing " + Helper.Components.ElementAt(SelectedComponentIndex).Key + new string('.', RemoveTimer / 25));
                System.Drawing.Size labelSize = removingLabel.GetSize(3);
                removingLabel.Render(r, layer, new Point(EditorRendering.SidebarX - labelSize.Width, layer.GetSize().Height - labelSize.Height), 3);
            }
        }

        public override void EventFired(object sender, Event e)
        {
            if(e is EntitySelectEvent s)
            {
                Selected = s.Entity.Id;
                SelectedComponentIndex = -1;
                SelectedPropertyIndex = 0;
                ListFromIndex = 0;

                Helper.Update(Selected);

                ComponentLocked = false;
            } else if(e is ModalChangeEvent)
            {
                ModalActive = true;
                ModalVisible = true;
            } else if(e is BeginModalChangeEvent bmce)
            {
                if (ComponentLocked) ComponentLocked = false;
                else
                {
                    bmce.SystemName = "entity_list";
                    ModalActive = false;
                    ModalVisible = false;
                }
            }
        }
    }
}
