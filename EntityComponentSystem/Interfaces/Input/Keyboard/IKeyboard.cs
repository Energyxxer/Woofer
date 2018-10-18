using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterfaces.Input.Keyboard
{
    public interface IKeyboard
    {
        ButtonState this[Key key] { get; }
        bool IsBeingUsed { get; }
    }
}
