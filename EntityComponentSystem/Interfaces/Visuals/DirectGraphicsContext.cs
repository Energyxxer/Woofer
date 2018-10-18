using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Interfaces.Visuals;

namespace GameInterfaces.GraphicsInterface
{
    public class DirectGraphicsContext<TSurface, TSource> : IDisposable
    {
        public readonly TSurface Surface;
        public readonly IGraphicsContext<TSurface, TSource> GraphicsContext;

        public DirectGraphicsContext(TSurface surface, IGraphicsContext<TSurface, TSource> graphicsContext)
        {
            Surface = surface;
            GraphicsContext = graphicsContext;
        }

        public virtual void Clear(Color color) => GraphicsContext.Clear(Surface, color);
        public virtual Size GetSize() => GraphicsContext.GetSize(Surface);
        public virtual Size GetSize(TSource source) => GraphicsContext.GetSize(source);

        public virtual void Draw(TSurface subject, Rectangle destination, Rectangle? source = null, DrawInfo info = default(DrawInfo)) => GraphicsContext.Draw(subject, Surface, destination, source, info);
        public virtual void Draw(TSource subject, Rectangle destination, Rectangle? source = null, DrawInfo info = default(DrawInfo)) => GraphicsContext.Draw(subject, Surface, destination, source, info);

        public virtual void FillRect(Rectangle rectangle, Color color) => GraphicsContext.FillRect(Surface, rectangle, color);
        public virtual void FillRect(Rectangle rectangle, DrawInfo info) => GraphicsContext.FillRect(Surface, rectangle, info);
        public virtual void DrawLine(Point point1, Point point2, Color color, int thickness) => GraphicsContext.DrawLine(Surface, point1, point2, color, thickness);

        public virtual TSurface CreateTarget(int width, int height) => GraphicsContext.CreateTarget(width, height);

        public virtual TSource TargetToSource(TSurface target) => GraphicsContext.TargetToSource(target);

        public virtual TSurface Scale(double scaleX, double scaleY, bool antialias) => GraphicsContext.Scale(Surface, scaleX, scaleY, antialias);
        public virtual TSurface Scale(double scale, bool antialias) => GraphicsContext.Scale(Surface, scale, antialias);

        public virtual void Update() => GraphicsContext.Update(Surface);
        public virtual void Update(Rectangle destination) => GraphicsContext.Update(Surface, destination);
        public virtual void DisposeSurface() => GraphicsContext.DisposeSurface(Surface);
        public virtual void DisposeSource(TSource source) => GraphicsContext.DisposeSource(source);

        public virtual void Reset() => GraphicsContext.Reset();

        public virtual void Complete()
        {
            Dispose();
        }

        public virtual void Dispose() { }

    }
}
