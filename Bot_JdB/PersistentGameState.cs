using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StateTile = Ants.Tile;


namespace Ants
{
    public class PersistentGameState
    {
        public struct Tile
        {
            public StateTile terrain;
            public StateTile objects;
        }

        public Tile[,] map;
        public Vector2i[] coords;
        public Vector2i dimensions;

        public PersistentGameState(int width, int height)
        {
            map = new Tile[width, height];
            dimensions = new Vector2i(width, height);

            coords = new Vector2i[width * height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    coords[i + j * width] = new Vector2i(i, j);
                }
            }

            Tile defaultTile;
            defaultTile.terrain = StateTile.Unseen;
            defaultTile.objects = StateTile.Unseen;
            foreach (var coord in coords)
                map[coord.x, coord.y] = defaultTile;

        }



        private Location location = new Location(0, 0);
        public void Update(StateTile[,] state)
        {
            
            
            foreach (var coord in coords)
            {
                Tile tile = map[coord.x, coord.y];
                StateTile s = state[coord.y, coord.x];


                bool canChange = tile.terrain == StateTile.Unseen ||
                                    tile.terrain == StateTile.MyHill ||
                                    tile.terrain == StateTile.TheirHill;
                //
                //  Update unknown terrain. Either it's water or land. If there's
                //  a special object, then it is also land.
                //
                if (canChange && s != StateTile.Unseen)
                {
                    tile.terrain = GetTerrainEquivalent(s);
                }

                tile.objects = s;

                map[coord.x, coord.y] = tile;
            }
        }


        private StateTile GetTerrainEquivalent(StateTile tile)
        {
            switch(tile)
            {
                case StateTile.Water:
                    return StateTile.Water;
                
                case StateTile.TheirHill:
                    return StateTile.TheirHill;

                case StateTile.MyHill:
                    return StateTile.MyHill;

                default:
                    return StateTile.Land;

            }
        }


        public String ToString(int x, int y, int width, int height)
        {
            StringBuilder builder = new StringBuilder("Persistent Game State: \n");
            for (int j = y; j < y + height; j++)
            {
                for (int i = x; i < x + width; i++)
                {
                    var tile = map[i, j];
                    builder.Append((int)tile.terrain + "\t");
                }
                builder.Append("\n");
            }

            return builder.ToString();
        }

        public override String ToString()
        {
            return ToString(0, 0, dimensions.x, dimensions.y);
        }

    }
}
