using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateTile = Ants.Tile;

namespace Ants
{
    public class DistanceField
    {

        private struct Tile
        {
            public int distance;
            public bool isLand;
            public bool isLocked;
        }

        private static readonly Vector2i UP = new Vector2i(0, -1);
        private static readonly Vector2i DOWN = new Vector2i(0, 1);
        private static readonly Vector2i LEFT = new Vector2i(-1, 0);
        private static readonly Vector2i RIGHT = new Vector2i(1, 0);
        private static readonly Vector2i[] ALL_DIRECTIONS = new Vector2i[] { UP, DOWN, LEFT, RIGHT };

        private Vector2i[] coords;
        private Tile[,] map;
        private Tile[,] scratch;
        private int width;
        private int height;
        private Vector2i dims;
        private StateTile type;
        private PersistentGameState gameState;
        private bool terrain;


        public DistanceField(PersistentGameState gameState, StateTile type, bool terrain = false)
        {
            width = gameState.dimensions.x;
            height = gameState.dimensions.y;
            dims = gameState.dimensions;

            this.type = type;
            this.terrain = terrain;
            this.gameState = gameState;

            map = new Tile[width, height];
            scratch = new Tile[width, height];

            coords = new Vector2i[width * height];
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    coords[i + j * width] = new Vector2i(i, j);
                }
            }

        }


        public void UpdateLand()
        {
            foreach (var coord in coords)
            {
                map[coord.x, coord.y].isLand = (gameState.map[coord.x, coord.y].terrain != StateTile.Water);
            }
        }

        public void UpdateLocked()
        {
            
            foreach (var coord in coords)
            {
                int x = coord.x;
                int y = coord.y;

                bool locked = terrain ?
                               gameState.map[x, y].terrain == type :
                               gameState.map[x, y].objects == type;

                map[x, y].isLocked = locked;

                if(locked)
                    map[x, y].distance = 0;
            }
        }

        public void PropagateOnce()
        {
            foreach(var coord in coords)
            {
                Tile tile = map[coord.x, coord.y];

                scratch[coord.x, coord.y] = tile;

                if (!tile.isLocked && tile.isLand)
                {

                    int min = ALL_DIRECTIONS
                                .Select(direction => Wrap(coord + direction))
                                .Select(sum => map[sum.x, sum.y])
                                .Where(t => t.isLand)
                                .Select(t => t.distance)
                                .Min();

                    scratch[coord.x, coord.y].distance = min + 1;
                }

                
                
                
            }


            //
            //  Flip our scratch pad and our map
            //
            Tile[,] tmp = map;
            map = scratch;
            scratch = tmp;
        }

        public void Propagate(int count)
        {
            UpdateLand();
            UpdateLocked();
            for (int i = 0; i < count; i++)
                PropagateOnce();
        }


        public Vector2i Wrap(Vector2i v)
        {
            while (v.x < 0) v.x += width;
            while (v.x >= width) v.x -= width;
            while (v.y < 0) v.y += height;
            while (v.y >= height) v.y -= height;
            return v;
        }


        public IEnumerable<Vector2i> GetDescent(int x, int y)
        {
            Vector2i coord = new Vector2i(x, y);

            return ALL_DIRECTIONS
                                .Select(direction => Wrap(coord + direction))
                                .Where(sum => map[sum.x, sum.y].isLand)
                                .OrderBy(sum => map[sum.x, sum.y].distance)
                                .Select(r => (r - coord).Centered(dims))
                                .ToArray();
        }

        public IEnumerable<Vector2i> GetAscent(int x, int y)
        {
            Vector2i coord = new Vector2i(x, y);

            return ALL_DIRECTIONS
                                .Select(direction => Wrap(coord + direction))
                                .Where(sum => map[sum.x, sum.y].isLand)
                                .OrderByDescending(sum => map[sum.x, sum.y].distance)
                                .Select(r => (r - coord).Centered(dims))
                                .ToArray();

            
        }

        public int GetDistance(int x, int y)
        {
            return map[x, y].distance;
        }

    }
}
