using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;
using WooferGame.Systems.Interaction;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Sounds;
using WooferGame.Systems.Timer;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Pulse
{
    class PulseEmitter : Entity
    {
        public PulseEmitter(Vector2D pos, Vector2D direction, double strength, double reach, bool solid = true)
        {
            this.Components.Add(new Spatial(pos));
            if (solid)
            {
                this.Components.Add(new Physical() { GravityMultiplier = 0 });
                this.Components.Add(new RigidBody(new CollisionBox(-8, -8, 16, 16)));
            }

            this.Components.Add(new Renderable(new Sprite("lab_objects", new Rectangle(-8, -8, 16, 16), new Rectangle(0, 352, 16, 16))));
            this.Components.Add(new LevelRenderable());
            this.Components.Add(new PulseEmitterComponent(direction, strength, reach));
            this.Components.Add(new SoundEmitter(new Sounds.Sound("pulse_low")));

        }
    }
}
