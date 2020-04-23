using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants;
using System;

namespace Bot1_Tests
{
    [TestClass]
    public class Vector2Tests
    {
        [TestMethod]
        public void TestMagnitude1()
        {
            Vector2 dut = new Vector2(2, 2);
            Assert.AreEqual(2*1.414, dut.magnitude, 0.002);
        }

        [TestMethod]
        public void TestMagnitude2()
        {
            Vector2 dut = new Vector2(1, 1);
            Assert.AreEqual(1.414, dut.magnitude, 0.002);
            //Assert.Fail();
        }

        [TestMethod]
        public void TestMagnitude3()
        {
            Vector2 dut = new Vector2(3, 0);
            Assert.AreEqual(3.0, dut.magnitude, 0.002);
        }

        [TestMethod]
        public void TestMagnitude4()
        {
            Vector2 dut = new Vector2(0, 4);
            Assert.AreEqual(4.0, dut.magnitude, 0.002);
            //Assert.Fail();
        }


        
        [TestMethod]
        public void TestDirection_North1()
        {
            Direction dir = GameState.GetDirection(new Vector2(0, -1));
            Assert.AreEqual(Direction.North, dir);
        }

        [TestMethod]
        public void TestDirection_North2()
        {
            Direction dir = GameState.GetDirection(new Vector2(-0.99f, -1));
            Assert.AreEqual(Direction.North, dir);
        }


        [TestMethod]
        public void TestDirection_South1()
        {
            Direction dir = GameState.GetDirection(new Vector2(0, 1));
            Assert.AreEqual(Direction.South, dir);
        }

        [TestMethod]
        public void TestDirection_South2()
        {
            Direction dir = GameState.GetDirection(new Vector2(-0.99f, 1));
            Assert.AreEqual(Direction.South, dir);
        }

        [TestMethod]
        public void TestDirection_West1()
        {
            Direction dir = GameState.GetDirection(new Vector2(-1, 0));
            Assert.AreEqual(Direction.West, dir);
        }

        [TestMethod]
        public void TestDirection_West2()
        {
            Direction dir = GameState.GetDirection(new Vector2(-1, 0.99f));
            Assert.AreEqual(Direction.West, dir);
        }

        [TestMethod]
        public void TestDirection_East1()
        {
            Direction dir = GameState.GetDirection(new Vector2(1, 0));
            Assert.AreEqual(Direction.East, dir);
        }

        [TestMethod]
        public void TestDirection_East2()
        {
            Direction dir = GameState.GetDirection(new Vector2(1, 0.9f));
            Assert.AreEqual(Direction.East, dir);
        }

        [TestMethod]
        public void TestToFrom1()
        {
            Location from = new Location(0, 0);
            Location to = new Location(0, 1);
            GameState gameState = new GameState(20, 20, 1, 1, 10, 1, 1);
            
            Vector2 dut = gameState.GetFromTo(from, to);

            Assert.AreEqual(1, dut.x, 0.001);
            Assert.AreEqual(0, dut.y, 0.001);
        }

        [TestMethod]
        public void TestToFrom1_WithWrap()
        {
            Location from = new Location(0, 0);
            Location to = new Location(0, 19);
            GameState gameState = new GameState(20, 20, 1, 1, 10, 1, 1);

            Vector2 dut = gameState.GetFromTo(from, to);

            Assert.AreEqual(-1, dut.x, 0.001);
            Assert.AreEqual(0, dut.y, 0.001);
        }

        [TestMethod]
        public void TestToFrom2()
        {
            Location from = new Location(0, 0);
            Location to = new Location(1, 0);
            GameState gameState = new GameState(20, 20, 1, 1, 10, 1, 1);

            Vector2 dut = gameState.GetFromTo(from, to);

            Assert.AreEqual(0, dut.x, 0.001);
            Assert.AreEqual(1, dut.y, 0.001);
        }

        [TestMethod]
        public void TestToFrom2_WithWrap()
        {
            Location from = new Location(0, 0);
            Location to = new Location(19, 0);
            GameState gameState = new GameState(20, 20, 1, 1, 10, 1, 1);

            Vector2 dut = gameState.GetFromTo(from, to);

            Assert.AreEqual(0, dut.x, 0.001);
            Assert.AreEqual(-1, dut.y, 0.001);
        }


    }
}
