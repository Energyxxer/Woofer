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
using WooferGame.Scenes.LevelObjects;
using WooferGame.Systems.Checkpoints;
using WooferGame.Systems.Commands;
using WooferGame.Systems.HealthSystems;
using WooferGame.Systems.HUD;
using WooferGame.Systems.Interaction;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Player.Actions;
using WooferGame.Systems.Visual;
using WooferGame.Systems.Visual.Particles;

namespace WooferGame.Systems.Player
{
    [ComponentSystem("general_player_system", ProcessingCycles.Update),
        Watching(typeof(PlayerComponent), typeof(OnLoadTrigger)),
        Listening(typeof(ActivationEvent), typeof(DeathEvent))]
    class GeneralPlayerSystem : ComponentSystem
    {
        public override void Update()
        {
            foreach(PlayerComponent player in WatchedComponents.OfType<PlayerComponent>())
            {
                if(!player.Initialized)
                {
                    Entity entity = player.Owner;
                    if(Woofer.Controller.CurrentSave.Data.HasWoofer)
                    {
                        entity.Components.Add(new PulseAbility());
                        entity.Components.Get<PulseAbility>().MaxEnergy = Woofer.Controller.CurrentSave.Data.MaxEnergy;
                    } else
                    {
                        entity.Components.Remove<PulseAbility>();
                    }
                    if(!entity.Components.Has<Health>())
                    {
                        entity.Components.Add(new Health());
                    }
                    entity.Components.Add(new DamageFlashing());
                    Health health = entity.Components.Get<Health>();
                    bool heal = health.CurrentHealth == health.MaxHealth;
                    health.MaxHealth = Woofer.Controller.CurrentSave.Data.MaxHealth;
                    if (heal) health.CurrentHealth = health.MaxHealth;
                    health.HealthBarOffset = new Vector2D(0, 32);
                    health.RemoveOnDeath = false;
                    player.Initialized = true;

                    foreach(Component listener in WatchedComponents.OfType<OnLoadTrigger>())
                    {
                        Owner.Events.InvokeEvent(new ActivationEvent(player, listener.Owner, null));
                    }
                }
            }
        }

        public override void EventFired(object sender, Event evt)
        {
            if (evt is ActivationEvent ae)
            {
                if (ae.Affected.Components.Has<WooferGiverComponent>())
                {
                    Entity player = WatchedComponents.FirstOrDefault()?.Owner;
                    if (player == null) return;
                    ae.Affected.Active = false;
                    if (!player.Components.Has<PulseAbility>())
                    {
                        player.Components.Add(new PulseAbility());
                        Woofer.Controller.CurrentSave.Data.HasWoofer = true;
                        Woofer.Controller.CommandFired(new SaveCommand());

                        Owner.Events.InvokeEvent(new ShowTextEvent(new Sprite("x_icons", new Rectangle(0, 0, 9, 9), new Rectangle(0, 9, 9, 9)) { Modifiers = Sprite.Mod_InputType }, "Activate", ae.Sender) { Duration = 10 });
                    }
                }
                if (ae.Affected.Components.Get<WooferUpgradeComponent>() is WooferUpgradeComponent wooferUpgrade)
                {
                    Entity player = WatchedComponents.FirstOrDefault()?.Owner;
                    if (player == null) return;
                    ae.Affected.Active = false;
                    Woofer.Controller.CurrentSave.Data.MaxEnergy = Math.Max(Woofer.Controller.CurrentSave.Data.MaxEnergy, wooferUpgrade.Energy);
                    if (player.GetComponent<PulseAbility>() is PulseAbility pulse)
                    {
                        pulse.MaxEnergy = Woofer.Controller.CurrentSave.Data.MaxEnergy;
                        pulse.EnergyMeter = pulse.MaxEnergy;
                    }
                    Woofer.Controller.CommandFired(new SaveCommand());
                    Spatial sp = ae.Affected.GetComponent<Spatial>();
                    if (sp != null) Owner.Entities.Add(new SoundParticle(sp.Position));
                    Owner.Controller.AudioUnit["refill"].Play();

                    Owner.Events.InvokeEvent(new ShowTextEvent(new Sprite("x_icons", new Rectangle(0, 0, 9, 9), new Rectangle(0, 9, 9, 9)) { Modifiers = Sprite.Mod_InputType }, "Activate", ae.Sender) { Duration = 10 });
                }
                if (ae.Affected.Components.Get<HealthUpgradeComponent>() is HealthUpgradeComponent healthUpgrade)
                {
                    Entity player = WatchedComponents.FirstOrDefault()?.Owner;
                    if (player == null) return;
                    ae.Affected.Active = false;
                    Woofer.Controller.CurrentSave.Data.MaxHealth = Math.Max(Woofer.Controller.CurrentSave.Data.MaxHealth, healthUpgrade.Health);
                    if (player.GetComponent<Health>() is Health health)
                    {
                        health.MaxHealth = Woofer.Controller.CurrentSave.Data.MaxHealth;
                        health.CurrentHealth = health.MaxHealth;
                        health.RegenCooldown = health.RegenRate;
                        health.HealthBarVisible = true;
                    }
                    Woofer.Controller.CommandFired(new SaveCommand());
                    Spatial sp = ae.Affected.GetComponent<Spatial>();
                    if (sp != null) Owner.Entities.Add(new SoundParticle(sp.Position));
                    Owner.Controller.AudioUnit["refill"].Play();
                }
            }
            else if(evt is DeathEvent de && de.Affected.HasComponent<PlayerComponent>())
            {
                Health health = de.Affected.GetComponent<Health>();
                if(health != null)
                {
                    health.RemoveOnDeath = false;
                    health.CurrentHealth = health.MaxHealth;
                }
                Owner.Events.InvokeEvent(new CheckpointRequestEvent(de.Affected.GetComponent<PlayerComponent>(), de.Affected));
            }
        }
    }
}
