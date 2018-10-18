using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Parallax
{
    class ParallaxObject : Entity
    {
        public ParallaxObject(Vector2D pos, Rectangle sourceBounds, Vector2D speed, double scale = 1)
        {
            this.Components.Add(new Spatial(pos));
            this.Components.Add(new Parallax("parallax_bg", sourceBounds, speed, scale));
        }
    }
}
