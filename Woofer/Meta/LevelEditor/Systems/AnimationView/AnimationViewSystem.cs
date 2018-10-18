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
using WooferGame.Systems.Visual.Animation;
using Point = System.Drawing.Point;
using Rectangle = EntityComponentSystem.Util.Rectangle;

namespace WooferGame.Meta.LevelEditor.Systems.AnimationView
{
    [ComponentSystem("animation_view", ProcessingCycles.Input | ProcessingCycles.Render, ProcessingFlags.Pause),
        Listening(typeof(SelectAnimationEvent))]
    class AnimationViewSystem : ComponentSystem
    {
        private bool ModalActive = false;
        private bool ModalVisible = false;

        private bool ComponentLocked = false;

        private int SelectedAnimationIndex = 0;
        private int SelectedPropertyIndex = 0;

        private int ListFromIndex = 0;
        private List<int> AnimationRenderOffsets = new List<int>();

        private List<AnimatedSprite> Animations;

        private ObjectSummary SelectedAnimation = null;

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
                        if (SelectedAnimationIndex - 1 >= -2) SelectedAnimationIndex--;
                    }
                }
                else if (movement.Y < 0)
                {
                    if (ComponentLocked)
                    {
                        if (SelectedPropertyIndex + 1 < SelectedAnimation.Members.Count) SelectedPropertyIndex++;
                    }
                    else
                    {
                        if (SelectedAnimationIndex + 1 <= Animations.Count ) SelectedAnimationIndex++;
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
                if (SelectedAnimationIndex == Animations.Count)
                {
                    Animations.Add(new AnimatedSprite());
                }
                else if (!ComponentLocked)
                {
                    SelectedAnimation = new ObjectSummary(Owner, Animations[SelectedAnimationIndex]);
                    SelectedPropertyIndex = 0;
                    ComponentLocked = true;
                }
                else
                {
                    if (SelectedPropertyIndex >= SelectedAnimation.Members.Count) return;
                    IMemberSummary member = SelectedAnimation.Members.Values.ElementAt(SelectedPropertyIndex);
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
            if (inputMap.Pulse.Pressed && SelectedAnimationIndex >= 0 && !ComponentLocked)
            {
                RemoveTimer += 2;
                if (RemoveTimer / 25 > 3)
                {
                    Animations.RemoveAt(SelectedAnimationIndex);
                    RemoveTimer = 0;
                    if (SelectedAnimationIndex >= Animations.Count) SelectedAnimationIndex = Animations.Count - 1;
                }
            }
            else RemoveTimer = 0;

            if (SelectedAnimationIndex >= 0 && SelectedAnimationIndex < AnimationRenderOffsets.Count)
            {
                int y = AnimationRenderOffsets[SelectedAnimationIndex];
                if (y < 0)
                {
                    ListFromIndex--;
                }
                else if (y > 720)
                {
                    ListFromIndex = Math.Max(0, SelectedAnimationIndex - 2);
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

            new TextUnit(EntityViewSystem.ObjectIcon, "Edit sprite animations", Color.White).Render(r, layer, new Point(x, y), 2);
            y += 20;

            layer.FillRect(new System.Drawing.Rectangle(x - 2 * EditorRendering.SidebarMargin, y, EditorRendering.SidebarWidth - 2 * EditorRendering.SidebarMargin, 3), Color.FromArgb(45, 45, 48));
            y += 8;

            AnimationRenderOffsets.Clear();

            int startY = 0;

            int index = 0;
            foreach (ObjectSummary animation in Animations.Select(a => new ObjectSummary(Owner, a)))
            {
                AnimationRenderOffsets.Add(y);

                bool doRender = index >= ListFromIndex;
                if (index == ListFromIndex)
                {
                    startY = y;
                    y = 2 * EditorRendering.SidebarMargin + 20 + 22 + 20 + 8;
                }

                if (index == SelectedAnimationIndex && doRender)
                {
                    layer.FillRect(new System.Drawing.Rectangle(x - 4, y - 2, EditorRendering.SidebarWidth - 2 * EditorRendering.SidebarMargin, 20), ComponentLocked ? Color.FromArgb(63, 63, 70) : Color.CornflowerBlue);
                }
                if (doRender) new TextUnit(
                     //new Sprite("editor", new Rectangle(0, 0, 16, 16), new Rectangle(0, 32, 16, 16)), 
                     "Animation " + (index+1))
                     .Render(r, layer, new Point(x, y), 2);
                y += 24;

                int memberIndex = 0;
                foreach (IMemberSummary member in animation.Members.Values)
                {
                    if (doRender)
                    {
                        TextUnit label = member.Label;
                        label.Color = ComponentLocked && SelectedAnimationIndex == index ? (memberIndex == SelectedPropertyIndex ? Color.CornflowerBlue : Color.White) : Color.Gray;
                        label.Render(r, layer, new Point(x + 8, y), 1);
                    }
                    y += 16;
                    memberIndex++;
                }

                y += 16;

                index++;
            }

            {
                AnimationRenderOffsets.Add(y);
                GUIButton button = new GUIButton(new Vector2D(x, y), "Add Animation", new Rectangle(0, 0, EditorRendering.SidebarWidth - 4 * EditorRendering.SidebarMargin, 24)) { TextSize = 2 };
                button.Highlighted = SelectedAnimationIndex == Animations.Count;
                button.Render(r, layer, Vector2D.Empty);

                for (int i = 0; i < ListFromIndex; i++)
                {
                    AnimationRenderOffsets[i] -= startY;
                }
                index++;
                y += 32;
            }


            if (RemoveTimer > 0 && SelectedAnimationIndex >= 0 && SelectedAnimationIndex < Animations.Count)
            {
                TextUnit removingLabel = new TextUnit("Removing Animation " + (SelectedAnimationIndex+1) + new string('.', RemoveTimer / 25));
                System.Drawing.Size labelSize = removingLabel.GetSize(3);
                removingLabel.Render(r, layer, new Point(EditorRendering.SidebarX - labelSize.Width, layer.GetSize().Height - labelSize.Height), 3);
            }
        }

        public override void EventFired(object sender, Event e)
        {
            if (e is SelectAnimationEvent s)
            {
                Animations = s.Animations;
                SelectedAnimationIndex = 0;
                SelectedPropertyIndex = 0;
                ListFromIndex = 0;

                ComponentLocked = false;
            }
            else if (e is ModalChangeEvent ce)
            {
                ModalActive = true;
                ModalVisible = true;
                if(ce.From != "number_input") SwitchTo = ce.From;
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

    [Event("animatable_select")]
    class SelectAnimationEvent : Event
    {
        public List<AnimatedSprite> Animations;

        public SelectAnimationEvent(List<AnimatedSprite> animations) : base(null)
        {
            this.Animations = animations;
        }
    }
}
