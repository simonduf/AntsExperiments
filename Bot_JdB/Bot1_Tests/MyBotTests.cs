using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants;
using System;

namespace Bot1_Tests
{
    [TestClass]
    public class MyBotTests
    {
        const Tile f = Tile.Food;
        const Tile _ = Tile.Land;
        const Tile u = Tile.Unseen;
        const Tile w = Tile.Water;
        const Tile h = Tile.MyHill;
        const Tile H = Tile.TheirHill;
        const Tile a = Tile.MyAnt;
        const Tile A = Tile.TheirAnt;

        [TestMethod]
        public void TestNoCollision1()
        {


            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,a,a,a,a,a,a,_,_},
                {_,_,a,a,w,a,a,a,_,_},
                {_,_,a,a,w,a,a,a,_,_},
                {_,_,a,a,f,a,a,a,_,_},
                {_,_,a,a,a,a,a,a,_,_},
                {_,_,a,a,a,a,a,a,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            MyBot dut = new MyBot();

            dut.Initialize(state);

            for (int i = 0; i < 10; i++)
                dut.Cook(state);


            dut.DoTurn(state);

            bool takeFood = false;
            foreach (Ant a in state.MyAnts)
            {
                Assert.IsTrue(a.hasMoved);
                Vector2i dst = a.position + Vector2i.AllDirections[(int)a.direction];
                if (state.map[dst.x, dst.y].isFood)
                    takeFood = true;
            }
            Assert.IsTrue(takeFood);


            for (int i = 0; i < state.MyAnts.Count; i++)
            {
                Ant a = state.MyAnts[i];
                Vector2i aPos = a.position + Vector2i.AllDirections[(int)a.direction];
                for (int j = 0; j < i; j++)
                {
                    Ant b = state.MyAnts[j];
                    Vector2i bPos = b.position + Vector2i.AllDirections[(int)b.direction];

                    Assert.AreNotEqual(aPos, bPos);
                }
            }


        }


        [TestMethod]
        public void TestNoCollision2()
        {


            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,w},
                {_,_,_,_,_,_,_,_,_,w},
                {_,_,_,_,_,_,_,_,_,w},
                {w,w,w,_,a,w,_,_,_,w},
                {_,_,w,w,a,w,_,_,_,w},
                {_,_,w,_,a,w,_,_,_,w},
                {_,_,w,w,_,_,_,_,_,w},
                {_,_,w,_,_,_,_,_,_,w},
                {_,_,w,_,_,_,_,_,_,w},
                {w,w,w,w,w,w,w,w,w,w},
            });

            MyBot dut = new MyBot();

            dut.Initialize(state);

            for(int i = 0; i < 10; i++)
                dut.Cook(state);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,w},
                {_,_,_,_,_,_,_,_,_,w},
                {_,_,_,_,f,_,_,_,_,w},
                {w,w,w,_,a,w,_,_,_,w},
                {_,_,w,w,a,w,_,_,_,w},
                {_,_,w,_,a,w,_,_,_,w},
                {_,_,w,w,_,_,_,_,_,w},
                {_,_,w,_,_,_,_,_,_,w},
                {_,_,w,_,_,_,_,_,_,w},
                {w,w,w,w,w,w,w,w,w,w},
            });

            dut.DoTurn(state);

            bool takeFood = false;
            foreach (Ant a in state.MyAnts)
            {
                Assert.IsTrue(a.hasMoved);
                Vector2i dst = a.position + Vector2i.AllDirections[(int)a.direction];
                if (state.map[dst.x, dst.y].isFood)
                    takeFood = true;
            }
            Assert.IsTrue(takeFood);


            for (int i = 0; i < state.MyAnts.Count; i++)
            {
                Ant a = state.MyAnts[i];
                Vector2i aPos = a.position + Vector2i.AllDirections[(int)a.direction];
                for (int j = 0; j < i; j++)
                {
                    Ant b = state.MyAnts[j];
                    Vector2i bPos = b.position + Vector2i.AllDirections[(int)b.direction];

                    Assert.AreNotEqual(aPos, bPos);
                }
            }


        }


        [TestMethod]
        public void PerformanceIssue()
        {
            Assert.Fail();

            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,a,a,a,a,a,a,_,_},
                {_,_,a,a,w,a,a,a,_,_},
                {_,_,a,a,w,a,a,a,_,_},
                {_,_,a,a,f,a,a,a,_,_},
                {_,_,a,a,a,A,a,a,_,_},
                {_,_,a,a,a,a,a,a,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            Bot dut = new MyBot();

            dut.Initialize(state);

            dut.DoTurn(state);



        }



    }
}
