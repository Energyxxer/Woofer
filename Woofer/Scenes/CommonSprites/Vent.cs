using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Util;
using WooferGame.Systems.Visual;

namespace WooferGame.Scenes.CommonSprites
{
    class Vent : Sprite
    {
        public Vent(Vector2D offset, HorizontalDirection direction) : base("lab_objects", new Rectangle(offset, new Size(32, 16)), new Rectangle(direction == HorizontalDirection.Left ? 32 : 0, 96, 32, 16))
        {
        }
        public Vent(HorizontalDirection direction) : base("lab_objects", new Rectangle(Vector2D.Empty, new Size(32, 16)), new Rectangle(direction == HorizontalDirection.Left ? 32 : 0, 96, 32, 16))
        {
        }
    }
}
