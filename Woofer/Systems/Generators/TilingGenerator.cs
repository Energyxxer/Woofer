using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using EntityComponentSystem.Util;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Generators
{
    [Component("tiling_generator")]
    class TilingGenerator : Component
    {
        [PersistentProperty]
        public Rectangle MaxBounds = new Rectangle();
        [PersistentProperty]
        public Rectangle TileBounds = new Rectangle();
        [PersistentProperty]
        public List<Sprite> Sprites = new List<Sprite>();
        [PersistentProperty]
        public bool Preview { get; set; } = false;
        public bool Generate { get; set; } = false;
    }
}
