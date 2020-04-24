using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Ants.Logger;


namespace Ants {

    class MyBot : Bot {


        private PersistentGameState pState;

        private DistanceField exploration = null;
        private DistanceField food = null;
        private DistanceField enemy = null;
        private bool[,] occupied = null;
        int turn = 0;

        int width;
        int height;

        public override void DoTurn(IGameState state) {

            width = state.Width;
            height = state.Height;

            turn++;

            if (pState == null)
                pState = new PersistentGameState(state.Width, state.Height);

            if (exploration == null)
                exploration = new DistanceField(pState, Tile.Unseen, terrain: true);

            if (food == null)
                food = new DistanceField(pState, Tile.Food);

            if (enemy == null)
                enemy = new DistanceField(pState, Tile.TheirHill, terrain: true);

            if (occupied == null)
                occupied = new bool[state.Width, state.Height];

            /*
            if (turn > 75)
            {
                //Log.Debug("Test");
                //Log.Debug(pState.ToString(34 - 5, 89 - 5, 10, 10));
                Log.Debug("Turn: " + turn);
                Print(state.AllTiles, 34 - 5, 89 - 5, 10, 10, (data,i,j) => data[j,i].ToString());
                //Print(state.AllTiles, 44 - 10, 54 - 10, 20, 20, (data, i, j) => data[j, i].ToString());
            }
            */

            try
            {
                pState.Update(state.AllTiles);

                exploration.Propagate(5);
                food.Propagate(5);
                enemy.Propagate(5);

                ClearOccupied();


                foreach (Ant ant in state.MyAnts)
                {
                    int x = ant.Col;
                    int y = ant.Row;

                    
                    if (enemy.GetDistance(x,y) < DistanceField.Max)
                        MoveAnt(ant, enemy.GetDescent(x, y));
                    else if (food.GetDistance(x, y) < exploration.GetDistance(x, y))
                        MoveAnt(ant, food.GetDescent(x, y));
                    else
                        MoveAnt(ant, exploration.GetDescent(x, y));


                    

                }
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
            }

        }

        private void ClearOccupied()
        {
            for(int i = 0; i < occupied.GetLength(0); i++)
            {
                for (int j = 0; j < occupied.GetLength(1); j++)
                {
                    occupied[i, j] = false;
                }
            }
        }

        private void MoveAnt(Ant ant, IEnumerable<Vector2i> moves)
        {
            Vector2i src = new Vector2i(ant.Col, ant.Row);

            foreach(Vector2i move in moves)
            {
                Vector2i dst = Vector2i.Wrap(src + move, width, height);

                if(!occupied[dst.x, dst.y])
                {
                    occupied[dst.x, dst.y] = true;
                    IssueOrder(ant, GetDirection(move));
                    return;
                }
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

        public static void Print<T>(T[,] data, int x, int y, int width, int height, Func<T[,], int, int, String> formater)
        {
            StringBuilder builder = new StringBuilder("Custom Print: \n");
            for (int j = y; j < y + height; j++)
            {
                for (int i = x; i < x + width; i++)
                {
                    var tile = data[i, j];
                    builder.Append(formater(data, i, j) + "\t");
                }
                builder.Append("\n");
            }

            Log.Debug(builder.ToString());
        }


        public static void Main (string[] args) {
			new Ants().PlayGame(new MyBot());
		}

	}
	
}