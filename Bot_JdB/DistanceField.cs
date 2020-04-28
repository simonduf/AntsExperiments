using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateTile = Ants.Tile;

namespace Ants
{
    public class DistanceField
    {
        public const int Max = int.MaxValue-5;
        private struct Tile
        {
            public int distance;
            public bool isLand;
            public bool isLocked;
        }


        

        private Vector2i[] coords;
        private Tile[,] map;
        private Tile[,] scratch;
        private int width;
        private int height;
        private Vector2i dims;
        
        private GameState gameState;
        System.Func<GameState.Tile, bool> marker;


        public DistanceField(GameState gameState, System.Func<GameState.Tile, bool> marker)
        {
            width = gameState.Width;
            height = gameState.Height;
            dims = new Vector2i(width, height);

            this.marker = marker;
            this.gameState = gameState;

            map = new Tile[width, height];
            scratch = new Tile[width, height];

            coords = new Vector2i[width * height];
            for(int j = 0; j < height; j++)
            {
                for(int i = 0; i < width; i++)
                {
                    coords[i + j * width] = new Vector2i(i, j);
                    map[i, j].distance = Max;
                }
            }

        }


        public String ToString(int x, int y, int width, int height)
        {
            StringBuilder builder = new StringBuilder("Distance Field: \n");
            for (int j = y; j < y+height; j++)
            {
                for (int i = x; i < x+width; i++)
                {
                    var tile = map[i, j];
                    if(tile.isLocked)
                        builder.Append("X\t");
                    else if(!tile.isLand)
                        builder.Append("--\t");
                    else
                        builder.Append(map[i, j].distance + "\t");
                }
                builder.Append("\n");
            }

            return builder.ToString();
        }

        public override String ToString()
        {
            return ToString(0,0,width,height);
        }


        public void UpdateLand()
        {
            foreach (var coord in coords)
            {
                map[coord.x, coord.y].isLand = gameState.map[coord.x, coord.y].terrain != GameState.Terrain.Water;
            }
        }

        public void UpdateLocked()
        {
            int count = 0;

            foreach (var coord in coords)
            {
                int x = coord.x;
                int y = coord.y;

                bool locked = marker(gameState.map[coord.x, coord.y]);

                map[x, y].isLocked = locked;

                if (locked)
                {
                    map[x, y].distance = 0;
                    count++;
                }

            }

            if(count == 0)
            {
                foreach (var coord in coords)
                    map[coord.x, coord.y].distance = Max;
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

                    int min = Vector2i.AllDirections
                                .Select(direction => Wrap(coord + direction))
                                .Select(sum => map[sum.x, sum.y])
                                .Where(t => t.isLand)
                                .Select(t => t.distance)
                                .Min();

                    scratch[coord.x, coord.y].distance = Math.Min(min + 1, Max);
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
            return Vector2i.Wrap(v, width, height);
        }


        public IEnumerable<Vector2i> GetDescent(int x, int y)
        {
            Vector2i coord = new Vector2i(x, y);

            return Vector2i.AllDirections
                                .Select(direction => Wrap(coord + direction))
                                .Where(sum => map[sum.x, sum.y].isLand)
                                .OrderBy(sum => map[sum.x, sum.y].distance)
                                .Select(r => (r - coord).Centered(dims))
                                .ToArray();
        }

        public IEnumerable<Vector2i> GetAscent(int x, int y)
        {
            Vector2i coord = new Vector2i(x, y);

            return Vector2i.AllDirections
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
