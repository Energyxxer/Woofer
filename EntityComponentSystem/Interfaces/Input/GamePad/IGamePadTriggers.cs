using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterfaces.Input.GamePad
{
    public interface IGamePadTriggers
    {
        float Left { get; }
        float Right { get; }
    }
}
