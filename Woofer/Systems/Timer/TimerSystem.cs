using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.ComponentSystems;
using WooferGame.Systems.Interaction;

namespace WooferGame.Systems.Timer
{
    [ComponentSystem("timer", ProcessingCycles.Update),
        Watching(typeof(TimerComponent))]
    class TimerSystem : ComponentSystem
    {
        public override void Update()
        {
            foreach(TimerComponent timer in WatchedComponents)
            {
                if (!timer.Owner.Active) continue;
                timer.TimeElapsed += Owner.DeltaTime;
                while(timer.TimeElapsed >= timer.Period)
                {
                    timer.TimeElapsed -= timer.Period;
                    Owner.Events.InvokeEvent(new ActivationEvent(timer, timer.Owner, null));
                }
            }
        }
    }
}
