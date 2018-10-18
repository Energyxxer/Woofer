using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterfaces.Input.GamePad
{
    public interface IGamePad
    {
        IGamePadButtons Buttons { get; }
        IGamePadThumbsticks Thumbsticks { get; }
        IGamePadTriggers Triggers { get; }
        IGamePadDPad DPad { get; }

        bool IsBeingUsed { get; }

        void SetVibration(float left, float right);
    }
}
