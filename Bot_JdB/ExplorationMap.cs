using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants
{
    public class ExplorationMap
    {
        public int[,] map;
        List<Vector2i> coords;

        public ExplorationMap(int width, int height)
        {
            map = new int[width, height];
            coords = Vector2i.GenerateCoords(Vector2i.Zero, new Vector2i(width, height));

            foreach (var c in coords)
                map[c.x, c.y] = int.MaxValue;

        }

        public void Increment()
        {
            foreach(var c in coords)
            {
                if (map[c.x, c.y] < int.MaxValue)
                    map[c.x, c.y]++;
            }
        }

        public void ZeroOutVisible(System.Func<Vector2i, bool> isVisible)
        {
            foreach(var c in coords)
            {
                if (isVisible(c))
                    map[c.x, c.y] = 0;
            }
        }
    }
}
