using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;

namespace WooferGame.Systems.HealthSystems
{
    [Event("death_event")]
    class DeathEvent : Event
    {
        public Entity Affected;

        public DeathEvent(Entity affected, Component sender) : base(sender)
        {
            Affected = affected;
        }
    }
}
