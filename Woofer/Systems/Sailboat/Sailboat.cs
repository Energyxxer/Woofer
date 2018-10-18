using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Pulse;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Sailboat
{
    class Sailboat : Entity
    {
        public Sailboat(Vector2D pos)
        {
            this.Components.Add(new Spatial(pos));
            this.Components.Add(new Physical() { GravityMultiplier = 0 });
            this.Components.Add(new RigidBody(
                new CollisionBox(-32, -6, 64, 6),
                new CollisionBox(-32, 0, 6, 6),
                new CollisionBox(32-6, 0, 6, 6)
                ));
            this.Components.Add(new Renderable(
                new Sprite("brick", new Rectangle(-32, -6, 64, 6)),
                new Sprite("brick", new Rectangle(-32, 0, 6, 6)),
                new Sprite("brick", new Rectangle(32-6, 0, 6, 6)),
                new Sprite("grass", new Rectangle(-1, 0, 2, 32))
                ));
            this.Components.Add(new LevelRenderable());

            this.Components.Add(new PulseReceiver() { Offset = new Vector2D(0, 24) });
            this.Components.Add(new SailboatComponent());
        }
    }
}
