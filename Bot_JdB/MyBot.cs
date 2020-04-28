using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Ants.Logger;


namespace Ants {

    class MyBot : Bot {

        private DistanceField exploration = null;
        private DistanceField food = null;
        private DistanceField enemy = null;
        private bool[,] occupied = null;
        private AttackManager attackManager;

        int turn = 0;



        int width;
        int height;


        public override void Initialize(GameState state)
        {
            width = state.Width;
            height = state.Height;

            exploration = new DistanceField(state, tile => tile.terrain == GameState.Terrain.Unknown);
            food = new DistanceField(state, tile => tile.isFood);
            enemy = new DistanceField(state, tile => tile.isEnemyHill);

            occupied = new bool[state.Width, state.Height];

            attackManager = new AttackManager(state);


        }

        public override void DoTurn(GameState state) {
            


            try
            {
                turn++;

                attackManager.MoveOffensive(state);

                exploration.Propagate(2);
                food.Propagate(2);
                enemy.Propagate(2);

                ClearOccupied();

                foreach (Ant ant in state.MyAnts)
                {
                    if (ant.hasMoved)
                        occupied[ant.position.x, ant.position.y] = true;
                }
                

                foreach (Ant ant in state.MyAnts)
                {
                    if (ant.hasMoved)
                        continue;

                    int x = ant.position.x;
                    int y = ant.position.y;

                    
                    if (food.GetDistance(x, y) < 10)
                        MoveAnt(ant, food.GetDescent(x, y));
                    else if (enemy.GetDistance(x, y) < DistanceField.Max)
                        MoveAnt(ant, enemy.GetDescent(x, y));
                    else
                        MoveAnt(ant, exploration.GetDescent(x, y));

                }


                foreach (Ant ant in state.MyAnts)
                    IssueOrder(ant.position, ant.direction);
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
            Vector2i src = ant.position;

            foreach(Vector2i move in moves)
            {
                Vector2i dst = Vector2i.Wrap(src + move, width, height);

                if(!occupied[dst.x, dst.y])
                {
                    occupied[dst.x, dst.y] = true;
                    ant.direction = GetDirection(move);
                    ant.hasMoved = true;
                    return;
                }
            }
        }


        public static Direction GetDirection(Vector2i v)
        {
            if (v.x == 0 && v.y == 0)
                return Direction.Halt;

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