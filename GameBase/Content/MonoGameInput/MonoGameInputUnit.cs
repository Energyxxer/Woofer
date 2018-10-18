using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Interfaces.Input.Mouse;
using GameInterfaces.Controller;
using GameInterfaces.Input.GamePad;
using GameInterfaces.Input.Keyboard;

namespace GameBase.MonoGameInput
{
    public class MonoGameInputUnit : IInputUnit
    {
        public IKeyboard Keyboard { get; private set; } = new MonoGameKeyboard();
        public IMouse Mouse { get; private set; } = new MonoGameMouse();
        public IGamePadCollection GamePads { get; private set; } = new MonoGameGamePadCollection();
    }
}
