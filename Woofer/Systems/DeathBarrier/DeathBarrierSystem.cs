using System;
using System.Collections.Generic;
using System.Linq;

using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;

using WooferGame.Systems.Checkpoints;

namespace WooferGame.Systems.DeathBarrier
{
    [ComponentSystem("death_barrier", ProcessingCycles.Update),
        Watching(typeof(DeathBarrierComponent), typeof(RemoveOnBarrierComponent), typeof(CheckpointOnBarrierComponent))]
    class DeathBarrierSystem : ComponentSystem
    {

        private List<Type> typeOrder = new List<Type>()
        {
            typeof(DeathBarrierComponent),
            typeof(RemoveOnBarrierComponent),
            typeof(CheckpointOnBarrierComponent)
        };

        public override void Update()
        {
            DeathBarrierComponent barrier = null;

            foreach(Component component in WatchedComponents.OrderBy(a => typeOrder.IndexOf(a.GetType())))
            {
                if(component is DeathBarrierComponent)
                {
                    barrier = component as DeathBarrierComponent;
                } else if(component is RemoveOnBarrierComponent)
                {
                    if(component.Owner.Components.Get<Spatial>() is Spatial sp)
                    {
                        if (sp.Position.Y < (barrier?.Y ?? 0))
                        {
                            component.Owner.Remove();
                        }
                    }
                } else if(component is CheckpointOnBarrierComponent)
                {
                    if(component.Owner.Components.Get<Spatial>() is Spatial sp)
                    {
                        if(sp.Position.Y < (barrier?.Y ?? 0))
                        {
                            Owner.Events.InvokeEvent(new CheckpointRequestEvent(barrier, component.Owner));
                        }
                    }
                }
            }
        }
    }
}
