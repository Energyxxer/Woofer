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

namespace WooferGame.Systems.Enemies.Boss
{
    [Component("boss_projectile_catcher")]
    class BossProjectileCatcher : Component
    {
        [PersistentProperty]
        public long BossId { get; set; } = 0;
    }
}
