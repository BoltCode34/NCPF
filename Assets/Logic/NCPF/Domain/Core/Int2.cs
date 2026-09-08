using UnityEngine;

namespace Core.Foundation
{
    public struct Int2
    {
        public int x { get; set; }
        public int y { get; set; }

        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Int2 operator +(Int2 a, Int2 b)
            => new Int2(a.x + b.x, a.y + b.y);

        public static Int2 operator -(Int2 a, Int2 b)
            => new Int2(a.x - b.x, a.y - b.y);
        public static bool operator ==(Int2 a, Int2 b)
            => a.x == b.x && a.y == b.y;
        public static bool operator !=(Int2 a, Int2 b)
            => a.x != b.x|| a.y != b.y;
        public static implicit operator Vector2Int(Int2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }
        public static implicit operator Int2(Vector2Int v)
        {
            return new Int2(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }
        public override bool Equals(object obj)
            => obj is Int2 other && x == other.x && y == other.y;

        public override int GetHashCode()
            => (x, y).GetHashCode();

        public override string ToString()
            => $"({x}, {y})";
        public static Int2 zero = new Int2(0, 0);
        public static Int2 one = new Int2(1, 1);
        public static Int2 right = new Int2(0, 1);
        public static Int2 up = new Int2(1, 0);
    }
}