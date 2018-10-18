using System.Drawing;
using EntityComponentSystem.ComponentSystems;
using GameInterfaces.Controller;
using WooferGame.Common;

namespace WooferGame.Meta.Loading
{
    [ComponentSystem("loading", ProcessingCycles.Update | ProcessingCycles.Render)]
    class LoadingScreenSystem : ComponentSystem
    {
        private double Timer = 0;

        public override void Update()
        {
            Timer += Owner.DeltaTime;
            if(Timer >= 1)
            {
                Timer--;
            }
        }

        public override void Render<TSurface, TSource>(ScreenRenderer<TSurface, TSource> r)
        {
            var layer = r.GetLayerGraphics("hi_res_overlay");

            layer.Draw(r.SpriteManager["parallax_bg"], new Rectangle(new Point(0, 0), layer.GetSize()), new System.Drawing.Rectangle(0, 0, 320, 180));
            layer.Draw(r.SpriteManager["parallax_bg"], new Rectangle(new Point(0, 0), layer.GetSize()), new System.Drawing.Rectangle(0, 180, 320, 180));
            layer.Draw(r.SpriteManager["parallax_bg"], new Rectangle(new Point(0, 0), layer.GetSize()), new System.Drawing.Rectangle(0, 360, 320, 180));

            TextUnit label = new TextUnit("Loading" + new string('.', (int)(Timer * 3) + 1));
            Size labelSize = label.GetSize(5);
            label.Render(r, layer, new Point(0, layer.GetSize().Height - labelSize.Height), 5);
        }
    }
}