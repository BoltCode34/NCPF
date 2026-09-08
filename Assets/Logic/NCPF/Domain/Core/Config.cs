using Core.Foundation;
using System;

namespace NCPF.Domain
{
    /// <summary>
    /// A node of the discrete lattice: cell (X, Y) plus a heading channel index Angle.
    /// Angle is a channel index, not degrees: arithmetic never wraps it modulo the
    /// channel count, that stays the caller's job.
    /// </summary>
    [Serializable]
    public struct Config
    {
        public int X;
        public int Y;
        public int Angle;
        public Int2 Cell => new Int2(X, Y);
        public Config(int x, int y, int angle)
        {
            X = x;
            Y = y;
            Angle = angle;
        }
        public Config(Int2 pos, int angle)
        {
            X = pos.x;
            Y = pos.y;
            Angle = angle;
        }

        public static Config operator +(Config left, Config right)
            => new Config(left.X + right.X, left.Y + right.Y, left.Angle + right.Angle);

        public static Config operator -(Config left, Config right)
            => new Config(left.X - right.X, left.Y - right.Y, left.Angle - right.Angle);

        public static Config operator -(Config value)
            => new Config(-value.X, -value.Y, -value.Angle);

        public static Config operator *(Config value, int scale)
            => new Config(value.X * scale, value.Y * scale, value.Angle * scale);

        public static Config operator *(int scale, Config value)
            => value * scale;

        public static Config operator /(Config value, int divisor)
            => new Config(value.X / divisor, value.Y / divisor, value.Angle / divisor);

        public static bool operator ==(Config left, Config right)
        {
            return left.X == right.X && left.Y == right.Y && left.Angle == right.Angle;
        }

        public static bool operator !=(Config left, Config right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Config other)
            {
                return this == other;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Angle.GetHashCode();
        }
    }
}
