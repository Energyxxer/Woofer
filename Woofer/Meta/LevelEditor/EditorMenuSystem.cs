using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Events;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;
using WooferGame.Common;
using WooferGame.Controller.Commands;
using WooferGame.Input;
using WooferGame.Meta.LevelEditor.Systems;
using WooferGame.Meta.LevelEditor.Systems.CursorModes;
using WooferGame.Meta.LevelEditor.Systems.EntityView;
using WooferGame.Meta.LevelEditor.Systems.InputModes;
using WooferGame.Meta.LevelEditor.Systems.ObjectEditor;
using WooferGame.Scenes;
using WooferGame.Systems.Visual;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;

namespace WooferGame.Meta.LevelEditor
{
    [ComponentSystem("editor_menu", ProcessingCycles.Input | ProcessingCycles.Update | ProcessingCycles.Render, ProcessingFlags.Pause)]
    class EditorMenuSystem : ComponentSystem
    {
        private bool ModalActive = false;
        private bool ModalVisible = true;

        private int SelectedIndex = 0;
        
        public override bool ShouldSave => false;

        private double SavedTimer = 0;

        public override void Input()
        {
            if (!ModalActive) return;
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            Vector2D movement = inputMap.Movement;

            if (movement.Magnitude > 1e-5 && Editor.MoveTimeframe.Execute())
            {
                if (movement.Y > 0)
                {
                    if (SelectedIndex - 1 >= 0) SelectedIndex--;
                }
                else if (movement.Y < 0)
                {
                    if (SelectedIndex + 1 <= 5) SelectedIndex++;
                }
            }

            if (inputMap.Jump.Consume())
            {
                switch(SelectedIndex)
                {
                    case 0:
                        {
                            PropertySummary property = new PropertySummary(Owner, typeof(Scene).GetProperty("Name"), Owner);
                            property.TriggerEdit(inputMap.Interact.Pressed);
                            ModalActive = false;
                            break;
                        }
                    case 1:
                        {
                            WooferSaveOperation save = new WooferSaveOperation(Owner, Owner.Name, Woofer.Controller.CurrentSave.DirectoryName);
                            save.Save();
                            SavedTimer = 3;
                            break;
                        }
                    case 2:
                        {
                            Owner.Events.InvokeEvent(new ForceModalChangeEvent("entity_list", null));
                            ModalActive = false;
                            ModalVisible = false;
                            break;
                        }
                    case 3:
                        {
                            Owner.Events.InvokeEvent(new ForceModalChangeEvent("system_list", null));
                            ModalActive = false;
                            ModalVisible = false;
                            break;
                        }
                    case 4:
                        {
                            Owner.Events.InvokeEvent(new StartObjectEditEvent(Woofer.Controller.CurrentSave.Data, "Edit Save Data", "editor_menu"));
                            Owner.Events.InvokeEvent(new ForceModalChangeEvent("object_editor", null));
                            ModalActive = false;
                            ModalVisible = false;
                            break;
                        }
                    case 5:
                        {
                            Woofer.Controller.CommandFired(new InternalSceneChangeCommand(new MainMenuScene()));
                            Woofer.Controller.Paused = false;
                            break;
                        }
                }
            }
        }

        internal static readonly Sprite CogIcon = new Sprite("editor", new Rectangle(0, 0, 16, 16), new Rectangle(0, 32, 16, 16));

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!ModalVisible) return;

            var layer = r.GetLayerGraphics("hi_res_overlay");

            System.Drawing.Rectangle sidebar = new System.Drawing.Rectangle(EditorRendering.SidebarX, 0, EditorRendering.SidebarWidth, 720);

            layer.FillRect(sidebar, Color.FromArgb(45, 45, 48));
            layer.FillRect(new System.Drawing.Rectangle(sidebar.X + EditorRendering.SidebarMargin, sidebar.Y + EditorRendering.SidebarMargin, sidebar.Width - 2 * EditorRendering.SidebarMargin, sidebar.Height - 2 * EditorRendering.SidebarMargin), Color.FromArgb(37, 37, 38));

            int index = 0;
            int x = EditorRendering.SidebarX + 2 * EditorRendering.SidebarMargin;
            int y = EditorRendering.SidebarMargin + 4;

            new TextUnit(CogIcon, Owner.Name, index == SelectedIndex ? Color.CornflowerBlue : Color.White).Render(r, layer, new Point(x, y), 2);
            y += 20;
            index++;

            layer.FillRect(new System.Drawing.Rectangle(x - 2 * EditorRendering.SidebarMargin, y, EditorRendering.SidebarWidth - 2 * EditorRendering.SidebarMargin, 3), Color.FromArgb(45, 45, 48));
            y += 8;

            Rectangle buttonBounds = new Rectangle(0, 0, EditorRendering.SidebarWidth - 4 * EditorRendering.SidebarMargin, 24);

            {
                new GUIButton(Vector2D.Empty, SavedTimer == 0 ? "Save" : "Saved", new Rectangle(buttonBounds)) { TextSize = 2, Highlighted = SelectedIndex == index, Focused = ModalActive }.Render(r, layer, new Vector2D(x, y));
                y += 32;
                index++;
            }

            {
                new GUIButton(Vector2D.Empty, "Entities", new Rectangle(buttonBounds)) { TextSize = 2, Highlighted = SelectedIndex == index, Focused = ModalActive }.Render(r, layer, new Vector2D(x, y));
                y += 32;
                index++;
            }

            {
                new GUIButton(Vector2D.Empty, "Systems", new Rectangle(buttonBounds)) { TextSize = 2, Highlighted = SelectedIndex == index, Focused = ModalActive }.Render(r, layer, new Vector2D(x, y));
                y += 32;
                index++;
            }

            {
                new GUIButton(Vector2D.Empty, "Edit Save Data", new Rectangle(buttonBounds)) { TextSize = 2, Highlighted = SelectedIndex == index, Focused = ModalActive }.Render(r, layer, new Vector2D(x, y));
                y += 32;
                index++;
            }

            {
                new GUIButton(Vector2D.Empty, "Quit", new Rectangle(buttonBounds)) { TextSize = 2, Highlighted = SelectedIndex == index, Focused = ModalActive }.Render(r, layer, new Vector2D(x, y));
                y += 32;
                index++;
            }

            /*foreach (IMemberSummary member in Members)
            {
                TextUnit label = member.Label;
                label.Color = SelectedPropertyIndex == index ? Color.CornflowerBlue : Color.White;
                label.Render(r, layer, new Point(x + 8, y), 2);
                y += 24;
                index++;
            }*/
        }

        public override void Update()
        {
            if(SavedTimer > 0)
            {
                SavedTimer -= Owner.DeltaTime;
                if (SavedTimer < 0) SavedTimer = 0;
            }
        }

        public override void EventFired(object sender, Event e)
        {
            if (e is ModalChangeEvent)
            {
                ModalActive = true;
                ModalVisible = true;
            }
            else if (e is BeginModalChangeEvent bmce)
            {
                bmce.SystemName = "editor_cursor";
                ModalActive = false;
            }
        }
    }
}
