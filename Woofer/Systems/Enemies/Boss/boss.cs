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

namespace WooferGame.Systems.Enemies.Boss
{
    [Component("boss")]
    class Boss : Component
    {
        public const int Circling = 1;
        public const int Laser = 2;
        public const int LaserIdle = 3;
        public const int Drop = 4;
        
        [PersistentProperty]
        public int State { get; set; } = Circling;
        [PersistentProperty]
        public bool Transitioning { get; set; } = true;

        [PersistentProperty]
        public double StateData { get; set; }
        public int FlameParticleProgress { get; internal set; }

        [PersistentProperty]
        public long FlameEntity { get; set; } = 0;
        [PersistentProperty]
        public long SentrySpawnerEntity { get; internal set; } = 0;
        [PersistentProperty]
        public long GlassHitEntity { get; internal set; } = 0;
        [PersistentProperty]
        public long BareHeadEntity { get; internal set; } = 0;

        [PersistentProperty]
        public long LeftPropeller { get; set; } = 0;
        [PersistentProperty]
        public long RightPropeller { get; set; } = 0;
        
        public int Difficulty { get; set; } = 0;

        [PersistentProperty]
        public int Health { get; set; } = 30;
        [PersistentProperty]
        public long OnDefeat { get; internal set; }

        public void ChangeState(int state)
        {
            State = state;
            Transitioning = true;
            StateData = 0;
        }
    }
}
