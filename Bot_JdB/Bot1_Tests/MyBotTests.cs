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
        public void TestMagnitude1()
        {


            GameState state = new GameState(10, 10, 1, 1, 1, 5, 1);

            state.Set(new Tile[,]
            {
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,a,_,_,_,_,_},
                {_,_,_,a,f,a,_,_,_,_},
                {_,_,_,_,a,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_,_,_,_},
            });

            Assert.Fail();

        }


    }
}
