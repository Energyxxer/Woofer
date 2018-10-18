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
using WooferGame.Controller.Commands;
using WooferGame.Scenes;
using WooferGame.Systems.Camera.Shake;
using WooferGame.Systems.Interaction;
using WooferGame.Systems.Linking;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Player;
using WooferGame.Systems.Timer;
using WooferGame.Systems.Visual;
using WooferGame.Systems.Visual.Particles;

namespace WooferGame.Systems.Enemies.Boss
{
    [ComponentSystem("BossSystem", ProcessingCycles.Update),
        Watching(typeof(PlayerComponent), typeof(Boss), typeof(BossProjectile)),
        Listening(typeof(ActivationEvent), typeof(SoftCollisionEvent))]
    class BossSystem : ComponentSystem
    {
        public override void Update()
        {
            Entity player = WatchedComponents.OfType<PlayerComponent>().FirstOrDefault()?.Owner;
            foreach(Boss boss in WatchedComponents.OfType<Boss>())
            {
                if (boss.Health <= 0) continue;
                Spatial sp = boss.Owner.GetComponent<Spatial>();
                Physical phys = boss.Owner.GetComponent<Physical>();

                Spatial playerSp = player?.GetComponent<Spatial>();
                if (sp == null || phys == null) continue;

                phys.GravityMultiplier = 1;
                phys.Velocity = new Vector2D(0, 362) * Owner.DeltaTime;

                Vector2D? flyTo = null;
                float flySpeed = 1;

                bool showParticles = true;

                boss.Difficulty = boss.Health > 20 ? 0 : 1;

                if (boss.State == Boss.Circling)
                {
                    if (boss.Transitioning)
                    {
                        if ((sp.Position - new Vector2D(0, boss.Difficulty >= 1 ? 192 : 128)).Magnitude < 1)
                        {
                            boss.Transitioning = false;
                            boss.StateData = 0;
                        }
                        else
                        {
                            flyTo = new Vector2D(0, boss.Difficulty >= 1 ? 192 : 128);
                            flySpeed = 2;
                        }
                    }
                    if (!boss.Transitioning)
                    {
                        boss.StateData += Owner.DeltaTime;
                        if (boss.StateData > 16)
                        {
                            boss.ChangeState(Owner.Random.Next(4) == 0 ? Boss.Drop : Boss.Laser);
                        }
                        else
                        {
                            phys.Velocity += new Vector2D(125.6 * Math.Cos(Math.PI * boss.StateData / 4), 0);
                            Vector2D hoverVelocity = new Vector2D(0, 48 * Math.Cos(Math.PI * boss.StateData));
                            phys.Velocity += hoverVelocity;
                            
                            if(boss.Difficulty >= 1 && Math.Round(boss.StateData, 3)%4 == 0)
                            {
                                Owner.Entities.Add(new BossProjectileEntity(sp.Position + new Vector2D(0, -48)));
                            }
                        }
                    }
                }
                else if (boss.State == Boss.Laser)
                {
                    if (boss.Transitioning)
                    {
                        if ((sp.Position - new Vector2D(boss.Difficulty >= 1 ? 160 : 0, 256)).Magnitude < 1)
                        {
                            boss.Transitioning = false;
                            boss.StateData = 0;
                        }
                        else
                        {
                            flyTo = new Vector2D(boss.Difficulty >= 1 ? 160 : 0, 256);
                            flySpeed = 2;
                        }
                    }
                    if (!boss.Transitioning)
                    {
                        boss.StateData += Owner.DeltaTime;
                        if (boss.Difficulty < 1 && boss.StateData < 1.5)
                        {
                            if (playerSp != null)
                            {
                                flyTo = new Vector2D(playerSp.Position.X, sp.Position.Y);
                            }
                        }
                        else
                        {
                            if (boss.FlameEntity != 0)
                            {
                                Entity entity = Owner.Entities[boss.FlameEntity];
                                if (entity != null) entity.Active = false;
                            }
                            var laser1 = new BossLaser(sp.Position + new Vector2D(0, -44), true);
                            var laser2 = new BossLaser(sp.Position + new Vector2D(-88, -76), false);
                            var laser3 = new BossLaser(sp.Position + new Vector2D(88, -76), false);
                            laser1.Components.Add(new FollowingComponent(boss.Owner.Id) { Offset = new Vector2D(0, -44) });
                            laser2.Components.Add(new FollowingComponent(boss.Owner.Id) { Offset = new Vector2D(-88, -76) });
                            laser3.Components.Add(new FollowingComponent(boss.Owner.Id) { Offset = new Vector2D(88, -76) });
                            Owner.Entities.Add(laser1);
                            Owner.Entities.Add(laser2);
                            Owner.Entities.Add(laser3);
                            boss.ChangeState(Boss.LaserIdle);
                            showParticles = false;
                        }
                    }
                }
                else if (boss.State == Boss.LaserIdle)
                {
                    boss.Transitioning = false;
                    boss.StateData += Owner.DeltaTime;
                    if (boss.StateData >= 5)
                    {
                        if (boss.FlameEntity != 0)
                        {
                            Entity entity = Owner.Entities[boss.FlameEntity];
                            if (entity != null) entity.Active = true;
                        }
                        boss.ChangeState(Boss.Circling);
                    }
                    else
                    {
                        if(boss.Difficulty >= 1)
                        {
                            phys.Velocity += new Vector2D(-64, 0);
                        }
                        showParticles = false;
                    }
                }
                else if (boss.State == Boss.Drop)
                {
                    if (boss.Transitioning)
                    {
                        if ((sp.Position - new Vector2D(-160, 256)).Magnitude < 1)
                        {
                            boss.Transitioning = false;
                            boss.StateData = 0;
                        }
                        else
                        {
                            flyTo = new Vector2D(-160, 256);
                            flySpeed = 2;
                        }
                    }
                    if (!boss.Transitioning)
                    {
                        if (sp.Position.X >= 160)
                        {
                            boss.ChangeState(Boss.Circling);
                        } else
                        {
                            boss.StateData += Owner.DeltaTime;
                            phys.Velocity += new Vector2D(96, 0);
                            if (boss.StateData >= 1)
                            {
                                boss.StateData = 0;
                                if(boss.SentrySpawnerEntity != 0)
                                {
                                    Entity spawner = Owner.Entities[boss.SentrySpawnerEntity];
                                    if(spawner != null)
                                    {
                                        Owner.Events.InvokeEvent(new ActivationEvent(boss, spawner, null));
                                    }
                                }
                            }
                        }
                    }
                }

                //Functions
                if(flyTo != null)
                {
                    phys.Velocity += flySpeed * (flyTo.Value - sp.Position);
                }

                //Particles
                if (showParticles && (boss.FlameParticleProgress == 0 || boss.FlameParticleProgress == 5 * 3 || boss.FlameParticleProgress == 5 * 5 || boss.FlameParticleProgress == 2 * 5 || boss.FlameParticleProgress == 7 * 5))
                {
                    int variant = boss.FlameParticleProgress == 0 ? 0 : boss.FlameParticleProgress == 3 * 5 ? 1 : boss.FlameParticleProgress == 5 * 5 ? 2 : boss.FlameParticleProgress == 2 * 5 ? 3 : 4;
                    var particle = new BossFlameParticle(sp.Position + phys.Velocity * Owner.DeltaTime + new Vector2D(0, -40), variant);
                    particle.GetComponent<Physical>().Velocity = phys.Velocity*0.35;
                    Owner.Entities.Add(particle);
                }
                boss.FlameParticleProgress++;
                if (boss.FlameParticleProgress >= 5 * 8) boss.FlameParticleProgress = 0;
            }
        }

