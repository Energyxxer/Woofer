using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Util;
using GameInterfaces.Input;

namespace GameBase.MonoGameInput
{
    static class MonoGameInputExtensions
    {
        public static ButtonState ToInterface(this Microsoft.Xna.Framework.Input.ButtonState state)
        {
            return state == Microsoft.Xna.Framework.Input.ButtonState.Pressed ? ButtonState.Pressed : ButtonState.Released;
        }
        public static Vector2D ToInterface(this Microsoft.Xna.Framework.Vector2 vec)
        {
            return new Vector2D(vec.X, vec.Y);
        }
    }
}
