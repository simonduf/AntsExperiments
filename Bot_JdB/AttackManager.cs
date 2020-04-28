using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Ants
{
    public class AttackManager
    {
        TacticalMap map;
        AttackGraph graph = new AttackGraph();

        List<Ant> ours = new List<Ant>(100);
        List<Ant> theirs;


        public AttackManager(GameState state)
        {
            map =  new TacticalMap(state.Width, state.Height, state.AttackRadius2);
        }

        public void MoveOffensive(GameState state)
        {
            //
            //  Make a list of enemy ants
            //
            theirs = state.EnemyAnts;

            //
            //  Update tactical map
            //
            map.Reset();
            map.SetLand(state);
            for (int i = 0; i < theirs.Count; i++)
                map.AddEnemy(theirs[i].position, i);


            //
            //  Make a list of offensive friendly ants
            //
            ours.Clear();
            foreach(Ant ant in state.MyAnts)
            {
                bool isInBattle = Vector2i.AllDirections
                        .Select(d => Vector2i.Wrap(ant.position + d, map.Width, map.Height))
                        .Select(coord => map[coord.x, coord.y].enemies.Count)
                        .Where(count => count > 0)
                        .Any();

                if (isInBattle)
                    ours.Add(ant);
            }
            

            //
            //  Build a node graph, identifying which ants can attack which ants
            //
            graph.Clear();
            BuildGraph(graph, ours, theirs, map);


            //
            //  Optimize the node graph 
            //
            MaximizeEngagement();
            for (int i = 0; i < 2; i++)
                OptimizeOnce();


            //
            //  Move ants in most optimal way
            //
            for(int i = 0; i < ours.Count; i++)
            {
                ours[i].direction = graph.GetDirection(i);
                ours[i].hasMoved = true;
            }


        }

        private void BuildGraph(AttackGraph graph, List<Ant> ours, List<Ant> theirs, TacticalMap map)
        {
            for (int i = 0; i < ours.Count; i++)
            {
                Ant myAnt = ours[i];

                //
                //  Get all tiles on tactical map that our ant can navigate on
                //
                IEnumerable<TacticalMap.Tile> tiles =
                                        Vector2i.AllDirections
                                            .Select(direction => Vector2i.Wrap(myAnt.position + direction, map.Width, map.Height))
                                            .Select(p => map[p.x, p.y]);
                //
                //  Identify enemies
                //
                IEnumerable<int> enemies = IdentifyEnemies(tiles);

                foreach (int enemyId in enemies)
                    graph.AddNodeGroup(i, enemyId, BuildLinksAssociatedToEnemy(enemyId, tiles));


            }
        }

        private List<AttackGraph.LinkOption> BuildLinksAssociatedToEnemy(int enemyId, IEnumerable<TacticalMap.Tile> tiles)
        {
            List<AttackGraph.LinkOption> options = new List<AttackGraph.LinkOption>();

            foreach (var tile in tiles)
            {
                AttackGraph.LinkOption option;

                option.destination = tile.position;
                option.available = tile.isLand;
                option.probability = tile.enemies
                                        .Where(e => e.enemyId == enemyId)
                                        .Select(e => e.probability)
                                        .SingleOrDefault();

                options.Add(option);
            }

            return options;
        }

        private IEnumerable<int> IdentifyEnemies(IEnumerable<TacticalMap.Tile> tiles)
        {
            return tiles.SelectMany(tile => tile.enemies).Select(e => e.enemyId).Distinct();
        }


        private void MaximizeEngagement()
        {
            for(int i = 0; i < ours.Count; i++)
            {
                Direction best = Direction.Halt;
                float maxEngagement = float.MinValue;

                foreach(Direction direction in DirectionExtensions.AllDirections)
                {
                    graph.SetDirection(i, direction);
                    float engagement = graph.GetEngagement(i);
                    if(engagement > maxEngagement && IsPositionValid(i))
                    {
                        maxEngagement = engagement;
                        best = direction;
                    }
                }

                graph.SetDirection(i, best);
            }
        }

        
        private void OptimizeOnce()
        {
            for(int i = 0; i < ours.Count; i++)
            {
                Direction best = graph.GetDirection(i);
                float maxValue = graph.GetValue();

                foreach(Direction direction in DirectionExtensions.AllDirections)
                {
                    //
                    //  Test for overlap
                    //
                    graph.SetDirection(i, direction);

                    if (!IsPositionValid(i))
                        continue;
                    
                    float value = graph.GetValue();

                    if(value > maxValue)
                    {
                        maxValue = value;
                        best = direction;
                    }
                }

                graph.SetDirection(i, best);

            }
        }

        private bool IsPositionValid(int id)
        {
            Vector2i position = graph.GetProjectedPosition(id);
            bool available = true;
            for (int j = 0; j < ours.Count; j++)
            {
                if (j == id)
                    continue;

                if (graph.GetProjectedPosition(j).Equals(position))
                {
                    available = false;
                    break;
                }
            }
            return available;
        }

        

    }
}
