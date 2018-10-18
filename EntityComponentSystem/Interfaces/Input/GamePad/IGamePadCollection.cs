using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterfaces.Input.GamePad
{
    public interface IGamePadCollection
    {
        IGamePad this[int index] { get; }
    }
}
