using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameInterfaces.Controller;

namespace WooferGame.Controller
{
    class BackParallaxRenderingLayer : IRenderingLayer
    {
        private IGameController gameController;

        public string Name => "parallax_back";
        public Size LayerSize => new Size(320, 180);
        public Rectangle Destination => new Rectangle(new Point(0, 0), gameController.RenderingUnit.ScreenSize);

        public BackParallaxRenderingLayer(IGameController gameController) => this.gameController = gameController;
    }
    class FrontParallaxRenderingLayer : IRenderingLayer
    {
        private IGameController gameController;

        public string Name => "parallax_front";
        public Size LayerSize => new Size(320, 180);
        public Rectangle Destination => new Rectangle(new Point(0, 0), gameController.RenderingUnit.ScreenSize);

        public FrontParallaxRenderingLayer(IGameController gameController) => this.gameController = gameController;
    }
}