        public override void EventFired(object sender, Event e)
        {
            if(e is ActivationEvent ae0 && ae0.Affected.GetComponent<BossProjectile>() is BossProjectile projectile)
            {
                if(!projectile.Deflected)
                {
                    LevelRenderable lr = projectile.Owner.GetComponent<LevelRenderable>();
                    if (lr != null) lr.ZOrder = 4;
                }
                projectile.Deflected = true;
            }
            if((e is ActivationEvent ae && ae.Affected.HasComponent<Boss>()) || (e is SoftCollisionEvent ce && ce.Sender.Owner.HasComponent<BossProjectile>() && ce.Victim.HasComponent<BossProjectileCatcher>()))
            {
                Boss boss = (e as ActivationEvent)?.Affected.GetComponent<Boss>() ?? Owner.Entities[(e as SoftCollisionEvent).Victim.GetComponent<BossProjectileCatcher>()?.BossId ?? 0].GetComponent<Boss>();
                if (boss == null) return;
                if(e is SoftCollisionEvent sce)
                {
                    if (!sce.Sender.Owner.GetComponent<BossProjectile>().Deflected) return;
                    sce.Sender.Owner.Remove();
                }
                int damage = e is SoftCollisionEvent ? 4 : 1;
                if(boss.Health >= 1)
                {
                    Owner.Events.InvokeEvent(new CameraShakeEvent(boss, 16));
                    boss.Health -= damage;
                    if(boss.Health == 20)
                    {
                        Owner.Events.InvokeEvent(new CameraShakeEvent(boss, 16));
                        Renderable renderable = boss.Owner.GetComponent<Renderable>();
                        if(renderable != null && renderable.Sprites.Count > 0)
                        {
                            renderable.Sprites[0].Source.Y = 160;
                        }
                        Entity hit = boss.GlassHitEntity != 0 ? Owner.Entities[boss.GlassHitEntity] : null;
                        if (hit != null) hit.Active = false;
                        Entity hit2 = boss.BareHeadEntity != 0 ? Owner.Entities[boss.BareHeadEntity] : null;
                        if (hit2 != null) hit2.Active = true;
                    }

                    if (boss.Health <= 0) KillBoss(boss);
                }
            }
            if(e is ActivationEvent ae2 && ae2.Affected.HasComponent<GameEnd>())
            {
                Woofer.Controller.AudioUnit["pulse_high"].Play();
                Woofer.Controller.AudioUnit["pulse_high"].Play();
                Woofer.Controller.AudioUnit["pulse_high"].Play();
                Owner.QueueAction(() => Owner.Controller.CommandFired(new InternalSceneChangeCommand(new EndScene())));
            }
        }

