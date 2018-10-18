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

namespace WooferGame.Systems.Refill
{
    class EnergyRefillOrb : Entity
    {
        public EnergyRefillOrb(Vector2D pos)
        {
            this.Components.Add(new Spatial(pos));
            this.Components.Add(new Physical() { GravityMultiplier = 0 });
            this.Components.Add(new SoftBody(new CollisionBox(-8, -8, 16, 16), 0) { Movable = false });
            this.Components.Add(new Renderable(new Sprite("grass", new Rectangle(-8, -8, 16, 16))));
            this.Components.Add(new LevelRenderable());

            this.Components.Add(new EnergyRefillComponent());
        }
    }
}
