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
using GameInterfaces.GraphicsInterface;
using WooferGame.Common;
using WooferGame.Input;
using static WooferGame.Meta.LevelEditor.Systems.InputModes.StartNumberInputEvent;
using Rectangle = EntityComponentSystem.Util.Rectangle;

namespace WooferGame.Meta.LevelEditor.Systems.InputModes
{
    [ComponentSystem("number_input", ProcessingCycles.Input | ProcessingCycles.Render, ProcessingFlags.Pause),
        Listening(typeof(StartNumberInputEvent))]
    class NumberInputSystem : ComponentSystem
    {
        private List<GUIButton> Pad;

        private bool ModalActive = false;

        private string Label = null;
        private string Display = "0";
        private bool Dirty = false;

        private OnSubmit Callback = null;

        private string SwitchTo = null;

        public override bool ShouldSave => false;

        private GUIButton DotButton = null;

        public NumberInputSystem()
        {
            Pad = new List<GUIButton>();

            int btnSize = 36;
            Rectangle btnBounds = new Rectangle(-btnSize / 2, -btnSize / 2, btnSize, btnSize);

            Pad.Add(new GUIButton(new Vector2D(0, 0), "1", btnBounds) { Highlighted = true });
            Pad.Add(new GUIButton(new Vector2D(btnSize + 4, 0), "2", btnBounds));
            Pad.Add(new GUIButton(new Vector2D(btnSize + 4 + btnSize + 4, 0), "3", btnBounds));
            Pad.Add(new GUIButton(new Vector2D(0, btnSize + 4), "4", btnBounds));
            Pad.Add(new GUIButton(new Vector2D(btnSize + 4, btnSize + 4), "5", btnBounds));
            Pad.Add(new GUIButton(new Vector2D(btnSize + 4 + btnSize + 4, btnSize + 4), "6", btnBounds));
            Pad.Add(new GUIButton(new Vector2D(0, (btnSize + 4) * 2), "7", btnBounds));
            Pad.Add(new GUIButton(new Vector2D(btnSize + 4, (btnSize + 4) * 2), "8", btnBounds));
            Pad.Add(new GUIButton(new Vector2D(btnSize + 4 + btnSize + 4, (btnSize + 4) * 2), "9", btnBounds));
            Pad.Add(new GUIButton(new Vector2D(0, (btnSize + 4) * 3), "-", btnBounds));
            Pad.Add(new GUIButton(new Vector2D(btnSize + 4, (btnSize + 4) * 3), "0", btnBounds));
            Pad.Add(DotButton = new GUIButton(new Vector2D(btnSize + 4 + btnSize + 4, (btnSize + 4) * 3), ".", btnBounds));

            Pad.Add(new GUIButton(new Vector2D(-btnSize-4, -2*(btnSize*3/4-4)), "^", new Rectangle(-btnSize / 2, -btnSize / 4, btnSize, btnSize / 2)) { TextSize = 2, Repeating = true });
            Pad.Add(new GUIButton(new Vector2D(-btnSize-4, -btnSize*3/4-4), "v", new Rectangle(-btnSize / 2, -btnSize / 4, btnSize, btnSize / 2)) { TextSize = 2, Repeating = true });

            Pad.Add(new GUIButton(new Vector2D(-(btnSize + 4), 0), "\b", btnBounds));
            Pad.Add(new GUIButton(new Vector2D(-(btnSize + 4), btnSize + 4), "\u0018", btnBounds));

            Pad.Add(new GUIButton(new Vector2D(-(btnSize + 4), (btnSize + 4)*5/2), "\u0007", new Rectangle(-btnSize/2, -btnSize-2, btnSize, btnSize*2+4)));
        }


        private Vector2D CurrentPos = new Vector2D();
        private GUIButton CurrentButton => Pad?.Find(b => b.Position == CurrentPos);

        public override void Input()
        {
            if (!ModalActive) return;
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            if(inputMap.Movement.Magnitude > 0 && Editor.MoveTimeframe.Execute())
            {
                Vector2D screenMovement = inputMap.Movement;
                screenMovement.Y *= -1;
                GUIButton nextButton = Pad.Where(b => b.Enabled && b.Position != CurrentPos && GeneralUtil.SubtractAngles((b.Position - CurrentPos).Angle, screenMovement.Angle) < Math.PI / 2).OrderBy(b => (b.Position - CurrentPos).Magnitude).FirstOrDefault();
                if (nextButton != null)
                {
                    SwitchButton(nextButton);
                }
            }

            if(CurrentButton != null && ((CurrentButton.Repeating && inputMap.Jump.ConsumeRepeating()) || (!CurrentButton.Repeating && inputMap.Jump.Consume())))
            {
                GUIButton button = CurrentButton;
                if(button != null && button.Enabled)
                {
                    if(char.IsDigit(button.Display[0]))
                    {
                        Display += button.Display[0];
                    } else if(button.Display[0] == '-')
                    {
                        if (Display == "0")
                        {
                            //Do nothing if zero
                        }
                        else if(Display.StartsWith("-"))
                        {
                            Display = Display.Substring(1);
                        } else
                        {
                            Display = "-" + Display;
                        }
                    } else if(button.Display[0] == '.')
                    {
                        if (!Display.Contains('.')) Display += '.';
                    } else if(button.Display[0] == '^' || button.Display[0] == 'v')
                    {
                        Normalize();
                        double value = double.Parse(Display);
                        if (button.Display[0] == '^') value++;
                        else value--;
                        Display = value.ToString();
                    } else if(button.Display[0] == '\b')
                    {
                        if (Display.Length > 0) Display = Display.Substring(0, Display.Length - 1);
                    } else if(button.Display[0] == '\u0018')
                    {
                        Display = "0";
                    } else if (button.Display[0] == '\u0007')
                    {
                        Submit();
                    }
                    Dirty = true;
                }
            }
        }

