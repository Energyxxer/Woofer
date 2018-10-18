using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Parallax
{
    [Component("parallax")]
    class Parallax : Component
    {
        [PersistentProperty]
        public string Texture { get; set; }
        [PersistentProperty]
        public Rectangle SourceBounds { get; set; }
        [PersistentProperty]
        public Vector2D Speed { get; set; }
        [PersistentProperty]
        public double Scale { get; set; }

        public Parallax()
        {
        }

        public Parallax(string texture, Rectangle sourceBounds, Vector2D speed, double scale)
        {
            Texture = texture;
            SourceBounds = sourceBounds;
            Speed = speed;
            Scale = scale;
        }
    }
}
