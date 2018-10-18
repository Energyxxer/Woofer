using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Interfaces.Visuals;
using EntityComponentSystem.Util;
using WooferGame.Systems.Visual.Animation;

namespace WooferGame.Systems.Visual.Particles
{
    class SoundParticle : EntityComponentSystem.Entities.Entity
    {
        public SoundParticle(Vector2D pos) : this(pos, 0)
        {

        }

        public SoundParticle(Vector2D pos, int delay)
        {
            Components.Add(new Spatial(pos));
            Components.Add(new Renderable(new Sprite("particles", new Rectangle(-32, -32, 64, 64), new Rectangle(0, 0, 0, 0)) { DrawMode=DrawMode.Overlay}));
            Components.Add(new LevelRenderable(4));
            Components.Add(new AnimationComponent(new AnimatedSprite(0, new Rectangle(0, 32, 64, 64), new Vector2D(64, 0), 6, 2) { FrameProgress = -(delay+1) }));
            Components.Add(new ParticleComponent());
        }
    }
}
