using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using WooferGame.Systems.Enemies;
using WooferGame.Systems.HealthSystems;
using WooferGame.Systems.Physics;

namespace WooferGame.Systems.Visual.Particles
{
    [ComponentSystem("common_particle_system", ProcessingCycles.Update),
        Watching(typeof(ProjectileComponent)),
        Listening(typeof(RigidCollisionEvent), typeof(DamageEvent))]
    class CommonParticleSystem : ComponentSystem
    {
        public override void Update()
        {
            foreach(ProjectileComponent projectile in WatchedComponents)
            {
                if (!projectile.Owner.Active) continue;
                Spatial sp = projectile.Owner.Components.Get<Spatial>();
                if(sp != null)
                {
                    Owner.Entities.Add(new EmberParticle(sp.Position, Owner.Random.Next(0, 3)));
                }
            }
        }

        public override void EventFired(object sender, Event e)
        {
            if((e is RigidCollisionEvent || e is DamageEvent) && e.Sender.Owner.Components.Has<ProjectileComponent>())
            {
                Spatial sp = e.Sender.Owner.Components.Get<Spatial>();
                if(sp != null)
                {
                    Owner.Entities.Add(new PopParticle(sp.Position, 2));
                }
            }
        }
    }
}
