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
using WooferGame.Systems.DeathBarrier;
using WooferGame.Systems.HealthSystems;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Pulse;
using WooferGame.Systems.Setters;
using WooferGame.Systems.Visual;
using WooferGame.Systems.Visual.Animation;

namespace WooferGame.Systems.Enemies.Boss
{
    [Component("boss_projectile")]
    class BossProjectile : Component
    {
        public bool Deflected { get; internal set; }
    }

    class BossProjectileEntity : Entity
    {
        public BossProjectileEntity(Vector2D pos)
        {
            Components.Add(new Spatial(pos));
            Components.Add(new Physical() { GravityMultiplier = 0.8 });
            Components.Add(new SoftBody(new CollisionBox(-16, -16, 32, 32), 1f) { Movable = false, PassThroughLevel = true });
            Components.Add(new LevelRenderable(-3));
            Components.Add(new Renderable(new Sprite("enemies", new Rectangle(-24, -24, 48, 48), new Rectangle(0, 48, 48, 48))));
            Components.Add(new AnimationComponent(new AnimatedSprite(0, new Rectangle(0, 48, 48, 48), new Vector2D(48, 0), 6, 10) { Loop = true }));
            Components.Add(new PulseReceiverPhysical());
            Components.Add(new VelocitySetter(0, new Vector2D(0, 384), true));
            Components.Add(new PulsePushable());
            Components.Add(new DamageOnContactComponent() { Remove = true });
            Components.Add(new RemoveOnBarrierComponent());
            Components.Add(new ProjectileComponent());

            Components.Add(new BossProjectile());
        }
    }
}
