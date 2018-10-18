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
using EntityComponentSystem.Util;

namespace WooferGame.Systems.HealthSystems
{
    [Component("health")]
    class Health : Component
    {
        [PersistentProperty]
        public int MaxHealth = 4;
        [PersistentProperty]
        public int CurrentHealth = 4;
        [PersistentProperty]
        public double RegenRate = 5;
        [PersistentProperty]
        public double RegenCooldown = 0;

        [PersistentProperty]
        public bool HealthBarVisible = false;

        [PersistentProperty]
        public bool DoShowHealth = true;

        [PersistentProperty]
        public double InvincibilityTimer = 0;

        [PersistentProperty]
        public double InvincibilityFrames = 40;

        [PersistentProperty]
        public Vector2D HealthBarOffset = new Vector2D(0, 16);

        [PersistentProperty]
        public int DeathTime = 40;
        
        [PersistentProperty]
        public bool RemoveOnDeath = true;
    }
}
