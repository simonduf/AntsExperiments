using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants
{
    public struct Vector2i
    {
        public int x;
        public int y;

        public Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static readonly Vector2i Zero = new Vector2i(0, 0);
        public static readonly Vector2i Left = new Vector2i(-1, 0);
        public static readonly Vector2i Right = new Vector2i(1, 0);
        public static readonly Vector2i Up = new Vector2i(0, -1);
        public static readonly Vector2i Down = new Vector2i(0, 1);
        public static readonly Vector2i[] AllDirections = new Vector2i[] { Up, Down, Right, Left, Zero };


        public override bool Equals(object other)
        {
            Vector2i? vOther = other as Vector2i?;

            if (vOther == null)
                return false;

            return Equals((Vector2i)vOther);
        }
        public bool Equals(Vector2i other)
        {
            return x == other.x && y == other.y;
        }

        public static  Vector2i operator +(Vector2i a, Vector2i b)
        {
            a.x += b.x;
            a.y += b.y;
            return a;
        }

        public static Vector2i operator -(Vector2i a, Vector2i b)
        {
            a.x -= b.x;
            a.y -= b.y;
            return a;
        }

        public Vector2i Centered(Vector2i b)
        {
            Vector2i result = this;

            while (result.x < -b.x/2) result.x += b.x;
            while (result.y < -b.y/2) result.y += b.y;
            while (result.x > b.x / 2) result.x -= b.x;
            while (result.y > b.y / 2) result.y -= b.y;

            return result;
        }

        public static Vector2i Wrap(Vector2i v, int width, int height)
        {
            while (v.x < 0) v.x += width;
            while (v.x >= width) v.x -= width;
            while (v.y < 0) v.y += height;
            while (v.y >= height) v.y -= height;
            return v;
        }

        public static List<Vector2i> GenerateCoords(Vector2i min, Vector2i max, System.Func<Vector2i, bool> condition = null)
        {
            int width = max.x - min.x;
            int height = max.y - min.y;

            List<Vector2i> result = new List<Vector2i>(width * height);
            for (int j = min.y; j < max.y; j++)
            {
                for (int i = min.x; i < max.x; i++)
                {
                    Vector2i candidate = new Vector2i(i, j);
                    if (condition == null || condition(candidate))
                        result.Add(candidate);
                }
            }

            return result;
        }

        public static List<Vector2i> GenerateCircle(int radius2)
        {
            int squares = (int)Math.Floor(Math.Sqrt(radius2));

            return Vector2i.GenerateCoords(
                new Vector2i(-squares, -squares),
                new Vector2i(squares + 1, squares + 1),
                v => (v.x * v.x + v.y * v.y) <= radius2);
        }
    }
}
