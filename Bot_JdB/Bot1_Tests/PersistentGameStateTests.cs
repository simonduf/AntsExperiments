using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants;
using System;

namespace Bot1_Tests
{
    [TestClass]
    public class PersistentGameStateTests
    {
        const Tile f = Tile.Food;
        const Tile _ = Tile.Land;
        const Tile u = Tile.Unseen;
        const Tile w = Tile.Water;
        const Tile h = Tile.Hill;

        [TestMethod]
        public void TestMagnitude1()
        {
            PersistentGameState dut = new PersistentGameState(3, 3);

            dut.Update(new Tile[,]
            {
                {_,_,_},
                {w,_,_},
                {u,h,f}
            });


            Assert.AreEqual(Tile.Land, dut.map[0, 0].objects);
            Assert.AreEqual(Tile.Land, dut.map[0, 0].terrain);

            Assert.AreEqual(Tile.Water, dut.map[0, 1].objects);
            Assert.AreEqual(Tile.Water, dut.map[0, 1].terrain);

            Assert.AreEqual(Tile.Unseen, dut.map[0, 2].objects);
            Assert.AreEqual(Tile.Unseen, dut.map[0, 2].terrain);

            Assert.AreEqual(Tile.Food, dut.map[2, 2].objects);
            Assert.AreEqual(Tile.Land, dut.map[2, 2].terrain);

            Assert.AreEqual(Tile.Hill, dut.map[1, 2].objects);
            Assert.AreEqual(Tile.Land, dut.map[1, 2].terrain);

            dut.Update(new Tile[,]
            {
                {u,u,u},
                {u,u,u},
                {u,u,u}
            });

            Assert.AreEqual(Tile.Unseen, dut.map[0, 0].objects);
            Assert.AreEqual(Tile.Land, dut.map[0, 0].terrain);

            Assert.AreEqual(Tile.Unseen, dut.map[0, 1].objects);
            Assert.AreEqual(Tile.Water, dut.map[0, 1].terrain);

            Assert.AreEqual(Tile.Unseen, dut.map[0, 2].objects);
            Assert.AreEqual(Tile.Unseen, dut.map[0, 2].terrain);

            Assert.AreEqual(Tile.Unseen, dut.map[2, 2].objects);
            Assert.AreEqual(Tile.Land, dut.map[2, 2].terrain);

            Assert.AreEqual(Tile.Unseen, dut.map[1, 2].objects);
            Assert.AreEqual(Tile.Land, dut.map[1, 2].terrain);


        }


    }
}
