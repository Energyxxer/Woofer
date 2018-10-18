using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Util;

namespace GameInterfaces.Input.GamePad
{
    public interface IGamePadThumbsticks
    {
        Vector2D Left { get; }
        Vector2D Right { get; }
    }
}
