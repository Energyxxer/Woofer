using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;

namespace WooferGame.Systems.Interaction
{
    [ComponentSystem("activation_system", ProcessingCycles.None),
        Listening(typeof(ActivationEvent))]
    class ActivationSystem : ComponentSystem
    {
        public override void EventFired(object sender, Event evt)
        {
            if(evt is ActivationEvent e && e.Affected.Components.Has<LinkedActivationComponent>())
            {
                LinkedActivationComponent links = e.Affected.Components.Get<LinkedActivationComponent>();
                if (links.Enabled)
                {
                    foreach (long id in links.EntitiesToActivate)
                    {
                        if (id != links.Owner.Id && Owner.Entities.ContainsId(id))
                        {
                            Owner.Events.InvokeEvent(new ActivationEvent(links, Owner.Entities[id], e));
                        }
                    }
                    if(links.OneTime)
                    {
                        links.Enabled = false;
                    }
                }
            }
        }
    }
}
