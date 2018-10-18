using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Timer
{
    [Component("timer")]
    class TimerComponent : Component
    {
        [PersistentProperty]
        public double TimeElapsed = 0;
        [PersistentProperty]
        public double Period;

        public TimerComponent() : this(1)
        {
        }

        public TimerComponent(double period)
        {
            this.Period = period;
        }
    }
}
