using System.Drawing;

using GameInterfaces.Controller;

namespace WooferGame.Controller
{
    public class BackgroundRenderingLayer : IRenderingLayer
    {
        private IGameController gameController;

        public string Name => "background";
        public Size LayerSize => new Size(320, 180);
        public Rectangle Destination => new Rectangle(new Point(0, 0), gameController.RenderingUnit.ScreenSize);

        public BackgroundRenderingLayer(IGameController gameController) => this.gameController = gameController;
    }
}
