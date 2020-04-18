using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ants {

	class MyBot : Bot {

        int FoodRadius = 20;
		public override void DoTurn (IGameState state) {

            //FoodRadius = (int)Math.Sqrt(state.ViewRadius2);
            

            //Calculate Enemy proximity
            var FoodProximity = calculateFoodProximity(state);
            var visibility =  calculateVisibilityProximity(state);

            foreach (Ant ant in state.MyAnts)
            {
                // check if we have time left to calculate more orders
                if (state.TimeRemaining < 10) break;



                // General game signals
                // Defend hill -> converge + sacrifice self
                //continue if move done

                //Self ant todo:
                // (enemy>Ally) ? flee() :attack
                //continue if move done


                if (getFood(state, FoodProximity, ant))
                    continue;


                explore(state, visibility, ant );

				

			}
			
		}

        static void InitArray(int[,] array, int val)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i,j] = val;
                }
            }
        }

        private int[,] calculateAllyProximity(IGameState state, Ant ant)
        {
            return calculateProximity(state, state.MyAnts.Where(x => x != ant).ToList());
        }

        private int[,] calculateVisibilityProximity(IGameState state)
        {
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

            return calculateProximity(state, notVisible);
        }

        private int[,] calculateProximity<T>(IGameState state, List<T> startItems) where T: Location
        {
            //TODO do not create a new array each frame...

            var map = new int[state.Width, state.Height];
            InitArray(map, 50);

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
            return calculateProximity(state, state.FoodTiles);
        }

        private bool getFood(IGameState state, int[,] FoodProximity,  Ant ant)
        {
            if (FoodProximity[ant.Col, ant.Row] < FoodRadius)
            {
                int value = 999;
                Direction dir = Direction.North;
                foreach (Direction direction in Ants.Aim.Keys)
                {
                    Location newLoc = state.GetDestination(ant, direction);
                    if (state.GetIsPassable(newLoc) && value > FoodProximity[newLoc.Col, newLoc.Row])
                    {
                        value = FoodProximity[newLoc.Col, newLoc.Row];
                        dir = direction;
                    }

                }

                IssueOrder(ant, dir);
                return true;
            }

            return false;
        }

        private bool explore(IGameState state, int[,] visibility, Ant ant)
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
                        if (state.GetIsPassable(newLoc) && value > visibility[newLoc.Col, newLoc.Row])
                        {
                            value = visibility[newLoc.Col, newLoc.Row];
                            dir = direction;
                        }

                    }
                    if (value != visibility[ant.Col, ant.Row])
                    {
                        IssueOrder(ant, dir);
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
                    if (state.GetIsPassable(newLoc) && value < AllyProximity[newLoc.Col, newLoc.Row])
                    {
                        value = AllyProximity[newLoc.Col, newLoc.Row];
                        dir = direction;
                    }

                }
                if (value != AllyProximity[ant.Col, ant.Row])
                {
                    IssueOrder(ant, dir);
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
	
}