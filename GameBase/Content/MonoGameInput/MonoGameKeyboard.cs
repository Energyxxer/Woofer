using System;

using GameInterfaces.Input;
using GameInterfaces.Input.Keyboard;

namespace GameBase.MonoGameInput
{
    public class MonoGameKeyboard : IKeyboard
    {
        public ButtonState this[Key key] => Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(MapKey(key)) ? ButtonState.Pressed : ButtonState.Released;

        public bool IsBeingUsed => Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeys().Length > 0;

        private Microsoft.Xna.Framework.Input.Keys MapKey(Key key)
        {
            return (Microsoft.Xna.Framework.Input.Keys) Enum.Parse(typeof(Microsoft.Xna.Framework.Input.Keys), key.ToString());
        }
    }
}
