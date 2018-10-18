using System;
using System.Collections.Generic;
using System.Linq;

using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Interfaces.Input;
using EntityComponentSystem.Util;
using GameInterfaces.Audio;
using GameInterfaces.Input;

using WooferGame.Input;
using WooferGame.Systems.Enemies;
using WooferGame.Systems.HealthSystems;
using WooferGame.Systems.Interaction;
using WooferGame.Systems.Movement;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Player;
using WooferGame.Systems.Player.Actions;
using WooferGame.Systems.Player.Animation;
using WooferGame.Systems.Visual.Particles;

namespace WooferGame.Systems.Pulse
{
    [ComponentSystem("pulse_system", 
        ProcessingCycles.Input | ProcessingCycles.Update),
        Watching(typeof(PulseAbility), typeof(PulsePushable), typeof(PulseReceiver)),
        Listening(typeof(PulseEvent), typeof(ActivationEvent))]
    class PulseSystem : ComponentSystem
    {
        public override void Input()
        {
            IInputMap inputMap = Woofer.Controller.InputManager.ActiveInputMap;

            foreach(PulseAbility pa in WatchedComponents.Where(c => c is PulseAbility))
            {
                ButtonInput pulseButton = inputMap.Pulse;

                if (pa.Owner.GetComponent<Health>() is Health health && health.CurrentHealth <= 0) continue;

                if(pa.EnergyMeter >= pa.PulseCost)
                {
                    if(pulseButton.Consume())
                    {
                        double strength = (pa.PulseStrength * Math.Sqrt(pa.EnergyMeter / pa.MaxEnergy));

                        if (pa.Owner.Components.Has<Physical>() && pa.Owner.Components.Has<PlayerOrientation>())
                        {
                            pa.Owner.Components.Get<Physical>().Velocity = pa.Owner.Components.Get<PlayerOrientation>().Unit * -strength;

                            ISoundEffect sound_low = Woofer.Controller.AudioUnit["pulse_low"];
                            sound_low.Pitch = (float)(strength / 256) - 1;
                            ISoundEffect sound_mid = Woofer.Controller.AudioUnit["pulse_mid"];
                            sound_mid.Pitch = sound_low.Pitch;
                            ISoundEffect sound_high = Woofer.Controller.AudioUnit["pulse_high"];
                            sound_high.Pitch = sound_low.Pitch;

                            sound_low.Volume = 0.6f;


                            sound_low.Play();
                            sound_mid.Play();
                            sound_high.Play();
                        }

                        pa.EnergyMeter -= pa.PulseCost;

                        if (pa.Owner.Components.Has<Spatial>()) {

                            Spatial sp = pa.Owner.Components.Get<Spatial>();
                            if (sp == null) continue;
                            PlayerOrientation po = pa.Owner.Components.Get<PlayerOrientation>();

                            Owner.Events.InvokeEvent(new PulseEvent(pa, sp.Position + pa.Offset, po != null ? po.Unit : new Vector2D(), strength, pa.MaxRange));
                        }
                    }
                }
            }
        }

        public override void Update()
        {
            foreach(PulseAbility pa in WatchedComponents.Where(c => c is PulseAbility))
            {
                if (pa.Owner.Components.Get<PlayerMovementComponent>().OnGround) pa.EnergyMeter = pa.MaxEnergy;
            }
        }

