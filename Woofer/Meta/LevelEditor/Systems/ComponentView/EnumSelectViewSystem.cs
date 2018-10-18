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
using WooferGame.Common;
using WooferGame.Input;
using static WooferGame.Meta.LevelEditor.Systems.ComponentView.StartEnumSelectEvent;

namespace WooferGame.Meta.LevelEditor.Systems.ComponentView
{
    [ComponentSystem("enum_select", ProcessingCycles.Input | ProcessingCycles.Render, ProcessingFlags.Pause),
        Listening(typeof(StartEnumSelectEvent))]
    class EnumerationSelectViewSystem : ComponentSystem
    {
        private bool ModalActive = false;
        private int SelectedIndex = 0;
        private int StartOffset = 0;

        private int AmountVisible = 16;

        public override bool ShouldSave => false;

        private string Title;
        private List<string> Options;

        private OnSubmit Callback = null;
        private string SwitchTo = null;

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
                    if (SelectedIndex + 1 < Options.Count) SelectedIndex++;
                }
            }

            if (SelectedIndex < StartOffset)
            {
                StartOffset = SelectedIndex;
            }
            if (SelectedIndex > StartOffset + AmountVisible)
            {
                StartOffset = SelectedIndex - AmountVisible;
            }

            if (inputMap.Jump.Consume())
            {
                Submit();
            }
        }

        private void Submit()
        {
            ModalActive = false;
            Owner.Events.InvokeEvent(new ForceModalChangeEvent(SwitchTo, null));
            if(SelectedIndex < Options.Count) Callback(Options[SelectedIndex]);
            //if (ShouldClose && !ModalActive && !Owner.Disposed) Owner.Events.InvokeEvent(new RequestModalChangeEvent(null));
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!ModalActive) return;
            SelectedIndex = Math.Max(0, Math.Min(SelectedIndex, Options.Count - 1));
            StartOffset = Math.Max(0, Math.Min(StartOffset, Options.Count - 1));
            var layer = r.GetLayerGraphics("hi_res_overlay");

            System.Drawing.Rectangle sidebar = new System.Drawing.Rectangle(EditorRendering.SidebarX, 0, EditorRendering.SidebarWidth, 720);

            int titleHeight = 24;
            
            layer.FillRect(sidebar, Color.FromArgb(45, 45, 48));
            layer.FillRect(new System.Drawing.Rectangle(sidebar.X + EditorRendering.SidebarMargin, sidebar.Y + EditorRendering.SidebarMargin+titleHeight, sidebar.Width - 2 * EditorRendering.SidebarMargin, sidebar.Height - 2 * EditorRendering.SidebarMargin-titleHeight), Color.FromArgb(37, 37, 38));


            int y = 10+titleHeight;
            int x = EditorRendering.SidebarX + EditorRendering.SidebarMargin;

            new TextUnit(Title).Render(r, layer, new Point(x + 8, 10), 2);
            
            int index = StartOffset;
            for (; index < Options.Count; index++)
            {
                string identifier = Options[index];
                if (index == SelectedIndex)
                {
                    layer.FillRect(new System.Drawing.Rectangle(x, y, EditorRendering.SidebarWidth - 2 * EditorRendering.SidebarMargin, 20), ModalActive ? Color.CornflowerBlue : Color.FromArgb(63, 63, 70));
                }
                TextUnit label = new TextUnit(identifier);
                label.Render<TSurface, TSource>(r, layer, new Point(x, y + 2), 2);
                y += 20;
                if (y > 720 - 16) break;
            }
            AmountVisible = index - StartOffset - 1;
        }

        public override void EventFired(object sender, Event e)
        {
            if (e is BeginModalChangeEvent bmce)
            {
                bmce.SystemName = SwitchTo;
                ModalActive = false;
            }
            else if (e is ModalChangeEvent change)
            {
                if(change.From != "enum_select") SwitchTo = change.From;
                ModalActive = true;
            } else if(e is StartEnumSelectEvent start)
            {
                Title = start.Title;
                Options = start.Options;
                Callback = start.Callback;
            }
        }
    }

    [Event("start_enum_select")]
    public class StartEnumSelectEvent : Event
    {
        public string Title;
        public List<String> Options;
        public OnSubmit Callback;
        
        public StartEnumSelectEvent(string title, List<String> options, OnSubmit callback, Component sender) : base(sender)
        {
            Title = title;
            Options = options;
            Callback = callback;
        }

        public delegate void OnSubmit(string value);
    }
}
