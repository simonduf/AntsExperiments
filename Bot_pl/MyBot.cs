using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Ants.Logger;


namespace Ants {

	class MyBot : Bot {


        int turn = 0;
		public override void DoTurn (IGameState state) {

            try
            {
                foreach (Ant ant in state.MyAnts)
                {
                    int x = ant.Col;
                    int y = ant.Row;

                    // check if we have time left to calculate more orders
                    if (state.TimeRemaining < 10) break;

                    IssueOrder(ant, Direction.South);


                }
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
            }
			
		}

    public static void Main (string[] args) {
			new Ants().PlayGame(new MyBot());
		}

	}
	
}