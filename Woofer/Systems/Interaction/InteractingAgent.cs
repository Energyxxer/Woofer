using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;
using WooferGame.Input;

namespace WooferGame.Systems.Interaction
{
    [Component("interacting_agent")]
    class InteractingAgent : Component
    {
        [PersistentProperty]
        public double MaxDistance { get; set; }

        public InteractingAgent() : this(32)
        {
        }

        public InteractingAgent(double maxDistance)
        {
            MaxDistance = maxDistance;
        }
    }
}
