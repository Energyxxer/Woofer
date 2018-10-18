using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Events;
using EntityComponentSystem.Util;
using WooferGame.Systems.Movement;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Player.Actions;
using WooferGame.Systems.Visual;
using WooferGame.Systems.Visual.Animation;
using WooferGame.Systems.Visual.Particles;

namespace WooferGame.Systems.Refill
{
    [ComponentSystem("energy_refill", ProcessingCycles.Update),
        Watching(typeof(EnergyRefillComponent), typeof(PlayerMovementComponent)),
        Listening(typeof(SoftCollisionEvent))]
    class EnergyRefillSystem : ComponentSystem
    {
        private PlayerMovementComponent player;
        private bool AnyDisabled = false;

        public override void Update()
        {
            player = WatchedComponents.FirstOrDefault(c => c is PlayerMovementComponent) as PlayerMovementComponent;
            if (AnyDisabled && (player?.OnGround ?? false))
            {
                foreach (EnergyRefillComponent refill in WatchedComponents.OfType<EnergyRefillComponent>())
                {
                    if (refill.Enabled) continue;
                    refill.Enabled = true;
                    Renderable renderable = refill.Owner.GetComponent<Renderable>();
                    if(renderable != null && renderable.Sprites.Count >= 1)
                    {
                        renderable.Sprites[0].Source = new Rectangle(0, 0, 16, 16);
                    }
                    AnimationComponent animatable = refill.Owner.GetComponent<AnimationComponent>();
                    if(animatable != null && animatable.Animations.Count >= 1 && animatable.Animations[0].SpriteIndex == 0)
                    {
                        AnimatedSprite animation = animatable.Animations[0];
                        animation.Loop = true;
                        animation.CurrentFrame = 0;
                        animation.LoopFrame = 5;
                        animation.FrameProgress = 0;
                        animation.FrameDuration = 8;
                        animation.FrameCount = 9;
                        animation.Frame = new Rectangle(128, 0, 16, 16);
                        animation.Step = new Vector2D(-16, 0);
                    }
                }
                AnyDisabled = false;
            }
        }

        public override void EventFired(object sender, Event evt)
        {
            if(evt is SoftCollisionEvent ce &&
                ce.Sender.Owner.Components.Get<EnergyRefillComponent>() is EnergyRefillComponent refill &&
                refill.Enabled &&
                ce.Victim.Components.Get<PulseAbility>() is PulseAbility pulse)
            {
                pulse.EnergyMeter = pulse.MaxEnergy;
                refill.Enabled = false;
                Spatial sp = refill.Owner.GetComponent<Spatial>();
                if(sp != null) Owner.Entities.Add(new SoundParticle(sp.Position));
                Owner.Controller.AudioUnit["refill"].Play();
                AnyDisabled = true;

                AnimationComponent animatable = refill.Owner.GetComponent<AnimationComponent>();
                if (animatable != null && animatable.Animations.Count >= 1 && animatable.Animations[0].SpriteIndex == 0)
                {
                    AnimatedSprite animation = animatable.Animations[0];
                    animation.Loop = false;
                    animation.CurrentFrame = 0;
                    animation.LoopFrame = 0;
                    animation.FrameProgress = 0;
                    animation.FrameDuration = 3;
                    animation.FrameCount = 6;
                    animation.Frame = new Rectangle(64, 0, 16, 16);
                    animation.Step = new Vector2D(16, 0);
                }
            }
        }
    }
}
