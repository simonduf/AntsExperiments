using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Ants
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x; this.y = y;
        }

        


        public float magnitude
        {
            get
            {
                return (float)Math.Sqrt((double)(x * x + y * y));
            }
        }

        public float magSquared
        {
            get
            {
                return x * x + y * y;
            }
        }

        public void Normalize()
        {
            float _magnitude = this.magnitude;

            x /= _magnitude;
            y /= _magnitude;
        }

        public static Vector2 zero { get { return new Vector2(0, 0); } }
        public static Vector2 west { get { return new Vector2(0, 0); } }
        public static Vector2 north { get { return new Vector2(0, 0); } }
        public static Vector2 east { get { return new Vector2(0, 0); } }
        public static Vector2 south { get { return new Vector2(0, 0); } }

        

    }
}
