using GameInterfaces.Controller;
using GameInterfaces.GraphicsInterface;

using Microsoft.Xna.Framework.Graphics;

namespace GameBase.MonoGameGraphics
{
    public class MonoGameScreenRenderer : ScreenRenderer<RenderTarget2D, Texture2D>
    {
        public MonoGameScreenRenderer(IGraphicsContext<RenderTarget2D, Texture2D> graphicsContext, ISpriteManager<Texture2D> spriteManager, IRenderingUnit renderingUnit) : base(graphicsContext, spriteManager, renderingUnit)
        {
        }
    }
}
