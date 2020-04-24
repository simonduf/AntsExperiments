using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Ants.Logger;

namespace Ants {

	public class MyBot : Bot {

        int FoodRadius = 20;
        int BattleRadius = 15;

        int turn = 0;
		public override void DoTurn (GameState state) {
            try
            {
                Log.Debug("Starting Turn " + turn++);// state.TimeRemaining
                //FoodRadius = (int)Math.Sqrt(state.ViewRadius2);


                //Calculate Enemy proximity
                var FoodProximity = calculateFoodProximity(state);
                var visibility = calculateVisibilityProximity(state);

                var antToMove = new List<Ant>(state.MyAnts);

                var antMoved = new List<Ant>();
                var ennemyConsidered = new List<Ant>();
                foreach (Ant enemy in state.EnemyAnts)
                {
                    if (ennemyConsidered.Contains(enemy))
                        continue;

                    if (Math.Sqrt(state.ViewRadius2) - visibility.At(enemy) < BattleRadius)
                    {
                        var battle = new Battle();
                        battle.StartBattle(state, enemy, state.MyAnts.OrderBy(a => state.GetDistance(enemy, a)).First());

                        ennemyConsidered.AddRange(battle.EnemyPlatoon);
                        antMoved.AddRange(battle.AllyPlatoon);

                    }
                }

                foreach (Ant ant in antToMove)
                {
                    // check if we have time left to calculate more orders
                    if (state.TimeRemaining < 10) break;



                    // General game signals
                    // Defend hill -> converge + sacrifice self
                    //continue if move done


                    if (getFood(state, FoodProximity, ant))
                        continue;


                    explore(state, visibility, ant);



                }
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
            }
			
		}



        private int[,] calculateAllyProximity(IGameState state, Ant ant)
        {
            return CalculateProximity(state, state.MyAnts.Where(x => x != ant).ToList());
        }

        private int[,] calculateVisibilityProximity(IGameState state)
        {
            state.CalculateVisibility();
            List<Location> notVisible = new List<Location>();
            for (int col = 0; col < state.Width; col++)
            {
                for (int row = 0; row < state.Height; row++)
                {
                    var loc = new Location( row, col);
                    if (!state.GetIsVisible(loc))
                        notVisible.Add(loc);
                }
            }

            return CalculateProximity(state, notVisible);
        }

        public static int[,] CalculateProximity<T>(IGameState state, List<T> startItems) where T: Location
        {
            //TODO do not create a new array each frame...

            var map = new int[state.Width, state.Height];
            map.Init(50);

            var queue = new Queue<Tuple<Location, int>>(startItems.Select(i => new Tuple<Location, int>(i, 0)).ToList());


            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                if (map[item.Item1.Col, item.Item1.Row] > item.Item2)
                {
                    map[item.Item1.Col, item.Item1.Row] = item.Item2;

                    foreach (Direction direction in Ants.Aim.Keys)
                    {
                        Location newLoc = state.GetDestination(item.Item1, direction);

                        if (state.GetIsPassable(newLoc))
                        {
                            queue.Enqueue(new Tuple<Location, int>(newLoc, item.Item2 + 1));
                        }
                    }
                }

            }
            return map;
        }

        private int[,] calculateFoodProximity(IGameState state)
        {
            return CalculateProximity(state, state.FoodTiles);
        }

        private bool getFood(GameState state, int[,] FoodProximity,  Ant ant)
        {
            if (FoodProximity[ant.Col, ant.Row] < FoodRadius)
            {
                int value = 999;
                Direction dir = Direction.North;
                foreach (Direction direction in Ants.Aim.Keys)
                {
                    Location newLoc = state.GetDestination(ant, direction);
                    if (state.GetIsPassable(newLoc) && !state.OccupiedNextRound.At(newLoc) && value > FoodProximity[newLoc.Col, newLoc.Row])
                    {
                        value = FoodProximity[newLoc.Col, newLoc.Row];
                        dir = direction;
                    }

                }

                IssueOrder(state, ant, dir);
                return true;
            }

            return false;
        }