        public override void EventFired(object sender, Event evt)
        {
            PulseEmitterComponent pem;
            if (evt is PulseEvent pe)
            {
                foreach(PulsePushable pp in WatchedComponents.Where(c => c is PulsePushable))
                {
                    if (!pp.Owner.Active) continue;
                    if (pp.Owner == evt.Sender.Owner) continue;
                    if (!pp.Owner.Components.Has<Spatial>() || !pp.Owner.Components.Has<Physical>()) continue;
                    Physical ph = pp.Owner.Components.Get<Physical>();

                    Vector2D center = pp.Owner.Components.Has<SoftBody>() ? pp.Owner.Components.Get<SoftBody>().Bounds.Offset(ph.Position).Center : ph.Position;

                    double distance = (center - pe.Source).Magnitude;

                    if (distance > pe.Reach) continue;

                    if(pe.Direction.Magnitude == 0 || GeneralUtil.SubtractAngles((center - pe.Source).Angle, pe.Direction.Angle) <= Math.Atan(1/pe.Direction.Magnitude))
                    {
                        double mass = 1;
                        if (pp.Owner.Components.Has<SoftBody>()) mass = pp.Owner.Components.Get<SoftBody>().Mass;
                        double factor = 1-(distance / pe.Reach);
                        ph.Velocity += ((center - pe.Source).Normalize() * (factor * pe.Strength) / mass);

                        if(pp.Owner.Components.Get<ProjectileComponent>() is ProjectileComponent projectile && projectile.Deflectable && pp.Owner.Components.Get<DamageOnContactComponent>() is DamageOnContactComponent damager)
                        {
                            if (pe.Sender != null)
                            {
                                projectile.Thrower = pe.Sender.Owner.Id;
                                if(pe.Sender.Owner.Components.Has<PlayerComponent>())
                                {
                                    damager.Filter = DamageFilter.DamageEnemies;
                                } else if(pe.Sender.Owner.Components.Has<EnemyComponent>())
                                {
                                    damager.Filter = DamageFilter.DamageAllies;
                                }
                            }
                        }
                    }
                }
                
                if (pe.Direction.Magnitude > 0)
                {
                    List<Entity> hit = new List<Entity>();
                    List<Entity> damaged = new List<Entity>();

                    for (int i = -1; i <= 1; i++)
                    {
                        RaycastEvent raycast = new RaycastEvent(evt.Sender, new FreeVector2D(pe.Source, pe.Source + pe.Direction.Rotate(i * (Math.PI / 4)) * pe.Reach));
                        Owner.Events.InvokeEvent(raycast);
                        if(raycast.Intersected != null)
                        {
                            foreach(RaycastIntersection intersection in raycast.Intersected)
                            {
                                if(intersection.Component.Owner != pe.Sender.Owner && 
                                    intersection.Component.Owner.Components.Has<PulseReceiverPhysical>() && 
                                    !hit.Contains(intersection.Component.Owner))
                                {
                                    hit.Add(intersection.Component.Owner);
                                }
                                else if (intersection.Component.Owner != pe.Sender.Owner &&
                                    intersection.Component.Owner.Components.Has<PulseDamaged>() &&
                                    !damaged.Contains(intersection.Component.Owner))
                                {
                                    damaged.Add(intersection.Component.Owner);
                                }
                            }
                        }
                    }

                    hit.ForEach(e => Owner.Events.InvokeEvent(new ActivationEvent(pe.Sender, e, pe)));
                    damaged.ForEach(e => Owner.Events.InvokeEvent(new DamageEvent(e, 1, pe.Sender)));
                }

                foreach(PulseReceiver pr in WatchedComponents.Where(c => c is PulseReceiver))
                {
                    if (!pr.Owner.Active) return;
                    if (pr.Owner == evt.Sender.Owner) continue;

                    Vector2D point = pr.Offset;

                    if (pr.Owner.Components.Get<Spatial>() is Spatial sp) point += sp.Position;

                    if(pe.Direction.Magnitude == 0 || GeneralUtil.SubtractAngles((point - pe.Source).Angle, pe.Direction.Angle) <= Math.PI/4)
                    {
                        double distance = (point - pe.Source).Magnitude;
                        if(distance <= pe.Reach * pr.Sensitivity)
                        {
                            Owner.Events.InvokeEvent(new ActivationEvent(pe.Sender, pr.Owner, pe));
                        }
                    }
                }

                if (!pe.Sender.Owner.Components.Has<PlayerAnimation>())
                {
                    for (int i = 0; i < 5 * Math.Pow(pe.Strength / 16384, 2); i++)
                    {
                        SoundParticle particle = new SoundParticle(pe.Source + pe.Direction * 12, i * 8);
                        Owner.Entities.Add(particle);
                    }
                }
            } else if(evt is ActivationEvent e && (pem = e.Affected.Components.Get<PulseEmitterComponent>()) != null)
            {
                Vector2D source = pem.Offset;
                if (pem.Owner.Components.Get<Spatial>() is Spatial sp) source += sp.Position;
                Owner.Events.InvokeEvent(new PulseEvent(pem, source, pem.Direction, pem.Strength, pem.Reach));
            }
        }
    }
}
