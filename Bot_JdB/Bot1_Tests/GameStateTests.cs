using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants;
using System;

namespace Bot1_Tests
{
    [TestClass]
    public class GameStateTests
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
        public void TestGameStates()
        {
            GameState dut = new GameState(3, 3, 1, 1, 1, 1, 1);

            dut.Set(new Tile[,]
            {
                {_,_,_},
                {w,_,_},
                {u,h,f}
            });


            Assert.AreEqual(GameState.Terrain.Land, dut.map[0, 0].terrain);
            Assert.AreEqual(GameState.Terrain.Water, dut.map[0, 1].terrain);
            Assert.AreEqual(GameState.Terrain.Unknown, dut.map[0, 2].terrain);

            
            Assert.IsTrue(dut.map[2, 2].isFood);
            Assert.IsTrue(dut.map[1, 2].isMyHill);

            
            dut.Set(new Tile[,]
            {
                {u,u,u},
                {u,u,u},
                {u,u,u}
            });

            Assert.AreEqual(GameState.Terrain.Land, dut.map[0, 0].terrain);
            Assert.AreEqual(GameState.Terrain.Water, dut.map[0, 1].terrain);
            Assert.AreEqual(GameState.Terrain.Unknown, dut.map[0, 2].terrain);

            Assert.IsFalse(dut.map[2, 2].isFood);
            Assert.IsTrue(dut.map[1, 2].isMyHill);

        }

        [TestMethod]
        public void HillTransitions()
        {
            GameState dut = new GameState(1, 1, 1, 1, 1, 1, 1);

            dut.Set(new Tile[,]
            {
                {H}
            });

            Assert.IsTrue(dut.map[0, 0].isEnemyHill);
            Assert.IsFalse(dut.map[0, 0].isEnemyAnt);
            /*
            dut.Set(new Tile[,]
            {
                {A}
            });

            Assert.IsTrue(dut.map[0, 0].isEnemyHill);
            Assert.IsTrue(dut.map[0, 0].isEnemyAnt);

            dut.Set(new Tile[,]
            {
                {H}
            });

            Assert.IsTrue(dut.map[0, 0].isEnemyHill);
            Assert.IsFalse(dut.map[0, 0].isEnemyAnt);
            */
            dut.Set(new Tile[,]
            {
                {_}
            });

            Assert.IsFalse(dut.map[0, 0].isEnemyHill);
            Assert.IsFalse(dut.map[0, 0].isEnemyAnt);



        }


    }



}
