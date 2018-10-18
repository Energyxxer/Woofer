using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Events;
using WooferGame.Systems.Visual.Animation;

namespace WooferGame.Systems.Visual.Particles
{
    [ComponentSystem("particles", ProcessingCycles.Update),
        Watching(typeof(ParticleComponent)),
        Listening(typeof(AnimationEndEvent))]
    class ParticleSystem : ComponentSystem
    {
        public int MaxParticleCount = 100;

        public override void Update()
        {
            if(WatchedComponents.Count > MaxParticleCount)
            {
                int toRemove = WatchedComponents.Count - MaxParticleCount;
                foreach(ParticleComponent particle in WatchedComponents)
                {
                    particle.Owner.Remove();
                    toRemove--;
                    if (toRemove <= 0) break;
                }
            }
        }

        public override void EventFired(object sender, Event evt)
        {
            if(evt is AnimationEndEvent ae)
            {
                if (!ae.Component.Owner.Components.Has<ParticleComponent>()) return;
                /*ae.Component.Animations.Remove(ae.Animation);
                if (ae.Component.Animations.Count == 0) */ae.Component.Owner.Remove();
            }
        }
    }
}
