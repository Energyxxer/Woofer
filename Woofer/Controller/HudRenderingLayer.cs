using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameInterfaces.Controller;

namespace WooferGame.Controller
{
    class HudRenderingLayer : IRenderingLayer
    {
        private IGameController gameController;

        public string Name => "hud";
        public Size LayerSize => new Size(320, 180);
        public Rectangle Destination => new Rectangle(new Point(0, 0), gameController.RenderingUnit.ScreenSize);

        public HudRenderingLayer(IGameController gameController) => this.gameController = gameController;
    }
}
