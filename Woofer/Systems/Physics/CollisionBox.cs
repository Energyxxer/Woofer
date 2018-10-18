using System;
using System.Collections.Generic;
using System.Linq;
using EntityComponentSystem.Util;

namespace WooferGame.Systems.Physics
{
    public class CollisionBox : IEquatable<CollisionBox>
    {
        private const int TopFace = 0;
        private const int RightFace = 1;
        private const int BottomFace = 2;
        private const int LeftFace = 3;

        public double X;
        public double Y;
        public double Width;
        public double Height;

        public double Top => Y + Height;
        public double Right => X + Width;
        public double Bottom => Y;
        public double Left => X;

        public double Area => Width * Height;
        public double Perimeter => 2 * (Width + Height);

        private CollisionFaceProperties[] faceProperties = new CollisionFaceProperties[4]; //top, right, bottom, left

        public CollisionFaceProperties TopFaceProperties
        {
            get => faceProperties[TopFace];
            set => faceProperties[TopFace] = value;
        }
        public CollisionFaceProperties RightFaceProperties
        {
            get => faceProperties[RightFace];
            set => faceProperties[RightFace] = value;
        }

        public Rectangle ToRectangle() => new Rectangle(X, Y, Width, Height);

        public CollisionFaceProperties BottomFaceProperties
        {
            get => faceProperties[BottomFace];
            set => faceProperties[BottomFace] = value;
        }
        public CollisionFaceProperties LeftFaceProperties
        {
            get => faceProperties[LeftFace];
            set => faceProperties[LeftFace] = value;
        }
        public Vector2D Center => new Vector2D(X + (Width / 2), Y + (Height / 2));

        public CollisionBox() : this(new Rectangle()) { }

        public CollisionBox(Rectangle rectangle) : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
        {

        }

        public CollisionBox(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            faceProperties[0] = new CollisionFaceProperties() { Enabled = true, Friction = 0.3 }; //top
            faceProperties[1] = new CollisionFaceProperties() { Enabled = true }; //right
            faceProperties[2] = new CollisionFaceProperties() { Enabled = true }; //bottom
            faceProperties[3] = new CollisionFaceProperties() { Enabled = true }; //left
        }

        public CollisionBox Offset(Vector2D delta)
        {
            return new CollisionBox(X + delta.X, Y + delta.Y, Width, Height)
            {
                TopFaceProperties = TopFaceProperties,
                RightFaceProperties = RightFaceProperties,
                BottomFaceProperties = BottomFaceProperties,
                LeftFaceProperties = LeftFaceProperties
            };
        }

        public bool IntersectsWith(CollisionBox other)
        {
            return this.Left < other.Right &&
                    this.Right > other.Left &&
                    this.Bottom < other.Top &&
                    this.Top > other.Bottom;
        }

        public CollisionBox Intersect(CollisionBox rectangle)
        {
            if (!IntersectsWith(rectangle)) return null;
            double x1 = Math.Max(this.Left, rectangle.Left);
            double y1 = Math.Max(this.Bottom, rectangle.Bottom);
            double x2 = Math.Min(this.Right, rectangle.Right);
            double y2 = Math.Min(this.Top, rectangle.Top);

            CollisionBox newBox = new CollisionBox(x1, y1, x2 - x1, y2 - y1);
            newBox.faceProperties[TopFace] = (newBox.Top == this.Top ? this : rectangle).faceProperties[TopFace];
            newBox.faceProperties[RightFace] = (newBox.Right == this.Right ? this : rectangle).faceProperties[RightFace];
            newBox.faceProperties[BottomFace] = (newBox.Bottom == this.Bottom ? this : rectangle).faceProperties[BottomFace];
            newBox.faceProperties[LeftFace] = (newBox.Left == this.Left ? this : rectangle).faceProperties[LeftFace];

            return newBox;
        }

        public CollisionBox Union(CollisionBox rectangle)
        {
            double x1 = Math.Min(this.Left, rectangle.Left);
            double y1 = Math.Min(this.Bottom, rectangle.Bottom);
            double x2 = Math.Max(this.Right, rectangle.Right);
            double y2 = Math.Max(this.Top, rectangle.Top);

            CollisionBox newBox = new CollisionBox(x1, y1, x2 - x1, y2 - y1);
            newBox.faceProperties[TopFace] = (newBox.Top == this.Top ? this : rectangle).faceProperties[TopFace];
            newBox.faceProperties[RightFace] = (newBox.Right == this.Right ? this : rectangle).faceProperties[RightFace];
            newBox.faceProperties[BottomFace] = (newBox.Bottom == this.Bottom ? this : rectangle).faceProperties[BottomFace];
            newBox.faceProperties[LeftFace] = (newBox.Left == this.Left ? this : rectangle).faceProperties[LeftFace];

            return newBox;
        }

        public Vector2D[] GetVertices()
        {
            Vector2D[] vertices = new Vector2D[4];
            vertices[0] = new Vector2D(Left, Top);
            vertices[1] = new Vector2D(Right, Top);
            vertices[2] = new Vector2D(Right, Bottom);
            vertices[3] = new Vector2D(Left, Bottom);
            return vertices;
        }

        public List<FreeVector2D> GetSides()
        {
            List<FreeVector2D> sides = new List<FreeVector2D>();
            Vector2D[] vertices = GetVertices();

            if(faceProperties[TopFace].Enabled) sides.Add(new FreeVector2D(vertices[0], vertices[1]));
            if(faceProperties[RightFace].Enabled) sides.Add(new FreeVector2D(vertices[1], vertices[2]));
            if(faceProperties[BottomFace].Enabled) sides.Add(new FreeVector2D(vertices[2], vertices[3]));
            if(faceProperties[LeftFace].Enabled) sides.Add(new FreeVector2D(vertices[3], vertices[0]));

            return sides;
        }

        public List<CollisionFaceProperties> GetFaceProperties()
        {
            return faceProperties.ToList();
        }

        public CollisionFaceProperties GetFaceProperties(Vector2D normal)
        {
            if(normal.X == 0)
            {
                if (normal.Y == 1) return faceProperties[TopFace];
                else if (normal.Y == -1) return faceProperties[BottomFace];
            }
            else if(normal.Y == 0)
            {
                if (normal.X == 1) return faceProperties[RightFace];
                else if (normal.X == -1) return faceProperties[LeftFace];
            }
            throw new ArgumentException("Normal given is not horizontal nor vertical");
        }

        public bool BelongsToBox(FreeVector2D side)
        {
            if (side.A.Y == side.B.Y) //Is horizontal
            {
                return side.A.Y == this.Bottom || side.A.Y == this.Top;
            }
            else if (side.A.X == side.B.X) //Is vertical
            {
                return side.A.X == this.Left || side.B.X == this.Right;
            }
            throw new ArgumentException("Side given is not horizontal nor vertical");
        }

        public override string ToString() => $"BoundingBox[X={X},Y={Y},Width={Width},Height={Height}]";

        public override bool Equals(object obj) => Equals(obj as CollisionBox);
        public bool Equals(CollisionBox other) => other != null && X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;

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
}
