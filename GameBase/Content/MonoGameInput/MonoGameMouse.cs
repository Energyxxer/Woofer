using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Interfaces.Input.Mouse;
using EntityComponentSystem.Util;
using GameInterfaces.Input;
using Mouse = Microsoft.Xna.Framework.Input.Mouse;

namespace GameBase.MonoGameInput
{
    public class MonoGameMouse : IMouse
    {
        public Vector2D Position {
            get {
                Microsoft.Xna.Framework.Point xnaPos = Mouse.GetState().Position;
                return new Vector2D(xnaPos.X, xnaPos.Y);
            }
            set => Mouse.SetPosition((int) value.X, (int) value.Y);
        }

        public int VerticalScrollWheelValue => Mouse.GetState().ScrollWheelValue;

        public int HorizontalScrollWheelValue => Mouse.GetState().ScrollWheelValue;

        public ButtonState LeftButton => Mouse.GetState().LeftButton.ToInterface();

        public ButtonState MiddleButton => Mouse.GetState().MiddleButton.ToInterface();

        public ButtonState RightButton => Mouse.GetState().RightButton.ToInterface();

        public bool IsBeingUsed => LeftButton.IsPressed() || MiddleButton.IsPressed() || RightButton.IsPressed();
    }
}
