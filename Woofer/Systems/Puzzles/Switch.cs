using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Puzzles
{
    class Switch : Entity
    {
        public Switch(Vector2D pos, Rectangle bounds)
        {
            this.Components.Add(new Spatial(pos));
            this.Components.Add(new Physical() { GravityMultiplier = 0 });
            this.Components.Add(new SoftBody(new CollisionBox(bounds), 0) { Movable = false  });
            this.Components.Add(new Renderable(new Sprite("switch", bounds)));
            this.Components.Add(new LevelRenderable());

            this.Components.Add(new SwitchComponent());
        }
    }
}
