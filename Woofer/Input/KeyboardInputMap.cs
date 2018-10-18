using EntityComponentSystem.Interfaces.Input;
using EntityComponentSystem.Interfaces.Input.Mouse;
using EntityComponentSystem.Util;

using GameInterfaces.Input;
using GameInterfaces.Input.Keyboard;

namespace WooferGame.Input
{
    class KeyboardInputMap : IInputMap
    {
        public string Name => "Keyboard and Mouse";

        public Vector2D Movement
        {
            get
            {
                Vector2D movement = new Vector2D();
                if (keyboard[Key.Up].IsPressed()) movement += Vector2D.UnitJ;
                if (keyboard[Key.Left].IsPressed()) movement -= Vector2D.UnitI;
                if (keyboard[Key.Down].IsPressed()) movement -= Vector2D.UnitJ;
                if (keyboard[Key.Right].IsPressed()) movement += Vector2D.UnitI;
                return movement.Normalize();
            }
        }

        public Vector2D Orientation => Movement;

        public ButtonInput Run { get; private set; }

        public ButtonInput Jump { get; private set; }

        public ButtonInput Pulse { get; private set; }

        public ButtonInput Interact { get; private set; }

        public ButtonInput Pause { get; private set; }

        public ButtonInput Back { get; private set; }

        public ButtonInput Debug { get; private set; }

        public ButtonInput Start { get; private set; }

        public Vector2D DebugMovement => Movement;

        public ButtonInput Quicksave { get; private set; }

        public ButtonInput Quickload { get; private set; }

        IKeyboard keyboard;
        IMouse mouse;

        public KeyboardInputMap(IKeyboard keyboard, IMouse mouse)
        {
            this.keyboard = keyboard;
            this.mouse = mouse;

            Run = new ButtonInput(() => keyboard[Key.LeftShift]);
            Jump = new ButtonInput(() => keyboard[Key.Z]);
            Pulse = new ButtonInput(() => keyboard[Key.X]);
            Interact = new ButtonInput(() => keyboard[Key.C]);
            Pause = new ButtonInput(() => keyboard[Key.Escape]);
            Back = new ButtonInput(() => keyboard[Key.Tab]);
            Debug = new ButtonInput(() => keyboard[Key.F3]);
            Start = new ButtonInput(() => keyboard[Key.Enter]);
            Quicksave = new ButtonInput(() => keyboard[Key.F5]);
            Quickload = new ButtonInput(() => keyboard[Key.F7]);
        }

        public bool IsBeingUsed => keyboard.IsBeingUsed || mouse.IsBeingUsed;

        public string IconSpritesheet => "keyboard_icons";

        public void SetVibration(float amount) { }

        public void ProcessInput()
        {
            Run.RegisterState();
            Jump.RegisterState();
            Pulse.RegisterState();
            Interact.RegisterState();
            Pause.RegisterState();
            Back.RegisterState();
            Debug.RegisterState();
            Start.RegisterState();
            Quicksave.RegisterState();
            Quickload.RegisterState();
        }
    }
}
