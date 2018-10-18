using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;
using WooferGame.Systems.Interaction;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Pulse;

namespace WooferGame.Systems.Sailboat
{
    [ComponentSystem("sailboat_system"),
        Listening(typeof(ActivationEvent))]
    class SailboatSystem : ComponentSystem
    {
        public override void EventFired(object sender, Event evt)
        {
            if(evt is ActivationEvent e &&
                e.Affected.Components.Has<SailboatComponent>() &&
                e.Affected.Components.Get<Physical>() is Physical phys)
            {
                Vector2D direction = (e.InnerEvent as PulseEvent).Direction;
                direction.Y = 0;
                phys.Velocity += direction * 16;
            }
        }
    }
}
