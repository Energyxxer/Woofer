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
using WooferGame.Systems.Sounds;
using WooferGame.Systems.Visual;
using Point = System.Drawing.Point;
using Rectangle = EntityComponentSystem.Util.Rectangle;

namespace WooferGame.Meta.LevelEditor.Systems.SoundView
{
    [ComponentSystem("sound_view", ProcessingCycles.Input | ProcessingCycles.Render, ProcessingFlags.Pause),
        Listening(typeof(SelectSoundEvent))]
    class SoundsViewSystem : ComponentSystem
    {
        private bool ModalActive = false;
        private bool ModalVisible = false;

        private bool ComponentLocked = false;

        private int SelectedElementIndex = 0;
        private int SelectedPropertyIndex = 0;

        private int ListFromIndex = 0;
        private List<int> ElementRenderOffsets = new List<int>();

        private List<Sound> Sounds;

        private ObjectSummary SelectedElement = null;

        private int RemoveTimer = 0;

        public override bool ShouldSave => false;

        public string SwitchTo { get; private set; }

        public override void Input()
        {
            if (!ModalActive) return;

            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            Vector2D movement = inputMap.Movement;

            if (movement.Magnitude > 1e-5 && Editor.MoveTimeframe.Execute())
            {
                if (movement.Y > 0)
                {
                    if (ComponentLocked)
                    {
                        if (SelectedPropertyIndex - 1 >= 0) SelectedPropertyIndex--;
                    }
                    else
                    {
                        if (SelectedElementIndex - 1 >= -2) SelectedElementIndex--;
                    }
                }
                else if (movement.Y < 0)
                {
                    if (ComponentLocked)
                    {
                        if (SelectedPropertyIndex + 1 < SelectedElement.Members.Count) SelectedPropertyIndex++;
                    }
                    else
                    {
                        if (SelectedElementIndex + 1 <= Sounds.Count) SelectedElementIndex++;
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
                if (SelectedElementIndex == Sounds.Count)
                {
                    Sounds.Add(new Sound());
                }
                else if (!ComponentLocked)
                {
                    SelectedElement = new ObjectSummary(Owner, Sounds[SelectedElementIndex]);
                    SelectedPropertyIndex = 0;
                    ComponentLocked = true;
                }
                else
                {
                    if (SelectedPropertyIndex >= SelectedElement.Members.Count) return;
                    IMemberSummary member = SelectedElement.Members.Values.ElementAt(SelectedPropertyIndex);
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
            if (inputMap.Pulse.Pressed && SelectedElementIndex >= 0 && !ComponentLocked)
            {
                RemoveTimer += 2;
                if (RemoveTimer / 25 > 3)
                {
                    Sounds.RemoveAt(SelectedElementIndex);
                    RemoveTimer = 0;
                    if (SelectedElementIndex >= Sounds.Count) SelectedElementIndex = Sounds.Count - 1;
                }
            }
            else RemoveTimer = 0;

            if (SelectedElementIndex >= 0 && SelectedElementIndex < ElementRenderOffsets.Count)
            {
                int y = ElementRenderOffsets[SelectedElementIndex];
                if (y < 0)
                {
                    ListFromIndex--;
                }
                else if (y > 720)
                {
                    ListFromIndex = Math.Max(0, SelectedElementIndex - 2);
                }
            }
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!ModalVisible) return;

            var layer = r.GetLayerGraphics("hi_res_overlay");
            System.Drawing.Rectangle sidebar = new System.Drawing.Rectangle(EditorRendering.SidebarX, 0, EditorRendering.SidebarWidth, 720);

            layer.FillRect(sidebar, Color.FromArgb(45, 45, 48));
            layer.FillRect(new System.Drawing.Rectangle(sidebar.X + EditorRendering.SidebarMargin, sidebar.Y + EditorRendering.SidebarMargin, sidebar.Width - 2 * EditorRendering.SidebarMargin, sidebar.Height - 2 * EditorRendering.SidebarMargin), Color.FromArgb(37, 37, 38));

            int x = EditorRendering.SidebarX + 2 * EditorRendering.SidebarMargin;
            int y = EditorRendering.SidebarMargin + 4;

            new TextUnit(EntityViewSystem.ObjectIcon, "Edit sound effects", Color.White).Render(r, layer, new Point(x, y), 2);
            y += 20;

            layer.FillRect(new System.Drawing.Rectangle(x - 2 * EditorRendering.SidebarMargin, y, EditorRendering.SidebarWidth - 2 * EditorRendering.SidebarMargin, 3), Color.FromArgb(45, 45, 48));
            y += 8;

            ElementRenderOffsets.Clear();

            int startY = 0;

            int index = 0;
            foreach (ObjectSummary element in Sounds.Select(a => new ObjectSummary(Owner, a)))
            {
                ElementRenderOffsets.Add(y);

                bool doRender = index >= ListFromIndex;
                if (index == ListFromIndex)
                {
                    startY = y;
                    y = 2 * EditorRendering.SidebarMargin + 20 + 22 + 20 + 8;
                }

                if (index == SelectedElementIndex && doRender)
                {
                    layer.FillRect(new System.Drawing.Rectangle(x - 4, y - 2, EditorRendering.SidebarWidth - 2 * EditorRendering.SidebarMargin, 20), ComponentLocked ? Color.FromArgb(63, 63, 70) : Color.CornflowerBlue);
                }
                if (doRender) new TextUnit(
                     //new Sprite("editor", new Rectangle(0, 0, 16, 16), new Rectangle(0, 32, 16, 16)), 
                     "Sound Effect " + (index + 1))
                     .Render(r, layer, new Point(x, y), 2);
                y += 24;

                int memberIndex = 0;
                foreach (IMemberSummary member in element.Members.Values)
                {
                    if (doRender)
                    {
                        TextUnit label = member.Label;
                        label.Color = ComponentLocked && SelectedElementIndex == index ? (memberIndex == SelectedPropertyIndex ? Color.CornflowerBlue : Color.White) : Color.Gray;
                        label.Render(r, layer, new Point(x + 8, y), 1);
                    }
                    y += 16;
                    memberIndex++;
                }

                y += 16;

                index++;
            }

            {
                ElementRenderOffsets.Add(y);
                GUIButton button = new GUIButton(new Vector2D(x, y), "Add Sound Effect", new Rectangle(0, 0, EditorRendering.SidebarWidth - 4 * EditorRendering.SidebarMargin, 24)) { TextSize = 2 };
                button.Highlighted = SelectedElementIndex == Sounds.Count;
                button.Render(r, layer, Vector2D.Empty);

                for (int i = 0; i < ListFromIndex; i++)
                {
                    ElementRenderOffsets[i] -= startY;
                }
                index++;
                y += 32;
            }


            if (RemoveTimer > 0 && SelectedElementIndex >= 0 && SelectedElementIndex < Sounds.Count)
            {
                TextUnit removingLabel = new TextUnit("Removing Sound Effect " + (SelectedElementIndex + 1) + new string('.', RemoveTimer / 25));
                System.Drawing.Size labelSize = removingLabel.GetSize(3);
                removingLabel.Render(r, layer, new Point(EditorRendering.SidebarX - labelSize.Width, layer.GetSize().Height - labelSize.Height), 3);
            }
        }

        public override void EventFired(object sender, Event e)
        {
            if (e is SelectSoundEvent s)
            {
                Sounds = s.Sounds;
                SelectedElementIndex = 0;
                SelectedPropertyIndex = 0;
                ListFromIndex = 0;

                ComponentLocked = false;
                SwitchTo = (Owner.Systems[ComponentSystem.IdentifierOf<ModalFocusSystem>()] as ModalFocusSystem).CurrentSystem;
            }
            else if (e is ModalChangeEvent ce)
            {
                ModalActive = true;
                ModalVisible = true;
            }
            else if (e is BeginModalChangeEvent bmce)
            {
                if (ComponentLocked) ComponentLocked = false;
                else
                {
                    bmce.SystemName = SwitchTo;
                    ModalActive = false;
                    ModalVisible = false;
                }
            }
        }
    }

    [Event("sound_select")]
    class SelectSoundEvent : Event
    {
        public List<Sound> Sounds;

        public SelectSoundEvent(List<Sound> sounds) : base(null)
        {
            this.Sounds = sounds;
        }
    }
}
