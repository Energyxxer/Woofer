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
    [Event("DamageEvent")]
    class DamageEvent : Event
    {
        public Entity Affected;
        public int Amount;
        public float Knockback = 1;

        public DamageEvent(Entity affected, int amount, Component sender) : base(sender)
        {
            Affected = affected;
            Amount = amount;
        }
    }
}
