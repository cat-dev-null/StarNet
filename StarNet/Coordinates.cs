using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarNet
{
    public struct Coordinates3D
    {
        public int X, Y, Z;

        public Coordinates3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Coordinates3D operator -(Coordinates3D a)
        {
            return new Coordinates3D(-a.X, -a.Y, -a.Z);
        }

        public static Coordinates3D operator +(Coordinates3D a, Coordinates3D b)
        {
            return new Coordinates3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Coordinates3D operator -(Coordinates3D a, Coordinates3D b)
        {
            return new Coordinates3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Coordinates3D operator *(Coordinates3D a, Coordinates3D b)
        {
            return new Coordinates3D(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Coordinates3D operator /(Coordinates3D a, Coordinates3D b)
        {
            return new Coordinates3D(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static Coordinates3D operator %(Coordinates3D a, Coordinates3D b)
        {
            return new Coordinates3D(a.X % b.X, a.Y % b.Y, a.Z % b.Z);
        }

        public static bool operator !=(Coordinates3D a, Coordinates3D b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Coordinates3D a, Coordinates3D b)
        {
            return a.Equals(b);
        }

        public static Coordinates3D operator +(Coordinates3D a, int b)
        {
            return new Coordinates3D(a.X + b, a.Y + b, a.Z + b);
        }

        public static Coordinates3D operator -(Coordinates3D a, int b)
        {
            return new Coordinates3D(a.X - b, a.Y - b, a.Z - b);
        }

        public static Coordinates3D operator *(Coordinates3D a, int b)
        {
            return new Coordinates3D(a.X * b, a.Y * b, a.Z * b);
        }

        public static Coordinates3D operator /(Coordinates3D a, int b)
        {
            return new Coordinates3D(a.X / b, a.Y / b, a.Z / b);
        }

        public static Coordinates3D operator %(Coordinates3D a, int b)
        {
            return new Coordinates3D(a.X % b, a.Y % b, a.Z % b);
        }

        public double DistanceTo(Coordinates3D other)
        {
            return Math.Sqrt(Square((double)other.X - (double)X) +
                             Square((double)other.Y - (double)Y) +
                             Square((double)other.Z - (double)Z));
        }

        private double Square(double num)
        {
            return num * num;
        }

        public override string ToString()
        {
            return string.Format("<{0},{1},{2}>", X, Y, Z);
        }

        public bool Equals(Coordinates3D other)
        {
            return other.X.Equals(X) && other.Y.Equals(Y) && other.Z.Equals(Z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Coordinates3D)) return false;
            return Equals((Coordinates3D)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = X.GetHashCode();
                result = (result * 397) ^ Y.GetHashCode();
                result = (result * 397) ^ Z.GetHashCode();
                return result;
            }
        }

        public static readonly Coordinates3D Zero = new Coordinates3D(0, 0, 0);
        public static readonly Coordinates3D One = new Coordinates3D(1, 1, 1);

        public static readonly Coordinates3D Up = new Coordinates3D(0, 1, 0);
        public static readonly Coordinates3D Down = new Coordinates3D(0, -1, 0);
        public static readonly Coordinates3D Left = new Coordinates3D(-1, 0, 0);
        public static readonly Coordinates3D Right = new Coordinates3D(1, 0, 0);
        public static readonly Coordinates3D Backwards = new Coordinates3D(0, 0, -1);
        public static readonly Coordinates3D Forwards = new Coordinates3D(0, 0, 1);

        public static readonly Coordinates3D East = new Coordinates3D(1, 0, 0);
        public static readonly Coordinates3D West = new Coordinates3D(-1, 0, 0);
        public static readonly Coordinates3D North = new Coordinates3D(0, 0, -1);
        public static readonly Coordinates3D South = new Coordinates3D(0, 0, 1);
    }
}
