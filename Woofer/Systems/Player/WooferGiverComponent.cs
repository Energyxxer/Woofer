using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Saves;

namespace WooferGame.Systems.Player
{
    [Component("woofer_giver")]
    class WooferGiverComponent : Component
    {
    }
    [Component("woofer_upgrade")]
    class WooferUpgradeComponent : Component
    {
        [PersistentProperty]
        public int Energy = 160;
    }
    [Component("health_upgrade")]
    class HealthUpgradeComponent : Component
    {
        [PersistentProperty]
        public int Health = 6;
    }
}
