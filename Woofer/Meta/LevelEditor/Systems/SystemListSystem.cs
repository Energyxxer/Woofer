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
using WooferGame.Meta.LevelEditor.Systems.InputModes;

namespace WooferGame.Meta.LevelEditor.Systems
{
    [ComponentSystem("system_list", ProcessingCycles.Input | ProcessingCycles.Render, ProcessingFlags.Pause)]
    class SystemListSystem : ComponentSystem
    {
        private bool ModalActive = false;
        private bool ModalVisible = false;
        private int SelectedIndex = 0;
        private int StartOffset = -1;

        private int AmountVisible = 16;

        public override bool ShouldSave => false;

        private int RemoveTimer = 0;

        public override void Input()
        {
            if (!ModalActive) return;
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            Vector2D movement = inputMap.Movement;

            if (movement.Magnitude > 1e-5 && Editor.MoveTimeframe.Execute())
            {
                bool shifting = inputMap.Interact.Pressed;

                if (movement.Y > 0)
                {
                    if(shifting)
                    {
                        ComponentSystem selected = Owner.Systems.ElementAtOrDefault(SelectedIndex);
                        if(selected != null)
                        {
                            if(SelectedIndex - 1 >= 0)
                            {
                                Owner.Systems.Dictionary.Insert(selected.SystemName, selected, SelectedIndex - 1);
                                Owner.Systems.QueueOnFlush(() => Owner.Systems.UpdateOrder());
                                SelectedIndex--;
                            }
                        }
                    } else if (SelectedIndex - 1 >= -1) SelectedIndex--;
                }
                else if (movement.Y < 0)
                {
                    if(shifting)
                    {
                        ComponentSystem selected = Owner.Systems.ElementAtOrDefault(SelectedIndex);
                        if (selected != null)
                        {
                            if (SelectedIndex + 1 < Owner.Systems.Count)
                            {
                                Owner.Systems.Dictionary.Insert(selected.SystemName, selected, SelectedIndex + 1);
                                Owner.Systems.QueueOnFlush(() => Owner.Systems.UpdateOrder());
                                SelectedIndex++;
                            }
                        }
                    } else if (SelectedIndex + 1 < Owner.Systems.Count) SelectedIndex++;
                }
                if (movement.Y != 0)
                {
                    RemoveTimer = 0;
                }
            }

            if (inputMap.Jump.Consume())
            {
                if (SelectedIndex == -1)
                {
                    Owner.Events.InvokeEvent(new StartEnumSelectEvent("Available systems", ComponentSystem.GetAllIdentifiers().Where(s => !Owner.Systems.Contains(s)).ToList(), AddSystem, null));
                    Owner.Events.InvokeEvent(new ForceModalChangeEvent("enum_select", null));
                    ModalActive = false;
                }
                else
                {
                    //Owner.Events.InvokeEvent(new EntitySelectEvent(Owner.Systems.ToList()[SelectedIndex], null));
                    //Owner.Events.InvokeEvent(new ForceModalChangeEvent("entity_view", null));
                    //ModalActive = false;
                    //ModalVisible = false;
                }
            }

            if (RemoveTimer > 0) RemoveTimer--;
            if (inputMap.Pulse.Pressed && SelectedIndex >= 0)
            {
                RemoveTimer += 2;
                if (RemoveTimer / 25 > 3)
                {
                    Owner.Systems.Remove(Owner.Systems.ElementAt(SelectedIndex).SystemName);
                    RemoveTimer = 0;
                }
            }
            else RemoveTimer = 0;
            
            if (SelectedIndex < StartOffset)
            {
                StartOffset = SelectedIndex;
            }
            if (SelectedIndex > StartOffset + AmountVisible)
            {
                StartOffset = SelectedIndex - AmountVisible;
            }
        }

        private void AddSystem(string identifier)
        {
            if(!Owner.Systems.Contains(identifier))
            {
                Type type = ComponentSystem.TypeForIdentifier(identifier);
                ComponentSystem system = type.GetConstructor(new Type[0]).Invoke(new object[0]) as ComponentSystem;
                if(system != null)
                {
                    Owner.Systems.Add(system);
                    SelectedIndex = Owner.Systems.Count - 1;
                }
            }
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!ModalVisible) return;
            SelectedIndex = Math.Max(-1, Math.Min(SelectedIndex, Owner.Systems.Count - 1));
            StartOffset = Math.Max(-1, Math.Min(StartOffset, Owner.Systems.Count - 1));
            var layer = r.GetLayerGraphics("hi_res_overlay");

            int y = 14;
            int x = EditorRendering.SidebarX + EditorRendering.SidebarMargin + 4;

            List<ComponentSystem> systems = Owner.Systems.ToList();
            int index = StartOffset;
            for (; index < systems.Count; index++)
            {
                if (index == -1)
                {
                    GUIButton button = new GUIButton(new Vector2D(x - 4 + EditorRendering.SidebarMargin, y), "Add System", new EntityComponentSystem.Util.Rectangle(0, 0, EditorRendering.SidebarWidth - 4 * EditorRendering.SidebarMargin, 24)) { TextSize = 2 };
                    button.Highlighted = SelectedIndex == -1;
                    button.Render(r, layer, Vector2D.Empty);

                    y += 24 + 8;
                }
                else
                {
                    ComponentSystem system = systems[index];
                    if (index == SelectedIndex)
                    {
                        layer.FillRect(new System.Drawing.Rectangle(x - 4, y, EditorRendering.SidebarWidth - 2 * EditorRendering.SidebarMargin, 20), ModalActive ? Color.CornflowerBlue : Color.FromArgb(63, 63, 70));
                    }
                    TextUnit label = new TextUnit(system.SystemName);
                    label.Render<TSurface, TSource>(r, layer, new Point(x, y + 2), 2);
                    y += 20;
                    if (y > 720 - 16) break;
                }
            }
            AmountVisible = index - StartOffset - 1;

            if (RemoveTimer > 0 && SelectedIndex >= 0)
            {
                TextUnit removingLabel = new TextUnit("Removing " + systems[SelectedIndex].SystemName + new string('.', RemoveTimer / 25));
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
    }
}