        private void KillBoss(Boss boss)
        {
            Woofer.Controller.AudioUnit.StopMusic();

            Woofer.Controller.AudioUnit["boss_defeat"].Play();
            Owner.Events.InvokeEvent(new CameraShakeEvent(boss, new Vector2D(-32, 0)));

            Physical phys = boss.Owner.GetComponent<Physical>();
            RigidBody rb = boss.Owner.GetComponent<RigidBody>();
            if (phys == null || rb == null) return;
            SoftBody sb = new SoftBody(rb.UnionBounds, 2);
            Owner.QueueAction(() => boss.Owner.Components.Add(sb));
            phys.Velocity = new Vector2D(0, 32);

            KillPropeller(Owner.Entities[boss.LeftPropeller]);
            KillPropeller(Owner.Entities[boss.RightPropeller]);

            Entity hit2 = boss.BareHeadEntity != 0 ? Owner.Entities[boss.BareHeadEntity] : null;
            if (hit2 != null) hit2.Active = false;

            if (boss.FlameEntity != 0)
            {
                Entity entity = Owner.Entities[boss.FlameEntity];
                if (entity != null) entity.Active = false;
            }

            Renderable renderable = boss.Owner.GetComponent<Renderable>();
            if (renderable != null && renderable.Sprites.Count > 0)
            {
                renderable.Sprites[0].Source.Y = 272;
            }

            Entity next = boss.OnDefeat != 0 ? Owner.Entities[boss.OnDefeat] : null;
            if(next != null) Owner.Events.InvokeEvent(new ActivationEvent(boss, next, null));

            Entity end = new Entity();
            end.Components.Add(new GameEnd());
            end.Components.Add(new TimerComponent(8));
            Owner.Entities.Add(end);
        }

        private void KillPropeller(Entity propeller)
        {
            if (propeller == null) return;
            Physical phys = propeller.GetComponent<Physical>();
            RigidBody rb = propeller.GetComponent<RigidBody>();
            if (phys == null || rb == null) return;
            SoftBody sb = new SoftBody(rb.UnionBounds, 2);
            Owner.QueueAction(() => propeller.Components.Add(sb));
            phys.Velocity = new Vector2D(0, 32);
            phys.GravityMultiplier = 1;
            
            propeller.Components.Remove<FollowingComponent>();

            Renderable renderable = propeller.GetComponent<Renderable>();
            if (renderable != null && renderable.Sprites.Count > 1)
            {
                renderable.Sprites[1].Source.Y = 272;
            }
        }
    }
}
