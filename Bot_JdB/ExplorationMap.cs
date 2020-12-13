using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants
{
    public class ExplorationMap
    {
        private readonly int visitDelay;

        public struct Tile
        {
            public int timeSinceVisit;
            public bool isCritical;
            public bool needsVisit;
        }

        public struct Configuration
        {
            public int width;
            public int height;
            public int visitDelay;
            public int noCriticalStartup;
            public System.Func<Vector2i, bool> visibilityCheck;
        }

        public Tile[,] map;
        List<Vector2i> coords;

        private int width;
        private int height;
        private int enableCritical;
        private System.Func<Vector2i, bool> isVisible;

        public ExplorationMap(Configuration config)
        {
            width = config.width;
            height = config.height;
            visitDelay = config.visitDelay;
            enableCritical = -config.noCriticalStartup;
            isVisible = config.visibilityCheck;

            map = new Tile[width, height];
            coords = Vector2i.GenerateCoords(Vector2i.Zero, new Vector2i(width, height));

            Tile defaultTile;
            defaultTile.isCritical = false;
            defaultTile.needsVisit = false;
            defaultTile.timeSinceVisit = int.MaxValue;

            foreach (var c in coords)
                map[c.x, c.y] = defaultTile;

        }

        public void Update()
        {
            enableCritical++;
            foreach (var c in coords)
            {
                Tile tile = map[c.x, c.y];

                if (isVisible(c))
                {
                    tile.timeSinceVisit = 0;
                    tile.needsVisit = false;
                }
                else
                {
                    if(tile.timeSinceVisit < int.MaxValue)
                        tile.timeSinceVisit++;

                    if (tile.timeSinceVisit > visitDelay || (tile.isCritical && enableCritical > 0))
                        tile.needsVisit = true;

                }

                map[c.x, c.y] = tile;
            }
        }

        

        public void SetCriticalZone(Vector2i center, int radius2)
        {
            List<Vector2i> circle = Vector2i.GenerateCircle(radius2)
                                            .Select(v => Vector2i.Wrap(v + center, width, height))
                                            .ToList();

            foreach(Vector2i c in circle)
                map[c.x, c.y].isCritical = true;



        }
    }
}
