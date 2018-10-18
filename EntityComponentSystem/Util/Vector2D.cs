using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Saves;

namespace EntityComponentSystem.Util
{
    [PersistentObject]
    public struct Vector2D : IEquatable<Vector2D>
    {
        public const int Rounding = 7;

        public static readonly Vector2D Empty = new Vector2D();
        public static readonly Vector2D UnitI = new Vector2D(1, 0);
        public static readonly Vector2D UnitJ = new Vector2D(0, 1);

        [PersistentProperty]
        public double X { get; set; }
        [PersistentProperty]
        public double Y { get; set; }

        public double Angle => Math.Atan2(Y, X);
        public double Magnitude => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));

        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector2D Normalize() => this == Empty ? Empty : this / Magnitude;

        public Vector2D Rotate(double rad)
        {
            double angle = this.Angle;
            double magnitude = this.Magnitude;
            return new Vector2D(Math.Round(magnitude * Math.Cos(angle + rad), Rounding), Math.Round(magnitude * Math.Sin(angle + rad), Rounding));
        }

        public static Vector2D operator +(Vector2D a, Vector2D b) => new Vector2D(a.X + b.X, a.Y + b.Y);

        public static Vector2D operator -(Vector2D a, Vector2D b) => new Vector2D(a.X - b.X, a.Y - b.Y);

        public static Vector2D operator -(Vector2D a) => new Vector2D(-a.X, -a.Y);

        public static Vector2D operator *(Vector2D a, double b) => new Vector2D(b * a.X, b * a.Y);

        public static Vector2D operator *(double b, Vector2D a) => new Vector2D(b * a.X, b * a.Y);

        public static Vector2D operator /(Vector2D a, double b) => new Vector2D(a.X / b, a.Y / b);

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public override bool Equals(object obj) => obj is Vector2D && Equals((Vector2D)obj);
        public bool Equals(Vector2D other) => X == other.X && Y == other.Y;

        public static bool operator ==(Vector2D a, Vector2D b) => a.Equals(b);
        public static bool operator !=(Vector2D a, Vector2D b) => !a.Equals(b);

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public void Round()
        {
            X = Math.Round(X, Rounding);
            Y = Math.Round(Y, Rounding);
        }

        public Vector2D GetRounded()
        {
            return new Vector2D(Math.Round(X, Rounding), Math.Round(Y, Rounding));
        }
    }
}
