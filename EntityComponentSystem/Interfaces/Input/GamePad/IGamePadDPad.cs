using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterfaces.Input.GamePad
{
    public interface IGamePadDPad
    {
        ButtonState Up { get; }
        ButtonState Down { get; }
        ButtonState Left { get; }
        ButtonState Right { get; }
    }
}
