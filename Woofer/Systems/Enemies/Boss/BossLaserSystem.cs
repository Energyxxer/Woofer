using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;
using WooferGame.Systems.HealthSystems;
using WooferGame.Systems.Interaction;
using WooferGame.Systems.Timer;
using WooferGame.Systems.Visual.Animation;

namespace WooferGame.Systems.Enemies.Boss
{
    [ComponentSystem("boss_laser_system", ProcessingCycles.None),
        Watching(typeof(BossLaserComponent)),
        Listening(typeof(ActivationEvent))]
    class BossLaserSystem : ComponentSystem
    {
        public override void EventFired(object sender, Event e)
        {
            if(e is ActivationEvent ae && ae.Affected.GetComponent<BossLaserComponent>() is BossLaserComponent laser)
            {
                if(laser.Owner.HasComponent<DamageOnContactComponent>())
                {
                    laser.Owner.Remove();
                } else
                {
                    var timer = laser.Owner.GetComponent<TimerComponent>();
                    if (timer != null) timer.Period = 3;
                    laser.Owner.Components.Remove<AnimationComponent>();
                    laser.Owner.Components.Add(new AnimationComponent(new AnimatedSprite(0, new Rectangle(176+(laser.Big ? 88 : 48), laser.Big ? 180 : 0, laser.Big ? 88 : 48, laser.Big ? 212 : 180), new Vector2D(laser.Big ? 88 : 48, 0), 2, 4) { Loop = true }));
                    laser.Owner.Components.Add(new DamageOnContactComponent() { Damage = laser.Big ? 2 : 1, Filter = DamageFilter.DamageAll, Knockback = 1 });
                }
            }
        }
    }
}
