using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Events;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;
using GameInterfaces.Controller;

namespace WooferGame.Systems.Parallax
{
    [ComponentSystem("ParallaxSystem", ProcessingCycles.Render),
        Watching(typeof(Parallax))]
    class ParallaxSystem : ComponentSystem
    {
        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            var back = r.GetLayerGraphics("parallax_back");
            var front= r.GetLayerGraphics("parallax_front");
            CameraView view = Owner.CurrentViewport;

            foreach(Parallax parallax in WatchedComponents)
            {
                if (!parallax.Owner.Active) continue;
                var layer = parallax.Speed.X >= 1 ? front : back;
                Spatial sp = parallax.Owner.Components.Get<Spatial>();

                Vector2D center = (sp.Position - view.Location);
                center.X *= parallax.Speed.X;
                center.Y *= parallax.Speed.Y;

                double width = parallax.SourceBounds.Width * parallax.Scale;
                double height = parallax.SourceBounds.Height * parallax.Scale;

                Rectangle destination = new Rectangle(center.X, center.Y, width, height);
                destination.X -= width / 2;
                destination.Y -= height / 2;

                destination.X += layer.GetSize().Width / 2;
                destination.Y += layer.GetSize().Height / 2;

                destination.Y *= -1;

                layer.Draw(r.SpriteManager[parallax.Texture], destination.ToDrawing(), parallax.SourceBounds.ToDrawing());
            }
        }
    }
}
