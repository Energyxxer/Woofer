using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;

namespace WooferGame.Systems.Visual.Animation
{
    [ComponentSystem("animation", ProcessingCycles.Update | ProcessingCycles.Render),
        Watching(typeof(AnimationComponent))]
    class AnimationSystem : ComponentSystem
    {
        public override void Update()
        {
            foreach(AnimationComponent c in WatchedComponents)
            {
                foreach(AnimatedSprite anim in c.Animations)
                {
                    if (anim.CurrentFrame < 0 || anim.CurrentFrame >= anim.FrameCount) continue;
                    anim.FrameProgress++;
                    if (anim.FrameProgress == 0) Owner.Events.InvokeEvent(new AnimationStartEvent(c, anim));
                    if (anim.FrameProgress >= anim.FrameDuration)
                    {
                        anim.FrameProgress = 0;
                        anim.CurrentFrame++;
                        if (anim.CurrentFrame >= anim.FrameCount)
                        {
                            if (anim.Loop)
                            {
                                anim.CurrentFrame = anim.LoopFrame;
                            }
                            else Owner.Events.InvokeEvent(new AnimationEndEvent(c, anim));
                        }
                    }
                }
            }
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            if (Owner.Controller.Paused) return;
            foreach(AnimationComponent c in WatchedComponents)
            {
                Renderable renderable = c.Owner.Components.Get<Renderable>();
                if (renderable == null) continue;

                foreach(AnimatedSprite anim in c.Animations)
                {
                    if(anim.SpriteIndex >= 0 && anim.SpriteIndex < renderable.Sprites.Count)
                    {
                        if (anim.FrameProgress >= 0) renderable.Sprites[anim.SpriteIndex].Source = anim.Frame + (anim.Step * anim.CurrentFrame);
                        else renderable.Sprites[anim.SpriteIndex].Source = new Rectangle(-1, -1, 0, 0);
                    }
                }
            }
        }
    }
}
