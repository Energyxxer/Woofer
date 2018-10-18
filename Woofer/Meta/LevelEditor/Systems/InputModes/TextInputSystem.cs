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
using static WooferGame.Meta.LevelEditor.Systems.InputModes.StartTextInputEvent;
using Rectangle = EntityComponentSystem.Util.Rectangle;

namespace WooferGame.Meta.LevelEditor.Systems.InputModes
{
    [ComponentSystem("text_input", ProcessingCycles.Input | ProcessingCycles.Update | ProcessingCycles.Render, ProcessingFlags.Pause),
        Listening(typeof(StartTextInputEvent))]
    class TextInputSystem : ComponentSystem
    {
        private List<GUIButton> Pad;

        private bool ModalActive = false;

        private string Label = null;
        private string Display = "";

        private int CaretIndex = 0;

        private OnSubmit Callback = null;

        private string SwitchTo = null;

        public override bool ShouldSave => false;

        private double CaretBlinkingProgress = 0;
        private double CaretBlinkingPeriod = 1;

        private string[] rows =
        {
            "`1234567890-=\b",
            " qwertyuiop[]\u0007",
            " asdfghjkl'\\",
            "\u001Bzxcvbnm,./",
            "         \u0011\u0012"
        };
        private string[] shiftRows =
        {
            "~!@#$%^&*()_+\b",
            " QWERTYUIOP{}\u0007",
            " ASDFGHJKL:\"|",
            "\u001BZXCVBNM<>?",
            "         \u0011\u0012"
        };

        internal bool Shift = false;

        public TextInputSystem()
        {
            Pad = new List<GUIButton>();

            int btnSize = 36;
            Rectangle btnBounds = new Rectangle(-btnSize / 2, -btnSize / 2, btnSize, btnSize);
            int spacebarSize = (btnSize+4) * 7 - 4;
            Rectangle spaceBounds = new Rectangle(-spacebarSize / 2, -btnSize / 2, spacebarSize, btnSize);

            int y = 0;
            for(int i = 0; i < rows.Length; i++)
            {
                string row = rows[i];
                int x = 0;
                for(int j = 0; j < row.Length; j++)
                {
                    char c = row[j];
                    if (c != ' ') Pad.Add(new GUIKeyboardButton(this, new Vector2D(x, y), c + "", shiftRows[i][j] + "", btnBounds) { Repeating = true });
                    x += btnSize + 4;
                }
                y += btnSize + 4;
            }
            y -= btnSize + 4;
            Pad.Add(new GUIButton(new Vector2D((btnSize + 4) * 5, y), " ", spaceBounds) { Repeating = true });

            //Pad.Add(new GUIButton(new Vector2D(-(btnSize + 4), 0), "\b", btnBounds));
            //Pad.Add(new GUIButton(new Vector2D(-(btnSize + 4), btnSize + 4), "\u0018", btnBounds));

            //Pad.Add(new GUIButton(new Vector2D(-(btnSize + 4), (btnSize + 4) * 5 / 2), "\u0007", new Rectangle(-btnSize / 2, -btnSize - 2, btnSize, btnSize * 2 + 4)));
        }


        private Vector2D CurrentPos = new Vector2D();
        private GUIButton CurrentButton => Pad?.Find(b => b.Position == CurrentPos);

        public override void Input()
        {
            if (!ModalActive) return;
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            if (inputMap.Movement.Magnitude > 0 && Editor.MoveTimeframe.Execute())
            {
                Vector2D screenMovement = inputMap.Movement;
                screenMovement.Y *= -1;

                Vector2D movementSource = CurrentPos;
                GUIButton button = CurrentButton;
                /*if(button != null)
                {
                    movementSource.X += screenMovement.X * button.Bounds.Width / 2;
                    movementSource.Y += screenMovement.Y * button.Bounds.Height / 2;
                }*/

                GUIButton nextButton = Pad.Where(b => b.Enabled && b.Position != CurrentPos && GeneralUtil.SubtractAngles((b.Position - CurrentPos).Angle, screenMovement.Angle) < Math.PI / 2).OrderBy(b => GeneralUtil.SubtractAngles((b.Position - CurrentPos).Angle, screenMovement.Angle)).ThenBy(b => (b.Position - movementSource).Magnitude).FirstOrDefault();
                if (nextButton != null)
                {
                    SwitchButton(nextButton);
                }
            }

            if (CurrentButton != null && ((CurrentButton.Repeating && inputMap.Jump.ConsumeRepeating()) || (!CurrentButton.Repeating && inputMap.Jump.Consume())))
            {
                GUIButton button = CurrentButton;
                if (button != null && button.Enabled)
                {
                    if (button.Display[0] == '\b')
                    {
                        if (CaretIndex > 0 && Display.Length > 0)
                        {
                            Display = Display.Substring(0, CaretIndex - 1) + Display.Substring(CaretIndex);
                            CaretIndex--;
                        }
                    } else if(button.Display[0] == '\u001B') // SHIFT
                    {
                        Shift = !Shift;
                    } else if(button.Display[0] == '\u0011') // LEFT
                    {
                        if(CaretIndex > 0) CaretIndex--;
                    } else if(button.Display[0] == '\u0012') // RIGHT
                    {
                        if(CaretIndex < Display.Length) CaretIndex++;
                    }
                    else if (button.Display[0] == '\u0007')
                    {
                        Submit();
                    } else
                    {
                        Display = Display.Substring(0, CaretIndex) + button.Display + Display.Substring(CaretIndex);
                        CaretIndex++;
                    }

                }
            }
        }

