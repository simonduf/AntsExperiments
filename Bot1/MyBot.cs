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


                //Calculate proximity
                var FoodProximity = calculateFoodProximity(state);
                var visibility = calculateVisibilityProximity(state);


                var antMoved = new List<Ant>();
                var ennemyConsidered = new List<Ant>();


                //Attack/defend
                foreach (Ant enemy in state.EnemyAnts)
                {
                    if (ennemyConsidered.Contains(enemy))
                        continue;

                    if (Math.Sqrt(state.ViewRadius2) - visibility.At(enemy) < BattleRadius)
                    {
                        var battle = new Battle();
                        battle.StartBattle(state, state.MyAnts.OrderBy(a => state.GetDistance(enemy, a)).First(), enemy);

                        ennemyConsidered.AddRange(battle.EnemyPlatoon);
                        antMoved.AddRange(battle.AllyPlatoon);

                    }
                }

                var antToMove = state.MyAnts.Except(antMoved).ToList();



                //Food.getAnt
                foreach (var food in state.FoodTiles)
                {
                    var result = FindClosest(state, food, x => antToMove.Select(ant => (x.Row == ant.Row && x.Col == ant.Col) ? ant:null).Where( y => y!=null).FirstOrDefault(), out Direction dir);
                    if (result != null)
                    {
                        dir = dir.Opposite();
                        Location newLoc = state.GetDestination(result, dir);
                        if(!state.OccupiedNextRound.At(newLoc))
                            IssueOrder(state, result, dir);
                        antMoved.Add(result);
                        antToMove.Remove(result);
                    }

                }


                antToMove = state.MyAnts.Except(antMoved).ToList();
                foreach (Ant ant in antToMove)
                {
                    // check if we have time left to calculate more orders
                    if (state.TimeRemaining < 10) break;



                    // General game signals
                    // Defend hill -> converge + sacrifice self
                    //continue if move done


                    //if (getFood(state, FoodProximity, ant)) //Replace par food.getAnt() above
                    //    continue;


                    explore(state, visibility, ant);



                }
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
            }

            //For debugging
            //while (state.TimeRemaining > 10) continue;
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

            var map = state.NewMap<int>();
            map.Init(50);

            var queue = new Queue<Tuple<Location, int>>(startItems.Select(i => new Tuple<Location, int>(i, 0)).ToList());


            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                if (map.At(item.Item1) > item.Item2)
                {
                    map.Set(item.Item1, item.Item2);

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


        public static T FindClosest<T>(GameState state, Location startingPoint,  Func<Location,T> test, out Direction dir, int MaxDistance = 10) where T : class
        {
           
            Dictionary<Location, int> distance = new Dictionary<Location, int>();

            var queue = new Queue<Location>();

            distance[startingPoint] = 0;
            queue.Enqueue(startingPoint);


            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                foreach (Direction direction in Ants.Aim.Keys)
                {
                    Location newLoc = state.GetDestination(item, direction);
                    if (distance.ContainsKey(newLoc))
                        continue;//Already tested

                    if (!state.GetIsPassable(newLoc))
                        continue;//Cant go there

                    T result = test(newLoc);
                    if (result != null)
                    {
                        dir = direction;
                        return result;
                    }

                    distance[newLoc] =  distance[item] + 1;
                    queue.Enqueue(newLoc);
                    
                }
                

            }

            dir = Direction.East;
            return null;
        }

        private int[,] calculateFoodProximity(IGameState state)
        {
            return CalculateProximity(state, state.FoodTiles);
        }

        private bool getFood(GameState state, int[,] FoodProximity,  Ant ant)
        {
            if (FoodProximity.At(ant) < FoodRadius)
            {
                int value = 999;
                Direction dir = Direction.North;
                foreach (Direction direction in Ants.Aim.Keys)
                {
                    Location newLoc = state.GetDestination(ant, direction);
                    if (state.GetIsPassable(newLoc) && !state.OccupiedNextRound.At(newLoc) && value > FoodProximity.At(newLoc))
                    {
                        value = FoodProximity.At(newLoc);
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

                int value = visibility.At(ant);
                if (value < 50)
                {
                    Direction dir = Direction.North;
                    foreach (Direction direction in Ants.Aim.Keys)
                    {
                        Location newLoc = state.GetDestination(ant, direction);
                        if (state.GetIsPassable(newLoc) && !state.OccupiedNextRound.At(newLoc) && value > visibility.At(newLoc))
                        {
                            value = visibility.At(newLoc);
                            dir = direction;
                        }

                    }
                    if (value != visibility.At(ant))
                    {
                        IssueOrder(state, ant, dir);
                        return true;
                    }
                }
            }


            //try distanciate other ants
            var AllyProximity = calculateAllyProximity(state, ant);
            if (AllyProximity.At(ant) < 20)
            {
                int value = AllyProximity.At(ant);
                Direction dir = Direction.North;
                foreach (Direction direction in Ants.Aim.Keys)
                {
                    Location newLoc = state.GetDestination(ant, direction);
                    if (state.GetIsPassable(newLoc) && !state.OccupiedNextRound.At(newLoc) && value < AllyProximity.At(newLoc))
                    {
                        value = AllyProximity.At(newLoc);
                        dir = direction;
                    }

                }
                if (value != AllyProximity.At(ant))
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
        const int platoonRadius = 4;
        const int platoonMaxTotalRadius = 10;
        const int GuardDistance = 2;

        public List<Ant> AllyPlatoon = new List<Ant>();
        public List<Ant> EnemyPlatoon = new List<Ant>();

        public static void BuildPlatoon(IGameState state, List<Ant> platoonToFill, Ant startingAnt)
        {
            var antList = (startingAnt.Team == 0 ? state.MyAnts : state.EnemyAnts).Where(x => state.GetDistance(startingAnt, x) < platoonMaxTotalRadius).ToList(); ;

            platoonToFill.Add(startingAnt);
            var newAnts = new List<Ant>(platoonToFill);

            //Coud be done using only an index for new Ants instead of another list
            while(newAnts.Count>0)
            {
                antList = antList.Except(newAnts).ToList();

                var current = newAnts.Last();
                newAnts.RemoveAt(newAnts.Count - 1);
                
                var newFound = antList.Where(x => state.GetDistance(current, x) < platoonRadius).ToList();

               
                newAnts.AddRange(newFound);
                platoonToFill.AddRange(newFound);
            }
            
        }

        //Return the distance to ennemy for each ant in the list
        public static List<int> BuildFrontline(IGameState state, List<Ant> platoon, List<Ant> Enemies, out int qty, out int distance)
        {
            var DistanceToFrontline = new List<int>(platoon.Count);

            for (int i = 0; i < platoon.Count; i++)
            {
                DistanceToFrontline.Add( Enemies.Select(enemy => state.GetDistance(platoon[i], enemy)).Min());
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

                var EnemyDist = BuildFrontline(state, EnemyPlatoon, AllyPlatoon, out int Eqty, out int Edistance);


                if (Eqty >= qty)
                {
                    //We have more ants, but they are not on the front line!
                    //Regroup!


                    if (AllyDist.Where(x => x == distance + 1).Count() > Eqty)
                    {
                        //At the end of the turn, we will be in position, no need to move back

                        Regroup(state, AllyDist.ToArray(), distance, true);

                    }
                    else
                    {
                        //Maybe we should move back our units...
                        Regroup(state, AllyDist.ToArray(), distance, true);

                    }


                }
                else
                {
                    Attack(state);
                }
            }
            else
            {
                Retreat(state);

            }


        }

        public void Retreat(GameState state)
        {
            var EnemyDist = MyBot.CalculateProximity(state, EnemyPlatoon);
            for (int i = 0; i < AllyPlatoon.Count; i++)
            {
                Retreat(state, EnemyDist, AllyPlatoon[i] );
            }
        }

        public void Retreat(GameState state, int[,] EnemyDist, Ant ant)
        {

            var best = Ants.Aim.Keys.Select(dir => new { dir, loc = state.GetDestination(ant, dir) } )
                            .Where(x => !state.OccupiedNextRound.At(x.loc))
                            .Where(x => state.GetIsPassable(x.loc))
                            .OrderBy(x => EnemyDist.At(x.loc))
                            .LastOrDefault();

            if (best == null)
                return;

            if ( EnemyDist.At(best.loc) >= EnemyDist.At(ant))
                Bot.IssueOrder(state, ant, best.dir);


            //foreach (Direction direction in Ants.Aim.Keys)
            //{
            //    Location newLoc = state.GetDestination(ant, direction);
            //   //Todo sort best result;
            //    if (state.GetIsPassable(newLoc) && !state.OccupiedNextRound.At(newLoc) && EnemyDist.At(newLoc) > EnemyDist.At(ant))
            //    {
            //        Bot.IssueOrder(state, ant, direction);
            //        break;
            //    }
            //}
        }

        public void Attack(GameState state, int[,] EnemyDist, Ant ant)
        {

            var best = Ants.Aim.Keys.Select(dir => new { dir, loc = state.GetDestination(ant, dir) })
                            .Where(x => !state.OccupiedNextRound.At(x.loc))
                            .Where(x => state.GetIsPassable(x.loc))
                            .OrderByDescending(x => EnemyDist.At(x.loc))
                            //.ThenBy(state.GetBestDirection(x.loc, loc) //Prioritize by direction, need to have a goal...
                            .LastOrDefault();

            if (best == null)
                return;//TODO wals around obstacle here??

            if (EnemyDist.At(best.loc) <= EnemyDist.At(ant))
                Bot.IssueOrder(state, ant, best.dir);


            //foreach (Direction direction in Ants.Aim.Keys)
            //{
            //    Location newLoc = state.GetDestination(ant, direction);
            //   //Todo sort best result;
            //    if (state.GetIsPassable(newLoc) && !state.OccupiedNextRound.At(newLoc) && EnemyDist.At(newLoc) > EnemyDist.At(ant))
            //    {
            //        Bot.IssueOrder(state, ant, direction);
            //        break;
            //    }
            //}
        }

        public void Attack(GameState state)
        {
            var EnemyDist = MyBot.CalculateProximity(state, EnemyPlatoon);
            for (int i = 0; i < AllyPlatoon.Count; i++)
            {
                Attack(state, EnemyDist, AllyPlatoon[i]);
            }
        }

        public void Regroup(GameState state, int[] distances, int fallbackDistance, bool moveBack = false)
        {
            var EnemyDist = MyBot.CalculateProximity(state, EnemyPlatoon);


            for (int i = 0; i < AllyPlatoon.Count; i++)
            {
                if (distances[i] <= fallbackDistance)// distance should be the min...
                {
                    if (moveBack)
                        Retreat(state, EnemyDist, AllyPlatoon[i]);
                    continue;
                }

                Attack(state, EnemyDist, AllyPlatoon[i]);
            }
        }

    }

    public static class DictHelper
    {
        public static TValue GetValueOrDefault<TKey, TValue>
        (this IDictionary<TKey, TValue> dictionary,
         TKey key,
         TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static TValue GetValueOrDefault<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary,
             TKey key,
             Func<TValue> defaultValueProvider)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value
                 : defaultValueProvider();
        }
    }

}