        private bool explore(GameState state, int[,] visibility, Ant ant)
        {

            ////find the nearest undiscovered spot and go in that direction
            // TODO if two direction are in the way of the "unknown", select the one with the most spread with the other ants
            {

                int value = visibility[ant.Col, ant.Row];
                if (value < 40)
                {
                    Direction dir = Direction.North;
                    foreach (Direction direction in Ants.Aim.Keys)
                    {
                        Location newLoc = state.GetDestination(ant, direction);
                        if (state.GetIsPassable(newLoc) && !state.OccupiedNextRound.At(newLoc) && value > visibility[newLoc.Col, newLoc.Row])
                        {
                            value = visibility[newLoc.Col, newLoc.Row];
                            dir = direction;
                        }

                    }
                    if (value != visibility[ant.Col, ant.Row])
                    {
                        IssueOrder(state, ant, dir);
                        return true;
                    }
                }
            }


            //try distanciate other ants
            var AllyProximity = calculateAllyProximity(state, ant);
            if (AllyProximity[ant.Col, ant.Row] < 20)
            {
                int value = AllyProximity[ant.Col, ant.Row];
                Direction dir = Direction.North;
                foreach (Direction direction in Ants.Aim.Keys)
                {
                    Location newLoc = state.GetDestination(ant, direction);
                    if (state.GetIsPassable(newLoc) && !state.OccupiedNextRound.At(newLoc) && value < AllyProximity[newLoc.Col, newLoc.Row])
                    {
                        value = AllyProximity[newLoc.Col, newLoc.Row];
                        dir = direction;
                    }

                }
                if (value != AllyProximity[ant.Col, ant.Row])
                {
                    IssueOrder(state, ant, dir);
                    return true;
                }
            }

            ////try all the directions
            //foreach (Direction direction in Ants.Aim.Keys)
            //{

            //    // GetDestination will wrap around the map properly
            //    // and give us a new location
            //    Location newLoc = state.GetDestination(ant, direction);

            //    // GetIsPassable returns true if the location is land
            //    if (state.GetIsPassable(newLoc))
            //    {
            //        IssueOrder(ant, direction);
            //        // stop now, don't give 1 and multiple orders
            //        return true;
            //    }
            //}

            return false;
        }


    public static void Main (string[] args) {
			new Ants().PlayGame(new MyBot());
		}

	}


    public class Battle
    {
        const int platoonRadius = 10;
        const int GuardDistance = 2;

        public List<Ant> AllyPlatoon = new List<Ant>();
        public List<Ant> EnemyPlatoon = new List<Ant>();

        public void BuildPlatoon(IGameState state, List<Ant> platoonToFill, Ant startingAnt)
        {

        }

        //Return the distance to ennemy for each ant in the list
        public List<int> BuildFrontline(IGameState state, List<Ant> platoon, List<Ant> Enemies, out int qty, out int distance)
        {
            var DistanceToFrontline = new List<int>(platoon.Count);

            for (int i = 0; i < platoon.Count; i++)
            {
                DistanceToFrontline[i] = Enemies.Select(enemy => state.GetDistance(platoon[i], enemy)).Min();
            }

            var _distance = DistanceToFrontline.Min();
            qty = DistanceToFrontline.Count(x => x == _distance);

            distance = _distance;

            return DistanceToFrontline;
        }

        public void StartBattle(GameState state, Ant AllyAnt, Ant EnemyAnt)
        {
            AllyPlatoon.Clear();
            EnemyPlatoon.Clear();
            BuildPlatoon(state, AllyPlatoon, AllyAnt);
            BuildPlatoon(state, EnemyPlatoon, EnemyAnt);

            if (AllyPlatoon.Count > EnemyPlatoon.Count)
            {
                //Attack!
                var AllyDist = BuildFrontline(state, AllyPlatoon, EnemyPlatoon, out int qty, out int distance);

                var EnemyDist = BuildFrontline(state, AllyPlatoon, EnemyPlatoon, out int Eqty, out int Edistance);


                if (Eqty > qty)
                {
                    //We have more ants, but they are not on the front line!
                    //Regroup!


                    if (AllyDist.Where(x => x == distance + 1).Count() > Eqty)
                    {
                        //At the end of the turn, we will be in position, no need to move back

                        Regroup(state, AllyDist.ToArray(), distance);

                    }
                    else
                    {
                        //Maybe we should move back our units...
                        //TODO backup frot row instead
                        Regroup(state, AllyDist.ToArray(), distance);
                    }


                }
                //else we just stand there for now
            }
            else
            {
                //retreat
                //foreach (var ant in AllyPlatoon)
                //{

                //}

            }


        }

        public void Regroup(GameState state, int[] distances, int fallbackDistance)
        {
            var EnemyDist = MyBot.CalculateProximity(state, EnemyPlatoon);
            for (int i = 0; i < AllyPlatoon.Count; i++)
            {
                if (distances[i] <= fallbackDistance)// distance should be the min...
                    continue;

                foreach (Direction direction in Ants.Aim.Keys)
                {
                    Location newLoc = state.GetDestination(AllyPlatoon[i], direction);

                    if (state.GetIsPassable(newLoc) && !state.OccupiedNextRound.At(newLoc) && EnemyDist.At(newLoc) < distances[i])
                    {
                        Bot.IssueOrder(state, AllyPlatoon[i], direction);
                        continue;
                    }
                }
            }
        }

    }


	
}