using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Refill
{
    [Component("energy_refill")]
    class EnergyRefillComponent : Component
    {
        [PersistentProperty]
        public bool Enabled = true;
    }
}
