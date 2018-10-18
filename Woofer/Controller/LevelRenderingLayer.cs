using System;
using System.Drawing;

using GameInterfaces.Controller;

namespace WooferGame.Controller
{
    public class LevelRenderingLayer : IRenderingLayer
    {
        private IGameController gameController;

        public string Name => "level";
        public Size LayerSize => new Size(EffectiveLayerSize.Width + 1, EffectiveLayerSize.Height + 1);
        public Size EffectiveLayerSize => LevelScreenSize;

        //private readonly int LockedResolutionWidth = 320;

        public static readonly Size DefaultLevelScreenSize = new Size(320, 180);

        public static Size LevelScreenSize = DefaultLevelScreenSize;

        public Rectangle Destination
        {
            get
            {
                Size screenSize = gameController.RenderingUnit.ScreenSize;

                double scale = (double)screenSize.Width / EffectiveLayerSize.Width;
                scale *= gameController.ActiveScene.CurrentViewport.Scale;

                Point destination = new Point(0, 0);
                destination.X -= (int)Math.Round((EffectiveLayerSize.Width * scale - screenSize.Width) / 2);
                destination.Y -= (int)Math.Round((EffectiveLayerSize.Height * scale - screenSize.Height) / 2);
                destination.X -= (int)(scale * EntityComponentSystem.Util.GeneralUtil.EuclideanMod(gameController.ActiveScene.CurrentViewport.X, 1));
                destination.Y += (int)(scale * (EntityComponentSystem.Util.GeneralUtil.EuclideanMod(gameController.ActiveScene.CurrentViewport.Y, 1)-1));

                return new Rectangle(destination, new Size((int)(LayerSize.Width * scale), (int)(LayerSize.Height * scale)));
            }
        }

        public LevelRenderingLayer(IGameController gameController) => this.gameController = gameController;
    }
}
