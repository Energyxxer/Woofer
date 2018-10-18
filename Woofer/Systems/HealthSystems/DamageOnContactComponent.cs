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
    [Component("damage_on_contact")]
    class DamageOnContactComponent : Component
    {
        [PersistentProperty]
        public int Damage = 1;

        [PersistentProperty]
        public DamageFilter Filter = DamageFilter.DamageAllies;

        [PersistentProperty]
        public bool Remove = false;

        [PersistentProperty]
        public float Knockback = 1;
    }

    enum DamageFilter
    {
        DamageAll, DamageAllies, DamageEnemies
    }
}
