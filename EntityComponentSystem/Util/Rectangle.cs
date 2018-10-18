using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Saves;

namespace EntityComponentSystem.Util
{
    [PersistentObject]
    public struct Rectangle : IEquatable<Rectangle>
    {
        [PersistentProperty]
        public double X;
        [PersistentProperty]
        public double Y;
        [PersistentProperty]
        public double Width;
        [PersistentProperty]
        public double Height;
        public static readonly Rectangle Empty = new Rectangle();

        public Vector2D Position => new Vector2D(X, Y);
        public Size Size => new Size(Width, Height);

        public double Left => X;
        public double Right => X + Width;
        public double Bottom => Y;
        public double Top => Y + Height;

        public Vector2D Center => new Vector2D(X + Width / 2, Y + Height / 2);

        public double Area => Width * Height;

        public Rectangle(Rectangle other) : this(other.X, other.Y, other.Width, other.Height)
        {
        }

        public Rectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rectangle(Vector2D pos, double width, double height)
        {
            X = pos.X;
            Y = pos.Y;
            Width = width;
            Height = height;
        }

        public Rectangle(Vector2D pos, Size size)
        {
            X = pos.X;
            Y = pos.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public static Rectangle operator +(Rectangle rect, Vector2D vect) => new Rectangle(rect.X + vect.X, rect.Y + vect.Y, rect.Width, rect.Height);
        public static Rectangle operator -(Rectangle rect, Vector2D vect) => new Rectangle(rect.X - vect.X, rect.Y - vect.Y, rect.Width, rect.Height);

        public static bool operator ==(Rectangle a, Rectangle b) => a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
        public static bool operator !=(Rectangle a, Rectangle b) => !(a == b);

        public override string ToString() => $"[x:{X}, y:{Y}, w:{Width}, h:{Height}]";
        public bool IntersectsWith(Rectangle other) => 
            this.Left < other.Right &&
            this.Right > other.Left &&
            this.Bottom < other.Top &&
            this.Top > other.Bottom;

        public bool Contains(Vector2D point)
        {
            return Left <= point.X && point.X <= Right && Bottom <= point.Y && point.Y <= Top;
        }

        public Rectangle Union(Rectangle other)
        {
            double x1 = Math.Min(this.Left, other.Left);
            double y1 = Math.Min(this.Bottom, other.Bottom);
            double x2 = Math.Max(this.Right, other.Right);
            double y2 = Math.Max(this.Top, other.Top);

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public override bool Equals(object obj) => obj is Rectangle && Equals((Rectangle)obj);
        public bool Equals(Rectangle other) => X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;

        public override int GetHashCode()
        {
            var hashCode = 466501756;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            return hashCode;
        }
    }

    public static class RectangleConversion
    {
        public static System.Drawing.Rectangle ToDrawing(this Rectangle rect)
        {
            return new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }
    }
}
