using EntityComponentSystem.Interfaces.Input;
using EntityComponentSystem.Util;

using GameInterfaces.Input;

namespace WooferGame.Input
{
    public interface IInputMap
    {
        string Name { get; }

        Vector2D Movement { get; }
        Vector2D Orientation { get; }
        
        ButtonInput Run { get; }
        ButtonInput Jump { get; }
        ButtonInput Pulse { get; }
        ButtonInput Interact { get; }
        ButtonInput Start { get; }

        bool IsBeingUsed { get; }
        string IconSpritesheet { get; }
        
        ButtonInput Debug { get; }
        Vector2D DebugMovement { get; }
        ButtonInput Quicksave { get; }
        ButtonInput Quickload { get; }

        ButtonInput Pause { get; }
        ButtonInput Back { get; }

        void SetVibration(float amount);
        void ProcessInput();
    }
}
