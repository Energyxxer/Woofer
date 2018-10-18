using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Util;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Visual.Animation;

namespace WooferGame.Systems.Visual.Particles
{
    class CloudParticle : Entity
    {
        public CloudParticle(Vector2D pos) : this(pos, 0)
        {

        }

        public CloudParticle(Vector2D pos, int delay)
        {
            Components.Add(new Spatial(pos));
            Components.Add(new Renderable(new Sprite("particles", new Rectangle(-4.5, -2, 9, 4), new Rectangle(0, 0, 9, 4))));
            Components.Add(new LevelRenderable());
            Components.Add(new AnimationComponent(new AnimatedSprite(0, new Rectangle(0, 0, 9, 4), new Vector2D(9, 0), 6, 4) { FrameProgress = -(delay + 1) }));
            Components.Add(new ParticleComponent());
            Components.Add(new Physical() { GravityMultiplier = 0.1 });
        }

        public override void Initialize()
        {
            Components.Get<Physical>().Velocity = new Vector2D(Owner.Random.NextDouble() * 16, 0).Rotate(Owner.Random.NextDouble() * 2 * Math.PI);
        }
    }

    class EmberParticle : Entity
    {
        public EmberParticle(Vector2D pos) : this(pos, 0)
        {
        }

        public EmberParticle(Vector2D pos, int delay)
        {
            Components.Add(new Spatial(pos));
            Components.Add(new Renderable(new Sprite("particles", new Rectangle(-4.5, -2, 9, 4), new Rectangle(0, 4, 9, 4))));
            Components.Add(new LevelRenderable());
            Components.Add(new AnimationComponent(new AnimatedSprite(0, new Rectangle(0, 4, 9, 4), new Vector2D(9, 0), 6, 2) { FrameProgress = -(delay + 1) }));
            Components.Add(new ParticleComponent());
            Components.Add(new Physical() { GravityMultiplier = 0.1 });
        }

        public override void Initialize()
        {
            Components.Get<Physical>().Velocity = new Vector2D(Owner.Random.NextDouble() * 16, 0).Rotate(Owner.Random.NextDouble() * 2 * Math.PI);
        }
    }

    class PopParticle : Entity
    {
        public PopParticle(Vector2D pos) : this(pos, 0)
        {
        }

        public PopParticle(Vector2D pos, int delay)
        {
            Components.Add(new Spatial(pos));
            Components.Add(new Renderable(new Sprite("particles", new Rectangle(-4, -4, 8, 8), new Rectangle(0, 8, 8, 8))));
            Components.Add(new LevelRenderable());
            Components.Add(new AnimationComponent(new AnimatedSprite(0, new Rectangle(0, 8, 8, 8), new Vector2D(8, 0), 8, 1) { FrameProgress = -(delay + 1) }));
            Components.Add(new ParticleComponent());
        }
    }

    class BossFlameParticle : Entity
    {
        public BossFlameParticle(Vector2D pos, int variant)
        {
            int xOff = 12 * (variant - 1);
            if(variant == 3)
            {
                xOff = -18;
            } else if(variant == 4)
            {
                xOff = 18;
            }

            int sourceX = 24 * (variant + 1);
            if (variant == 3) sourceX = 140;
            else if (variant == 4) sourceX = 210;

            Rectangle source = new Rectangle(sourceX, 0, 24, 96);

            Components.Add(new Spatial(pos + new Vector2D(xOff, 0)));
            Components.Add(new Renderable(new Sprite("boss_particles", new Rectangle(-12, -96, 24, 96), source)));
            Components.Add(new Physical() { GravityMultiplier = 0 });
            Components.Add(new LevelRenderable(-1));
            Components.Add(new AnimationComponent(new AnimatedSprite(0, source, new Vector2D(0, 96), 8, 5)));
            Components.Add(new ParticleComponent());
        }
    }
}