        public override void Update()
        {
            if (!ModalActive) return;
            CaretBlinkingProgress += Owner.DeltaTime;
            while(CaretBlinkingProgress > CaretBlinkingPeriod)
            {
                CaretBlinkingProgress -= CaretBlinkingPeriod;
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
            ShouldClose = true;
            Callback(Display);
            if (ShouldClose) Owner.Events.InvokeEvent(new RequestModalChangeEvent(null));
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (!ModalActive) return;

            var layer = r.GetLayerGraphics("hi_res_overlay");

            if (Label != null)
            {
                TextUnit label = new TextUnit(Label);
                label.Render(r, layer, new Point(80 - 18, 500 - 18 - 30 - 30), 3);
            }

            TextUnit textDisplay = new TextUnit(Display);
            layer.FillRect(new System.Drawing.Rectangle(80 - 18, 500 - 18 - 30 - 7, 40 * 13 - 4, 36 - 3), Color.FromArgb(27, 27, 28));
            textDisplay.Render(r, layer, new Point(80 - 18 + 4, 500 - 18 - 30), 3);

            if(CaretBlinkingProgress <= CaretBlinkingPeriod/2)
            {
                int caretX = 80 - 18 + new TextUnit(Display.Substring(0, CaretIndex)).GetSize(3).Width + 6;
                layer.FillRect(new System.Drawing.Rectangle(caretX, 500 - 18 - 30, 2, 24), Color.White);
            }

            foreach (GUIButton button in Pad)
            {
                button.Render(r, layer, new Vector2D(80, 500));
            }
        }

        public override void EventFired(object sender, Event e)
        {
            if (e is StartTextInputEvent start)
            {
                Display = start.InitialValue;
                CaretIndex = start.InitialValue.Length;
                CaretBlinkingProgress = 0;
                Callback = start.Callback;
                Label = start.Label;
            }
            else if (e is ModalChangeEvent change)
            {
                ShouldClose = false;
                if (change.From != "text_input")
                {
                    SwitchTo = change.From;
                }
                SwitchButton(Pad[0]);
                ModalActive = true;
            }
            else if (e is BeginModalChangeEvent bmce)
            {
                bmce.SystemName = SwitchTo;
                ModalActive = false;
            }
        }
    }

    [Event("start_text_input")]
    public class StartTextInputEvent : Event
    {
        public string Label = null;
        public string InitialValue;
        public OnSubmit Callback;
        
        public StartTextInputEvent(string initialValue, OnSubmit callback, Component sender) : base(sender)
        {
            InitialValue = initialValue;
            Callback = callback;
        }

        public delegate void OnSubmit(string value);
    }

    internal class GUIKeyboardButton : GUIButton
    {
        private TextInputSystem Owner;
        private string NormalDisplay;
        private string ShiftDisplay;

        public GUIKeyboardButton(TextInputSystem owner, Vector2D position, string display, string altDisplay, Rectangle bounds) : base(position, display, bounds)
        {
            Owner = owner;
            NormalDisplay = display;
            ShiftDisplay = altDisplay;
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r, DirectGraphicsContext<TSurface, TSource> layer, Vector2D offset)
        {
            Display = Owner.Shift ? ShiftDisplay : NormalDisplay;
            base.Render(r, layer, offset);
        }
    }
}
