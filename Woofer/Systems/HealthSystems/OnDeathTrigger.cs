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

namespace WooferGame.Systems.HealthSystems
{
    [Component("on_death_trigger")]
    class OnDeathTrigger : Component
    {
        [PersistentProperty]
        public List<long> EntitiesToActivate = new List<long>();
    }
}
