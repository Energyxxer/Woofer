using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Events;

namespace WooferGame.Systems.Interaction
{
    [Event("interaction_range_leave")]
    class InteractionRangeExit : Event
    {
        public InteractionRangeExit(Component sender) : base(sender)
        {
        }
    }
}
