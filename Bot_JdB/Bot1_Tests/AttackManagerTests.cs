using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants;
using System;

namespace Bot1_Tests
{
    [TestClass]
    public class AttackManagerTests
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
        public void TestDecision1v1_North()
        {
            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,A,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,a,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            AttackManager dut = new AttackManager(state);

            dut.MoveOffensive(state);

            Assert.IsTrue(state.MyAnts[0].hasMoved);
            Assert.AreEqual(Direction.South, state.MyAnts[0].direction);
        }

        [TestMethod]
        public void TestDecision1v1_West()
        {
            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,A,_,_,a,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            AttackManager dut = new AttackManager(state);

            dut.MoveOffensive(state);

            Assert.IsTrue(state.MyAnts[0].hasMoved);
            Assert.AreEqual(Direction.East, state.MyAnts[0].direction);
        }

        [TestMethod]
        public void TestDecision1v1_South()
        {
            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,a,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,A,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            AttackManager dut = new AttackManager(state);

            dut.MoveOffensive(state);

            Assert.IsTrue(state.MyAnts[0].hasMoved);
            Assert.AreEqual(Direction.North, state.MyAnts[0].direction);
        }

        [TestMethod]
        public void TestDecision1v1_East()
        {
            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,a,_,_,A,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            AttackManager dut = new AttackManager(state);

            dut.MoveOffensive(state);

            Assert.IsTrue(state.MyAnts[0].hasMoved);
            Assert.AreEqual(Direction.West, state.MyAnts[0].direction);
        }

        [TestMethod]
        public void TestDecision1v1_Wrap1()
        {
            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,A,_,_,_,_,_,_,a,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            AttackManager dut = new AttackManager(state);

            dut.MoveOffensive(state);

            Assert.IsTrue(state.MyAnts[0].hasMoved);
            Assert.AreEqual(Direction.West, state.MyAnts[0].direction);
        }


        [TestMethod]
        public void TestDecision1v1_Wrap2()
        {
            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,A,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,a,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            AttackManager dut = new AttackManager(state);

            dut.MoveOffensive(state);

            Assert.IsTrue(state.MyAnts[0].hasMoved);
            Assert.AreEqual(Direction.North, state.MyAnts[0].direction);
        }


        [TestMethod]
        public void TestDecision1v2_1()
        {
            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,A,A,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,a,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            AttackManager dut = new AttackManager(state);

            dut.MoveOffensive(state);

            Assert.IsTrue(state.MyAnts[0].hasMoved);
            Assert.AreEqual(Direction.South, state.MyAnts[0].direction);
        }

        [TestMethod]
        public void TestDecision1v2_2()
        {
            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,A,A,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,a,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            AttackManager dut = new AttackManager(state);

            dut.MoveOffensive(state);

            Assert.IsTrue(state.MyAnts[0].hasMoved);
            Assert.AreEqual(Direction.South, state.MyAnts[0].direction);
        }


        [TestMethod]
        public void TestDecision1v2_3()
        {
            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,A,A,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,a,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            AttackManager dut = new AttackManager(state);

            dut.MoveOffensive(state);

            Assert.IsTrue(state.MyAnts[0].hasMoved);
            Assert.AreNotEqual(Direction.North, state.MyAnts[0].direction);
        }


        [TestMethod]
        public void TestDisengage()
        {
            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,A,A,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,a,_,_,_,_},
                {_,_,_,_,a,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            AttackManager dut = new AttackManager(state);

            dut.MoveOffensive(state);

            Assert.IsTrue(state.MyAnts[0].hasMoved);
            Assert.IsFalse(state.MyAnts[1].hasMoved);
        }

        [TestMethod]
        public void TestNoCollisions_1()
        {
            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,a,_,_,_,_},
                {_,_,a,a,a,a,a,_,_,_},
                {_,_,a,a,_,_,_,_,_,_},
                {_,_,a,a,_,A,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            AttackManager dut = new AttackManager(state);

            dut.MoveOffensive(state);


            foreach (Ant a in state.MyAnts)
                Assert.IsTrue(a.hasMoved);


            for (int i = 0; i < state.MyAnts.Count; i++)
            {
                Ant a = state.MyAnts[i];
                Vector2i aPos = a.position + Vector2i.AllDirections[(int)a.direction];
                for(int j = 0; j < i; j++)
                {
                    Ant b = state.MyAnts[j];
                    Vector2i bPos = b.position + Vector2i.AllDirections[(int)b.direction];

                    Assert.AreNotEqual(aPos, bPos);
                }
            }
        }


    }



}
