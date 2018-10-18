using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EntityComponentSystem.Interfaces.Visuals;
using GameInterfaces.GraphicsInterface;

namespace GameInterfaces.Controller
{
    public class ScreenRenderer<TSurface, TSource>
    {
        public readonly IGraphicsContext<TSurface, TSource> GraphicsContext;

        private IRenderingUnit renderingUnit;
        private Dictionary<string, IRenderingLayer> layers = new Dictionary<string, IRenderingLayer>();

        public ISpriteManager<TSource> SpriteManager;

        public ScreenRenderer(IGraphicsContext<TSurface, TSource> graphicsContext, ISpriteManager<TSource> spriteManager, IRenderingUnit renderingUnit)
        {
            this.GraphicsContext = graphicsContext;
            this.SpriteManager = spriteManager;
            UpdateLayers(renderingUnit);
        }

        public void UpdateLayers(IRenderingUnit renderingUnit)
        {
            this.renderingUnit = renderingUnit;
            
            layers.Clear();

            foreach(IRenderingLayer layer in renderingUnit.Layers.Values)
            {
                layers[layer.Name] = layer;
            }
        }

        public LayerGraphics<TSurface, TSource> GetLayerGraphics(string name)
        {
            return new LayerGraphics<TSurface, TSource>(layers[name], GraphicsContext);
        }

        public void Begin()
        {
            GraphicsContext.Begin();
        }

        public void End()
        {
            GraphicsContext.Reset();
        }
    }
}
