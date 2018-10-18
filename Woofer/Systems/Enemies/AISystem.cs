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
using WooferGame.Systems.Physics;
using WooferGame.Systems.Player;

namespace WooferGame.Systems.Enemies
{
    [ComponentSystem("ai_system", ProcessingCycles.Input | ProcessingCycles.Update),
        Watching(typeof(PlayerComponent), typeof(SentryAI)),
        Listening(typeof(RigidCollisionEvent))]
    class AISystem : ComponentSystem
    {
        private static readonly Random Random = new Random();

        public override void Input()
        {
            foreach (SentryAI sentry in WatchedComponents.Where(c => c is SentryAI && c.Owner.Active))
            {
                sentry.OnGround = false;
            }
        }

        public override void Update()
        {
            Entity player = WatchedComponents.FirstOrDefault()?.Owner;
            if (player == null) return;
            Spatial playerSp = player.Components.Get<Spatial>();
            if (playerSp == null) return;

            foreach(SentryAI sentry in WatchedComponents.Where(c => c is SentryAI && c.Owner.Active))
            {
                if (sentry.ActionTime > 0) sentry.ActionTime--;
                if ((sentry.Owner.Components.Get<Health>()?.CurrentHealth ?? 1) <= 0) continue;
                Spatial sp = sentry.Owner.Components.Get<Spatial>();
                Physical phys = sentry.Owner.Components.Get<Physical>();
                if (sp == null || phys == null) continue;
                bool inRange = (sp.Position - playerSp.Position).Magnitude < sentry.FollowDistance;

                if (sentry.Action == SentryAction.Throw && sentry.ActionTime == 30)
                {
                    Vector2D velocity = new Vector2D();
                    double targetTime = 0.8;
                    velocity.X = (playerSp.Position.X - sp.Position.X) / targetTime;
                    velocity.Y = ((playerSp.Position.Y - sp.Position.Y - 4) + (362 * targetTime * targetTime) / 2) / targetTime;
                    velocity = velocity.Normalize() * Math.Min(velocity.Magnitude, 256);

                    Owner.Entities.Add(new Projectile(sentry.Owner, sp.Position+new Vector2D(0, 4), velocity));
                }

                if (!sentry.OnGround) continue;

                if(inRange && sentry.ActionTime == 0)
                {
                    //Choose action

                    double distance = (playerSp.Position - sp.Position).Magnitude;

                    int[] weights = { 1, 2, 3 };
                    if (distance > 64) weights[0] = 10;
                    else if (distance < 48) weights[1] = 6;
                    else weights[2] = 4;

                    SentryAction next;

                    int pick = Random.Next(weights[0] + weights[1] + weights[2]);
                    if (pick < weights[0]) next = SentryAction.Charge;
                    else if (pick < weights[0] + weights[1]) next = SentryAction.Dodge;
                    else next = SentryAction.Throw;
                    
                    sentry.Action = next;

                    switch(next)
                    {
                        case SentryAction.Charge:
                            {
                                phys.Velocity = new Vector2D(128 * Math.Sign(playerSp.Position.X - sp.Position.X), 64);
                                sentry.ActionTime = Random.Next(30, 60);
                                break;
                            }
                        case SentryAction.Dodge:
                            {
                                phys.Velocity = new Vector2D(-64 * Math.Sign(playerSp.Position.X - sp.Position.X), 64);
                                sentry.ActionTime = Random.Next(30, 60);
                                break;
                            }
                        case SentryAction.Throw:
                            {
                                sentry.ActionTime = 80;
                                break;
                            }
                    }
                }
            }
        }

        public override void EventFired(object sender, Event e)
        {
            if (e is RigidCollisionEvent ce)
            {
                if (ce.Victim.Components.Get<SentryAI>() is SentryAI ai && ce.Normal.Y > 0)
                {
                    ai.OnGround = true;
                }
            }
        }
    }
}
