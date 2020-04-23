using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Ants.Logger;


namespace Ants {

    class MyBot : Bot {


        private PersistentGameState pState;

        private DistanceField exploration = null;
        private DistanceField food = null;
        int turn = 0;



        int FoodRadius = 20;

        public override void DoTurn(IGameState state) {

            turn++;

            if (pState == null)
                pState = new PersistentGameState(state.Width, state.Height);

            if (exploration == null)
                exploration = new DistanceField(pState, Tile.Unseen, terrain: true);

            if (food == null)
                food = new DistanceField(pState, Tile.Food);



            try
            {
                pState.Update(state.AllTiles);

                exploration.Propagate(5);
                food.Propagate(5);

                if (turn == 63)
                    //Log.Debug(food.ToString(25,35,30,30));
                    Log.Debug(pState.ToString(25, 35, 30, 30));

                //Log.Debug("Starting Turn " + turn++);// state.TimeRemaining
                //FoodRadius = (int)Math.Sqrt(state.ViewRadius2);


                //Calculate Enemy proximity
                //var FoodProximity = calculateFoodProximity(state);
                //var visibility = calculateVisibilityProximity(state);

                foreach (Ant ant in state.MyAnts)
                {
                    int x = ant.Col;
                    int y = ant.Row;

                    // check if we have time left to calculate more orders
                    //if (state.TimeRemaining < 10) break;



                    // General game signals
                    // Defend hill -> converge + sacrifice self
                    //continue if move done

                    //Self ant todo:
                    // (enemy>Ally) ? flee() :attack
                    //continue if move done

                    /*
                    if (getFood(state, FoodProximity, ant))
                        continue;


                    explore(state, visibility, ant);
                    */
                    /*
                    var tiles = state.AllTiles;

                    float distance = float.MaxValue;
                    Vector2 direction = Vector2.east;

                    foreach(Location food in state.FoodTiles)
                    {
                        Vector2 dir = state.GetFromTo(ant, food);
                        float mag = dir.magnitude;

                        if(mag < distance)
                        {
                            distance = mag;
                            direction = dir;
                        }

                    }
                    */

                    //IssueOrder(ant, Direction.South);
                    IssueOrder(ant, GetDirection(food.GetDescent(x, y).FirstOrDefault()));


                }
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
            }

        }

        public static Direction GetDirection(Vector2i v)
        {
            if (Math.Abs(v.x) > Math.Abs(v.y))
            {
                return v.x > 0.0f ? Direction.East : Direction.West;
            }
            else
            {
                return v.y > 0.0f ? Direction.South : Direction.North;
            }
        }

    


        public static void Main (string[] args) {
			new Ants().PlayGame(new MyBot());
		}

	}
	
}