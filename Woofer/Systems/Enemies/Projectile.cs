using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;
using WooferGame.Systems.DeathBarrier;
using WooferGame.Systems.HealthSystems;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Player;
using WooferGame.Systems.Pulse;
using WooferGame.Systems.Visual;
using WooferGame.Systems.Visual.Animation;

namespace WooferGame.Systems.Enemies
{
    class Projectile : Entity
    {
        public Projectile(Entity thrower, Vector2D pos, Vector2D velocity)
        {
            Components.Add(new Spatial(pos));
            Components.Add(new Physical() { Velocity = velocity});

            Components.Add(new LevelRenderable());
            Components.Add(new Renderable(new Sprite("enemies", new Rectangle(-8, -8, 16, 16), new Rectangle(0, 32, 16, 16))));
            Components.Add(new AnimationComponent(new AnimatedSprite(0, new Rectangle(0, 32, 16, 16), new Vector2D(16, 0), 7, 5) { Loop = true }));

            Components.Add(new SoftBody(new CollisionBox(-4, -4, 8, 8), 0.5f));

            Components.Add(new RemoveOnCollision());
            Components.Add(new RemoveOnBarrierComponent());
            Components.Add(new ProjectileComponent() { Thrower = thrower.Id });
            Components.Add(new PulsePushable());
            Components.Add(new DamageOnContactComponent() { Damage = 1, Remove = true, Filter = (thrower.Components.Get<PlayerComponent>() != null) ? DamageFilter.DamageEnemies : DamageFilter.DamageAllies });
        }
    }
}
