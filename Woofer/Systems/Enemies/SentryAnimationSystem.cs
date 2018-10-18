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
using GameInterfaces.Controller;
using WooferGame.Systems.Visual;

namespace WooferGame.Systems.Enemies
{
    [ComponentSystem("sentry_animation", ProcessingCycles.Render),
        Watching(typeof(SentryAI))]
    class SentryAnimationSystem : ComponentSystem
    {
        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            foreach(SentryAI sentry in WatchedComponents)
            {
                Renderable renderable = sentry.Owner.Components.Get<Renderable>();
                if (renderable == null) continue;
                if(renderable.Sprites.Count == 0)
                {
                    renderable.Sprites.Add(new Sprite("enemies", new Rectangle(-16, -13, 32, 32), new Rectangle(0, 0, 32, 32)));
                }
                renderable.Sprites[0].Source.X = sentry.OnGround ? 0 : 32;
                if(sentry.ActionTime > 30 && sentry.Action == SentryAction.Throw)
                {
                    if (Math.Sin(Math.Pow(((float)sentry.ActionTime - 80) / 8, 2)) >= 0)
                    {
                        renderable.Sprites[0].Source.X = 64;
                    }
                }
            }
        }
    }
}
