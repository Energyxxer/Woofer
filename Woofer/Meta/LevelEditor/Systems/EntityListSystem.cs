using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
using WooferGame.Meta.LevelEditor.Systems.EntityOutlines;
using WooferGame.Meta.LevelEditor.Systems.InputModes;
using WooferGame.Systems;

namespace WooferGame.Meta.LevelEditor.Systems
{
    [ComponentSystem("entity_list", ProcessingCycles.Input | ProcessingCycles.Render, ProcessingFlags.Pause)]
    class EntityListSystem : ComponentSystem
    {
        private bool ModalActive = false;
        private bool ModalVisible = false;
        private int SelectedIndex = 0;
        private int StartOffset = -1;

        private int AmountVisible = 16;

        public override bool ShouldSave => false;
        
        private EntityOutline Outline;

        private int RemoveTimer = 0;
        private int RemoveSpeed = 0;

        private const string BLANK_ENTITY = "Blank Entity";
        private const string FROM_PREFABS = "From Prefabs";

        public override void Input()
        {
            if (!ModalActive) return;
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            if (Outline == null)
            {
                Outline = new EntityOutline(Owner, 0);
                Owner.Events.InvokeEvent(new BeginOverlay(Outline));
            }

            Vector2D movement = inputMap.Movement;

            if(movement.Magnitude > 1e-5 && Editor.MoveTimeframe.Execute())
            {
                if (movement.Y > 0)
                {
                    if (SelectedIndex - 1 >= -1) SelectedIndex--;
                }
                else if (movement.Y < 0)
                {
                    if (SelectedIndex + 1 < Owner.Entities.Count) SelectedIndex++;
                }
                if(movement.Y != 0)
                {
                    RemoveTimer = 0;
                }
            }

            if (inputMap.Jump.Consume())
            {
                if (SelectedIndex == -1)
                {
                    Owner.Events.InvokeEvent(new StartEnumSelectEvent("Add Entity", new List<string>() { BLANK_ENTITY, FROM_PREFABS }, CreateEntity, null));
                    Owner.Events.InvokeEvent(new ForceModalChangeEvent("enum_select", null));
                    ModalActive = false;
                    ModalVisible = false;
                }
                else
                {
                    Owner.Events.InvokeEvent(new EntitySelectEvent(Owner.Entities.ToList()[SelectedIndex], null));
                    Owner.Events.InvokeEvent(new ForceModalChangeEvent("entity_view", null));
                    ModalActive = false;
                    ModalVisible = false;
                }
            }

            if(RemoveTimer > 0) RemoveTimer--;
            if (inputMap.Pulse.Pressed && SelectedIndex >= 0)
            {
                RemoveTimer += 2 + RemoveSpeed;
                if (RemoveTimer / 25 > 3)
                {
                    Owner.Entities.Remove(Owner.Entities.ElementAt(SelectedIndex).Id);
                    RemoveTimer = 0;
                    RemoveSpeed++;
                }
            }
            else
            {
                RemoveTimer = 0;
                RemoveSpeed = 0;
            }

            Outline.Id = 0;
            if (SelectedIndex >= 0 && SelectedIndex < Owner.Entities.Count)
            {
                Outline.Id = Owner.Entities.ToList()[SelectedIndex].Id;
            }
            if (SelectedIndex < StartOffset)
            {
                StartOffset = SelectedIndex;
            }
            if (SelectedIndex > StartOffset + AmountVisible)
            {
                StartOffset = SelectedIndex - AmountVisible;
            }
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!ModalVisible) return;
            SelectedIndex = Math.Max(-1, Math.Min(SelectedIndex, Owner.Entities.Count - 1));
            StartOffset = Math.Max(-1, Math.Min(StartOffset, Owner.Entities.Count - 1));
            var layer = r.GetLayerGraphics("hi_res_overlay");

            int y = 14;
            int x = EditorRendering.SidebarX + EditorRendering.SidebarMargin + 4;

            List<Entity> entities = Owner.Entities.ToList();
            int index = StartOffset;
            for (; index < entities.Count; index++)
            {
                if (index == -1)
                {
                    GUIButton button = new GUIButton(new Vector2D(x - 4 + EditorRendering.SidebarMargin, y), "Add Entity", new EntityComponentSystem.Util.Rectangle(0, 0, EditorRendering.SidebarWidth - 4 * EditorRendering.SidebarMargin, 24)) { TextSize = 2 };
                    button.Highlighted = SelectedIndex == -1;
                    button.Render(r, layer, Vector2D.Empty);

                    y += 24 + 8;
                }
                else
                {
                    Entity entity = entities[index];
                    if(index == SelectedIndex)
                    {
                        layer.FillRect(new System.Drawing.Rectangle(x - 4, y, EditorRendering.SidebarWidth - 2 * EditorRendering.SidebarMargin, 20), ModalActive ? Color.CornflowerBlue : Color.FromArgb(63, 63, 70));
                    }
                    TextUnit label = new TextUnit(entity.Name);
                    label.Render<TSurface, TSource>(r, layer, new Point(x, y + 2), 2);
                    y += 20;
                    if (y > 720 - 16) break;
                }
            }
            AmountVisible = index - StartOffset - 1;

            if(RemoveTimer > 0 && SelectedIndex >= 0)
            {
                TextUnit removingLabel = new TextUnit("Removing " + entities[SelectedIndex].Name + new string('.', RemoveTimer / 25));
                System.Drawing.Size labelSize = removingLabel.GetSize(3);
                removingLabel.Render(r, layer, new Point(EditorRendering.SidebarX - labelSize.Width, layer.GetSize().Height - labelSize.Height), 3);
            }
        }

        public override void EventFired(object sender, Event e)
        {
            if (e is BeginModalChangeEvent bmce)
            {
                bmce.SystemName = "editor_menu";
                ModalActive = false;
                ModalVisible = false;
            }
            else if (e is ModalChangeEvent)
            {
                ModalActive = true;
                ModalVisible = true;
            }
        }

        private void CreateEntity(string instruction)
        {
            if(instruction == BLANK_ENTITY)
            {
                Owner.Entities.Add(new Entity());
                Owner.Entities.Flush();
                SelectedIndex = Owner.Entities.Count - 1;
            } else if(instruction == FROM_PREFABS)
            {
                TagIOUtils.Refresh();
                Owner.Events.InvokeEvent(new StartEnumSelectEvent("Select Prefab", TagIOUtils.GetPrefabNames(), SpawnPrefab, null));
                Owner.Events.InvokeEvent(new ForceModalChangeEvent("enum_select", null));
                ModalActive = false;
                ModalVisible = false;
            }
        }

        private void SpawnPrefab(string name)
        {
            Entity entity = TagIOUtils.InstantiatePrefab(name);
            if(entity != null)
            {
                Owner.Entities.Add(entity);
                Owner.Entities.Flush();
                if(entity.Components.Get<Spatial>() is Spatial sp)
                {
                    sp.Position = Owner.CurrentViewport.Location;
                }
                SelectedIndex = Owner.Entities.Count - 1;
            }
        }
    }
}
