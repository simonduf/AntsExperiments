using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ants
{
    public class TacticalMap
    {
        public struct Tile
        {
            public bool isLand;
            public Vector2i position;
            public List<EnemyPresence> enemies;
        }

        public struct EnemyPresence
        {
            public float probability;
            public int enemyId;
        }


        private List<Vector2i> coords;
        private Tile[,] map;
        public readonly int Width;
        public readonly int Height;


        private struct StampTile
        {
            public Vector2i offset;
            public float probability;
        }

        private StampTile[] stamp;


        public Tile this[int x, int y]
        {
            get
            {
                return map[x, y];
            }
        }

        
        public TacticalMap(int width, int height, int attackRadius2)
        {
            Width = width;
            Height = height;

            map = new Tile[width, height];
            coords = Vector2i.GenerateCoords(Vector2i.Zero, new Vector2i(width, height));

            foreach(var c in coords)
            {
                Tile tile;
                tile.enemies = new List<EnemyPresence>();
                tile.position = c;
                tile.isLand = true;
                map[c.x, c.y] = tile;
            }

            stamp = BuildStamp(attackRadius2);
        }

        private static StampTile[] BuildStamp(int attackRadius2)
        {
            List<Vector2i> primaryStamp = Vector2i.GenerateCircle(attackRadius2);

            List<StampTile> lStamp = new List<StampTile>();

            List<Vector2i> stampDecomp = primaryStamp
                            .SelectMany(s =>
                            {
                                return Vector2i.AllDirections.Select(d => d + s);
                            }).ToList();

            foreach (Vector2i position in stampDecomp)
            {
                int i = lStamp.FindIndex(s => s.offset.Equals(position));

                if (i >= 0)
                {
                    StampTile tile = lStamp[i];
                    tile.probability += 0.2f;
                    lStamp[i] = tile;
                }
                else
                {
                    StampTile tile;
                    tile.offset = position;
                    tile.probability = 0.2f;
                    lStamp.Add(tile);
                }
            }
            return lStamp.ToArray();
        }


        public void SetLand(GameState state)
        {
            foreach(var c in coords)
                map[c.x, c.y].isLand = state.map[c.x, c.y].terrain != GameState.Terrain.Water;
        }


        public void AddEnemy(Vector2i position, int id)
        {
            foreach(StampTile s in stamp)
            {
                Vector2i pos = Vector2i.Wrap(position + s.offset, Width, Height);

                EnemyPresence enemy;
                enemy.enemyId = id;
                enemy.probability = s.probability;

                map[pos.x, pos.y].enemies.Add(enemy);
            }
        }

        public void Reset()
        {
            foreach (var c in coords)
                map[c.x, c.y].enemies.Clear();
        }


    }
}
