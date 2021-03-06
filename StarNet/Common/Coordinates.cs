using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarNet.Common
{
    public struct Coordinates2D
    {
        public int X, Y;

        public Coordinates2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Coordinates2D operator -(Coordinates2D a)
        {
            return new Coordinates2D(-a.X, -a.Y);
        }

        public static Coordinates2D operator +(Coordinates2D a, Coordinates2D b)
        {
            return new Coordinates2D(a.X + b.X, a.Y + b.Y);
        }

        public static Coordinates2D operator -(Coordinates2D a, Coordinates2D b)
        {
            return new Coordinates2D(a.X - b.X, a.Y - b.Y);
        }

        public static Coordinates2D operator *(Coordinates2D a, Coordinates2D b)
        {
            return new Coordinates2D(a.X * b.X, a.Y * b.Y);
        }

        public static Coordinates2D operator /(Coordinates2D a, Coordinates2D b)
        {
            return new Coordinates2D(a.X / b.X, a.Y / b.Y);
        }

        public static Coordinates2D operator %(Coordinates2D a, Coordinates2D b)
        {
            return new Coordinates2D(a.X % b.X, a.Y % b.Y);
        }

        public static bool operator !=(Coordinates2D a, Coordinates2D b)
        {
            return !a.Equals(b);
        }

        public static bool operator ==(Coordinates2D a, Coordinates2D b)
        {
            return a.Equals(b);
        }

        public static Coordinates2D operator +(Coordinates2D a, int b)
        {
            return new Coordinates2D(a.X + b, a.Y + b);
        }

        public static Coordinates2D operator -(Coordinates2D a, int b)
        {
            return new Coordinates2D(a.X - b, a.Y - b);
        }

        public static Coordinates2D operator *(Coordinates2D a, int b)
        {
            return new Coordinates2D(a.X * b, a.Y * b);
        }

        public static Coordinates2D operator /(Coordinates2D a, int b)
        {
            return new Coordinates2D(a.X / b, a.Y / b);
        }

        public static Coordinates2D operator %(Coordinates2D a, int b)
        {
            return new Coordinates2D(a.X % b, a.Y % b);
        }

        public double DistanceTo(Coordinates2D other)
        {
            return Math.Sqrt(Square((double)other.X - (double)X) +
                             Square((double)other.Y - (double)Y));
        }

        private double Square(double num)
        {
            return num * num;
        }

        public override string ToString()
        {
            return string.Format("<{0},{1}>", X, Y);
        }

        public bool Equals(Coordinates2D other)
        {
            return other.X.Equals(X) && other.Y.Equals(Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Coordinates2D)) return false;
            return Equals((Coordinates2D)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = X.GetHashCode();
                result = (result * 397) ^ Y.GetHashCode();
                return result;
            }
        }

        public static readonly Coordinates2D Zero = new Coordinates2D(0, 0);
        public static readonly Coordinates2D One = new Coordinates2D(1, 1);

        public static readonly Coordinates2D Up = new Coordinates2D(0, 1);
        public static readonly Coordinates2D Down = new Coordinates2D(0, -1);
        public static readonly Coordinates2D Left = new Coordinates2D(-1, 0);
        public static readonly Coordinates2D Right = new Coordinates2D(1, 0);
    }
}
