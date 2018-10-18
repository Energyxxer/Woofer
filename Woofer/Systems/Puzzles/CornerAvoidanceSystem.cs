using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;
using WooferGame.Systems.Physics;

namespace WooferGame.Systems.Puzzles
{
    [ComponentSystem("corner_avoidance"),
        Listening(typeof(RigidCollisionEvent))]
    class CornerAvoidanceSystem : ComponentSystem
    {
        public override void EventFired(object sender, Event evt)
        {
            if(evt is RigidCollisionEvent ce && ce.Victim.Components.Has<AvoidCorners>())
            {
                AvoidCorners ac = ce.Victim.Components.Get<AvoidCorners>();
                Physical phys = ce.Victim.Components.Get<Physical>();
                SoftBody sb = ce.Victim.Components.Get<SoftBody>();

                if(ce.Normal.Y != 0)
                {
                    if(ac.Bouncing)
                    {
                        sb.Mass = ac.RecordedMass;
                    }
                    ac.Bouncing = false;
                } else
                {
                    ac.RecordedMass = sb.Mass;
                    ac.Bouncing = true;
                    phys.Velocity = new Vector2D(32 * ce.Normal.X, 32);
                    sb.Mass = 0;
                }
            }
        }
    }
}
