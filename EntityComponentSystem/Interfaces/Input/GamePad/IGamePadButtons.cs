using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterfaces.Input.GamePad
{
    public interface IGamePadButtons
    {
        ButtonState A { get; }
        ButtonState B { get; }
        ButtonState X { get; }
        ButtonState Y { get; }
        ButtonState Start { get; }
        ButtonState Back { get; }
        ButtonState Home { get; }
        ButtonState LeftBumper { get; }
        ButtonState RightBumper { get; }
        ButtonState LeftStick { get; }
        ButtonState RightStick { get; }
    }
}