        private void SwitchButton(GUIButton button)
        {
            if (CurrentButton != null) CurrentButton.Highlighted = false;
            button.Highlighted = true;
            CurrentPos = button.Position;
        }

        bool ShouldClose = false;

        private void Submit()
        {
            double value = double.Parse(Display);
            ShouldClose = true;
            Callback(value);
            if(ShouldClose) Owner.Events.InvokeEvent(new RequestModalChangeEvent(null));
        }

        private void Normalize()
        {
            bool negative = Display.StartsWith("-");
            if (negative) Display = Display.Substring(1);

            int dotIndex = Display.IndexOf('.');
            if(dotIndex > -1)
            {
                if (dotIndex == 0) {
                    Display = "0" + Display;
                    dotIndex++;
                }
                if (dotIndex == Display.Length - 1)
                {
                    Display = Display.Substring(0, dotIndex);
                }
            }

            if (Display.Length == 0)
            {
                Display = "0";
                negative = false;
            }

            if(negative) Display = "-" + Display;
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!ModalActive) return;
            if (Dirty)
            {
                bool negative = Display.StartsWith("-");
                if (negative) Display = Display.Substring(1);

                if (Display.Length == 0)
                {
                    Display = "0";
                    negative = false;
                }

                int i;
                for(i = 0; i < Display.Length; i++)
                {
                    if (Display[i] == '0' && i + 1 < Display.Length && Display[i + 1] != '.') continue;
                    else break;
                }
                Display = (negative ? "-" : "") + Display.Substring(i);

                Dirty = false;
            }

            var layer = r.GetLayerGraphics("hi_res_overlay");

            if (Label != null)
            {
                TextUnit label = new TextUnit(Label);
                label.Render(r, layer, new Point(80 - 18, 500 - 18 - 30 - 30), 3);
            }

            TextUnit numberDisplay = new TextUnit(Display);
            layer.FillRect(new System.Drawing.Rectangle(80 - 18, 500 - 18 - 30 -7, 40 * 3 - 4, 36-3), Color.FromArgb(27, 27, 28));
            numberDisplay.Render(r, layer, new Point(80 - 18+4, 500 - 18 - 30), 3);

            foreach (GUIButton button in Pad)
            {
                button.Render(r, layer, new Vector2D(80, 500));
            }
        }

        public override void EventFired(object sender, Event e)
        {
            if(e is StartNumberInputEvent start)
            {
                Display = start.InitialValue.ToString();
                Callback = start.Callback;
                DotButton.Enabled = start.AllowDecimals;
                Label = start.Label;
            } else if(e is ModalChangeEvent change)
            {
                //Console.WriteLine("changed from " + change.From);
                ShouldClose = false;
                if(change.From != "number_input")
                {
                    SwitchTo = change.From;
                }
                SwitchButton(Pad[0]);
                ModalActive = true;
            } else if(e is BeginModalChangeEvent bmce)
            {
                bmce.SystemName = SwitchTo;
                ModalActive = false;
            }
        }
    }

    internal class GUIButton
    {
        public Vector2D Position;
        public string Display;
        public Rectangle Bounds;
        public bool Enabled = true;
        public bool Highlighted = false;
        public bool Focused = true;
        public bool Repeating = false;
        public int TextSize = 3;

        public GUIButton(Vector2D position, string display, Rectangle bounds)
        {
            Position = position;
            Display = display;
            Bounds = bounds;
        }
        
        public virtual void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r, DirectGraphicsContext<TSurface, TSource> layer, Vector2D offset)
        {
            if (!Enabled) return;
            layer.FillRect((Bounds + (Position + offset)).ToDrawing(), Highlighted ? Focused ? Color.CornflowerBlue : Color.FromArgb(63, 63, 70) : Color.FromArgb(37, 37, 38));
            TextUnit label = new TextUnit(Display);
            Rectangle displayBounds = (Bounds + (Position + offset));
            System.Drawing.Size labelSize = label.GetSize(TextSize);
            label.Render(r, layer, new Point((int)(displayBounds.X + displayBounds.Width / 2 - labelSize.Width / 2), (int)(displayBounds.Y + displayBounds.Height / 2 - labelSize.Height / 2)), TextSize);
        }
    }

    [Event("start_number_input")]
    public class StartNumberInputEvent : Event
    {
        public string Label = null;
        public double InitialValue;
        public OnSubmit Callback;
        public bool AllowDecimals;

        public StartNumberInputEvent(double initialValue, OnSubmit callback, Component sender) : this(initialValue, callback, true, sender) { }

        public StartNumberInputEvent(double initialValue, OnSubmit callback, bool allowDecimals, Component sender) : base(sender)
        {
            InitialValue = initialValue;
            Callback = callback;
            AllowDecimals = allowDecimals;
        }

        public delegate void OnSubmit(double value);
    }
}
