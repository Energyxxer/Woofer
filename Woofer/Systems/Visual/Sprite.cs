using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Interfaces.Visuals;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Meta.LevelEditor;

namespace WooferGame.Systems.Visual
{
    [PersistentObject]
    class Sprite
    {
        public const byte Mod_None = 0;
        public const byte Mod_InputType = 1;

        [PersistentProperty("texture")]
        public string _texture;
        public string Texture
        {
            get
            {
                if ((Modifiers & Mod_InputType) != 0) return Woofer.Controller.InputManager.ActiveInputMap.IconSpritesheet;
                return _texture;
            }
            set => _texture = value;
        }
        [PersistentProperty]
        public Rectangle Destination;
        [PersistentProperty]
        public byte Modifiers = Mod_None;
        [PersistentProperty]
        public Rectangle Source;

        [PersistentProperty]
        public DrawMode DrawMode = DrawMode.Normal;

        [HideInInspector]
        private float _opacity = 1f;

        [PersistentProperty]
        public float Opacity {
            get => _opacity;
            set => _opacity = value;
        }

        [PersistentProperty]
        public float ViewOrder { get; set; } = 0;

        public Sprite()
        {
        }

        public Sprite(string texture, Rectangle destination) : this(texture, destination, Rectangle.Empty)
        {
        }

        public Sprite(string texture, Rectangle destination, Rectangle source)
        {
            Texture = texture;
            Destination = destination;
            Source = source;
        }

        public Sprite Clone() => new Sprite() {
            _texture = _texture,
            Destination = new Rectangle(Destination),
            Modifiers = Modifiers,
            Source = new Rectangle(Source),
            DrawMode = DrawMode,
            _opacity = _opacity,
            ViewOrder = ViewOrder
        };
    }
}
