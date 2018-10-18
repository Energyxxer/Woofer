using EntityComponentSystem.Interfaces.Input;
using EntityComponentSystem.Util;

using GameInterfaces.Input;
using GameInterfaces.Input.GamePad;

namespace WooferGame.Input
{
    class GamePadInputMap : IInputMap
    {
        public string Name => "Game Pad";

        public Vector2D Movement => gamePad.Thumbsticks.Left;

        public Vector2D Orientation => gamePad.Thumbsticks.Left;

        public ButtonInput Run { get; private set; }
        public ButtonInput Jump { get; private set; }
        public ButtonInput Pulse { get; private set; }
        public ButtonInput Interact { get; private set; }
        public ButtonInput Start { get; private set; }
        public ButtonInput Pause { get; private set; }
        public ButtonInput Back { get; private set; }

        public bool IsBeingUsed => gamePad.IsBeingUsed;

        public string IconSpritesheet => "gamepad_icons";

        public ButtonInput Debug { get; private set; }
        public Vector2D DebugMovement => Orientation;

        public ButtonInput Quicksave { get; private set; }
        public ButtonInput Quickload { get; private set; }

        private IGamePad gamePad;

        public GamePadInputMap(IGamePad gamePad)
        {
            this.gamePad = gamePad;

            Run = new ButtonInput(() => gamePad.Buttons.Y);
            Jump = new ButtonInput(() => gamePad.Buttons.A);
            Pulse = new ButtonInput(() => gamePad.Buttons.X);
            Interact = new ButtonInput(() => gamePad.Buttons.B);
            Start = new ButtonInput(() => gamePad.Buttons.Start);
            Pause = new ButtonInput(() => gamePad.Buttons.Start);
            Back = new ButtonInput(() => gamePad.Buttons.Back);

            Debug = new ButtonInput(() => gamePad.Buttons.RightStick);
            Quicksave = new ButtonInput(() => gamePad.Buttons.LeftBumper);
            Quickload = new ButtonInput(() => gamePad.Buttons.RightBumper);
        }

        public void SetVibration(float amount) => gamePad.SetVibration(amount, amount);

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
