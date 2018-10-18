using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Util;
using GameInterfaces.Input;

namespace EntityComponentSystem.Interfaces.Input.Mouse
{
    public interface IMouse
    {
        Vector2D Position { get; set; }
        int VerticalScrollWheelValue { get; }
        int HorizontalScrollWheelValue { get; }

        ButtonState LeftButton { get; }
        ButtonState MiddleButton { get; }
        ButtonState RightButton { get; }
        
        bool IsBeingUsed { get; }
    }
}
