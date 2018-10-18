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
using GameInterfaces.Controller;
using WooferGame.Systems.Interaction;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Player;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.HealthSystems
{
    [ComponentSystem("health_system", ProcessingCycles.Update | ProcessingCycles.Render),
        Watching(typeof(Health)),
        Listening(typeof(DamageEvent), typeof(SoftCollisionEvent), typeof(RigidCollisionEvent))]
    class HealthSystem : ComponentSystem
    {
        public override void Update()
        {
            foreach (Health health in WatchedComponents)
            {
                if (!health.Owner.Active) continue;
                if (health.InvincibilityTimer > 0) health.InvincibilityTimer--;
                if (health.CurrentHealth <= 0)
                {
                    health.DeathTime--;
                    if(health.DeathTime <= 0)
                    {
                        if(health.Owner.Components.Get<OnDeathTrigger>() is OnDeathTrigger onDeath)
                        {
                            foreach(long id in onDeath.EntitiesToActivate)
                            {
                                Entity entity = Owner.Entities[id];
                                if(entity != null)
                                {
                                    Owner.Events.InvokeEvent(new ActivationEvent(health, entity, null));
                                }
                            }
                        }
                        Owner.Events.InvokeEvent(new DeathEvent(health.Owner, health));
                        if(health.RemoveOnDeath) health.Owner.Remove();
                    }
                    continue;
                }
                if (health.RegenCooldown > 0)
                {
                    health.RegenCooldown -= Owner.DeltaTime;
                    if(health.RegenCooldown <= 0)
                    {
                        if (health.CurrentHealth >= health.MaxHealth) health.HealthBarVisible = false;

                        if (health.CurrentHealth < health.MaxHealth) health.CurrentHealth++;
                        health.RegenCooldown = health.RegenRate;
                    }
                }
            }
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            var layer = r.GetLayerGraphics("hi_res_overlay");

            foreach(Health health in WatchedComponents)
            {
                if (!health.Owner.Active) continue;
                if (!health.DoShowHealth) continue;
                if (!health.HealthBarVisible) continue;
                Spatial sp = health.Owner.Components.Get<Spatial>();

                int width = 50;
                int height = 5;

                if(sp != null)
                {
                    System.Drawing.Point destination = Renderable.ToScreenCoordinates(sp.Position + health.HealthBarOffset, layer.GetSize());
                    layer.FillRect(new System.Drawing.Rectangle(destination.X - width / 2, destination.Y + height / 2, width, height), System.Drawing.Color.FromArgb(201, 48, 56));
                    layer.FillRect(new System.Drawing.Rectangle(destination.X - width / 2, destination.Y + height / 2, width*health.CurrentHealth/health.MaxHealth, height), System.Drawing.Color.FromArgb(81, 196, 63));
                }
            }
        }

        public override void EventFired(object sender, Event e)
        {
            if(e is DamageEvent de && de.Affected.Components.Get<Health>() is Health health)
            {
                if(health.InvincibilityTimer <= 0)
                {
                    health.CurrentHealth -= de.Amount;
                    health.HealthBarVisible = true;
                    health.InvincibilityTimer = health.InvincibilityFrames;
                    health.RegenCooldown = health.RegenRate;
                    if(de.Sender.Owner.Components.Get<Spatial>() is Spatial origin && health.Owner.Components.Get<Spatial>() is Spatial target && health.Owner.Components.Get<Physical>() is Physical phys)
                    {
                        phys.Velocity = new Vector2D(96 * Math.Sign(target.Position.X - origin.Position.X), 96) * de.Knockback;
                    }
                    if(health.CurrentHealth <= 0)
                    {
                        health.DeathTime = 80;
                    }
                }
            }
            else if (e is SoftCollisionEvent sce && sce.Sender.Owner.Components.Get<DamageOnContactComponent>() is DamageOnContactComponent sHurt && sce.Victim.Components.Get<Health>() is Health sVictim)
            {
                if ((sHurt.Owner.Components.Get<Health>()?.CurrentHealth ?? 1) <= 0) return;
                if (sVictim.CurrentHealth <= 0) return;
                if (sHurt.Filter == DamageFilter.DamageAll ||
                    (sHurt.Filter == DamageFilter.DamageAllies && sVictim.Owner.Components.Has<PlayerComponent>()) ||
                    (sHurt.Filter == DamageFilter.DamageEnemies && !sVictim.Owner.Components.Has<PlayerComponent>())
                )
                {
                    Owner.Events.InvokeEvent(new DamageEvent(sVictim.Owner, sHurt.Damage, sHurt) { Knockback = sHurt.Knockback });
                    if(sHurt.Remove)
                    {
                        sHurt.Owner.Remove();
                    }
                }
            }
            else if (e is RigidCollisionEvent rce && rce.Sender.Owner.Components.Get<DamageOnContactComponent>() is DamageOnContactComponent rHurt && rce.Victim.Components.Get<Health>() is Health rVictim)
            {
                if ((rHurt.Owner.Components.Get<Health>()?.CurrentHealth ?? 1) <= 0) return;
                if (rVictim.CurrentHealth <= 0) return;
                if (rHurt.Filter == DamageFilter.DamageAll ||
                    (rHurt.Filter == DamageFilter.DamageAllies && rVictim.Owner.Components.Has<PlayerComponent>()) ||
                    (rHurt.Filter == DamageFilter.DamageEnemies && !rVictim.Owner.Components.Has<PlayerComponent>())
                )
                {
                    Owner.Events.InvokeEvent(new DamageEvent(rVictim.Owner, rHurt.Damage, rHurt) { Knockback = rHurt.Knockback });
                    if (rHurt.Remove)
                    {
                        rHurt.Owner.Remove();
                    }
                }
            }
        }
    }
}
