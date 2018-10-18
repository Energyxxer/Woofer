using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using WooferGame.Systems.Interaction;
using WooferGame.Systems.Physics;

namespace WooferGame.Systems.Setters
{
    [ComponentSystem("setters"),
        Listening(typeof(ActivationEvent))]
    class SetterSystem : ComponentSystem
    {
        public override void EventFired(object sender, Event e)
        {
            if(e is ActivationEvent ae)
            {
                if (ae.Affected.Components.Get<PositionSetter>() is PositionSetter posSet)
                {
                    Entity toAffect;
                    if (posSet.ChangedId == 0) toAffect = ae.Affected;
                    else toAffect = Owner.Entities[posSet.ChangedId];

                    if (toAffect != null && toAffect.Components.Get<Spatial>() is Spatial sp)
                    {
                        if (posSet.Add) sp.Position += posSet.Amount;
                        else sp.Position = posSet.Amount;
                    }
                }
                if (ae.Affected.Components.Get<VelocitySetter>() is VelocitySetter velSet)
                {
                    Entity toAffect;
                    if (velSet.ChangedId == 0) toAffect = ae.Affected;
                    else toAffect = Owner.Entities[velSet.ChangedId];

                    if (toAffect != null && toAffect.Components.Get<Physical>() is Physical phys)
                    {
                        if (velSet.Add) phys.Velocity += velSet.Amount;
                        else phys.Velocity = velSet.Amount;
                    }
                }
                if (ae.Affected.Components.Get<ActiveSetter>() is ActiveSetter actSet)
                {
                    Entity toAffect;
                    if (actSet.ChangedId == 0) toAffect = ae.Affected;
                    else toAffect = Owner.Entities[actSet.ChangedId];

                    if (toAffect != null)
                    {
                        toAffect.Active = actSet.Active;
                    }
                }
            }
        }
    }
}
