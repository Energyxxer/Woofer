using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityComponentSystem.Util
{
    public struct FreeVector2D : IEquatable<FreeVector2D>
    {
        public Vector2D A;
        public Vector2D B;

        public double Magnitude => (B - A).Magnitude;
        public double Angle => (B - A).Angle;
        public Vector2D Normal => (B - A).Rotate(Math.PI / 2).Normalize();

        public FreeVector2D(Vector2D b) : this(new Vector2D(), b)
        {
        }

        public FreeVector2D(Vector2D a, Vector2D b)
        {
            A = a;
            B = b;
        }

        public FreeVector2D Rotate(double rad)
        {
            return new FreeVector2D(A.Rotate(rad), B.Rotate(rad));
        }

        public Vector2D BoundToOrigin()
        {
            return B - A;
        }

        public bool Contains(Vector2D point)
        {
            return Math.Round(Magnitude, 8) == Math.Round((A - point).Magnitude + (B - point).Magnitude, 8);
        }

        public bool IntersectsWith(FreeVector2D other)
        {
            if (other.Angle == this.Angle) return false;

            {
                FreeVector2D thisRotated = this.Rotate(-this.Angle);
                FreeVector2D otherRotated = other.Rotate(-this.Angle);

                if (otherRotated.A.Y > thisRotated.A.Y == otherRotated.B.Y > thisRotated.A.Y) return false;
            }
            {
                FreeVector2D thisRotated = this.Rotate(-other.Angle);
                FreeVector2D otherRotated = other.Rotate(-other.Angle);

                if (thisRotated.A.Y > otherRotated.A.Y == thisRotated.B.Y > otherRotated.A.Y) return false;
            }
            return true;
        }

        public Vector2D? GetIntersection(FreeVector2D other)
        {
            if (other.Angle == this.Angle) return null;
            FreeVector2D thisRotated = this.Rotate(-this.Angle);
            FreeVector2D otherRotated = other.Rotate(-this.Angle);

            double yOff = thisRotated.A.Y - otherRotated.A.Y;
            double xOff = yOff / Math.Tan(otherRotated.Angle);

            double intersectX = otherRotated.A.X + xOff;
            double intersectY = otherRotated.A.Y + yOff;

            Vector2D intersection = new Vector2D(intersectX, intersectY);
            if (thisRotated.Contains(intersection) && otherRotated.Contains(intersection))
            {
                return intersection.Rotate(this.Angle);
            }
            return null;
        }

        public Vector2D GetMiddle()
        {
            return new Vector2D((A.X + B.X) / 2, (A.Y + B.Y) / 2);
        }

        public void SwapEnds()
        {
            Vector2D oldB = B;
            B = A;
            A = oldB;
        }

        public override bool Equals(object obj) => obj is FreeVector2D && Equals((FreeVector2D)obj);
        public bool Equals(FreeVector2D other) => (A.Equals(other.A) && B.Equals(other.B)) || (A.Equals(other.B) && B.Equals(other.A));

        public override int GetHashCode()
        {
            var hashCode = -1817952719;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(A);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2D>.Default.GetHashCode(B);
            return hashCode;
        }

        public override string ToString() => $"[{A}, {B}]";
        public bool HasVertex(Vector2D vertex)
        {
            return A.Equals(vertex) || B.Equals(vertex);
        }
        public bool SharesVertex(FreeVector2D other)
        {
            return this.HasVertex(other.A) || this.HasVertex(other.B);
        }
    }
}
