using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Interfaces.Input.Mouse;
using GameInterfaces.Input.GamePad;
using GameInterfaces.Input.Keyboard;

namespace GameInterfaces.Controller
{
    public interface IInputUnit
    {
        IKeyboard Keyboard { get; }
        IMouse Mouse { get; }
        IGamePadCollection GamePads { get; }
    }
}
