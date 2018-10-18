using System.Linq;
using EntityComponentSystem.ComponentSystems;
using EntityComponentSystem.Scenes;

using GameInterfaces.Controller;
using WooferGame.Systems.HealthSystems;

namespace WooferGame.Systems.Visual
{
    [ComponentSystem("level_renderer", ProcessingCycles.Render),
        Watching(typeof(LevelRenderable))]
    public class LevelRenderer : ComponentSystem
    {

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            /*var bg = r.GetLayerGraphics("background");
            bg.Clear(System.Drawing.Color.FromArgb(32, 255, 0, 255));
            bg.Complete();*/

            var layer = r.GetLayerGraphics("level");

            CameraView view = Owner.CurrentViewport;

            WatchedComponents = WatchedComponents.OrderBy(c => ((LevelRenderable) c).ZOrder).ToList();

            foreach (LevelRenderable tile in WatchedComponents)
            {
                if (!tile.Owner.Active) continue;
                if (tile.Owner.Components.Has<DamageFlashing>() && tile.Owner.Components.Get<Health>() is Health health && ((int)(health.InvincibilityTimer / 6) % 2) != 0) continue;
                if (tile.Owner.Components.Get<Renderable>() is Renderable renderable)
                {
                    Renderable.Render(layer, view, r, renderable.Sprites, renderable.Owner.Components.Get<Spatial>());
                }
            }
            
        }
    }
}
