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
    }
}
