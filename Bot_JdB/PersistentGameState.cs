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
        


        public void Update(StateTile[,] state)
        {
            foreach(var coord in coords)
            {
                Tile tile = map[coord.x, coord.y];
                StateTile s = state[coord.y, coord.x];

                //
                //  Update unknown terrain. Either it's water or land. If there's
                //  a special object, then it is also land.
                //
                if (tile.terrain == StateTile.Unseen && s != StateTile.Unseen)
                    tile.terrain = (s == StateTile.Water) ? StateTile.Water : StateTile.Land;

                tile.objects = s;

                map[coord.x, coord.y] = tile;
            }
        }


    }
}
