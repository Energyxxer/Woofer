using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameInterfaces.Controller;
using GameInterfaces.GraphicsInterface;

namespace EntityComponentSystem.Interfaces.Visuals
{
    public class LayerGraphics<TSurface, TSource> : DirectGraphicsContext<TSurface, TSource>
    {
        public readonly IRenderingLayer Layer;

        public LayerGraphics(IRenderingLayer layer, IGraphicsContext<TSurface, TSource> graphicsContext) : base(default(TSurface), graphicsContext)
        {
            Layer = layer;
        }

        public override void Draw(TSurface subject, Rectangle destination, Rectangle? source = null, DrawInfo info = default(DrawInfo)) => base.Draw(subject, Convert(destination), source, info);
        public override void Draw(TSource subject, Rectangle destination, Rectangle? source = null, DrawInfo info = default(DrawInfo)) => base.Draw(subject, Convert(destination), source, info);
        public override void DrawLine(Point point1, Point point2, Color color, int thickness) => base.DrawLine(Convert(point1), Convert(point2), color, thickness);
        public override void FillRect(Rectangle rectangle, Color color) => base.FillRect(Convert(rectangle), color);
        public override void FillRect(Rectangle rectangle, DrawInfo info) => base.FillRect(Convert(rectangle), info);
        public override void Update(Rectangle destination) => base.Update(Convert(destination));
        public override Size GetSize() => Layer.LayerSize;

        private Rectangle Convert(Rectangle rect)
        {
            Rectangle destination = Layer.Destination;

            double scaleX = (double)destination.Width / Layer.LayerSize.Width;
            double scaleY = (double)destination.Height / Layer.LayerSize.Height;

            rect.X = (int)Math.Floor(destination.X + rect.X * scaleX);
            rect.Y = (int)Math.Floor(destination.Y + rect.Y * scaleY);
            rect.Width = (int)Math.Round(rect.Width * scaleX);
            rect.Height = (int)Math.Round(rect.Height * scaleY);

            return rect;
        }

        private Point Convert(Point point)
        {
            Rectangle destination = Layer.Destination;

            double scaleX = (double)destination.Width / Layer.LayerSize.Width;
            double scaleY = (double)destination.Height / Layer.LayerSize.Height;

            point.X = (int)Math.Floor(destination.X + point.X * scaleX);
            point.Y = (int)Math.Floor(destination.Y + point.Y * scaleY);

            return point;
        }
    }
}
