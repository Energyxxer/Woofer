using System.Collections.Generic;
using System.Drawing;

namespace GameInterfaces.Controller
{
    public interface IRenderingUnit
    {
        Size ScreenSize { get; }
        Dictionary<string, IRenderingLayer> Layers { get; }

        void LoadContent<TSurface, TSource>(ScreenRenderer<TSurface, TSource> screenRenderer);
        void Draw<TSurface, TSource>(ScreenRenderer<TSurface, TSource> screenRenderer);
    }
}
