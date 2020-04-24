using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants;
using System;
using System.Linq;

namespace Bot1_Tests
{
    [TestClass]
    public class DistanceFieldTest
    {

        [TestMethod]
        public void TestBasic()
        {
            DistanceField dut = new DistanceField(5, 5, Tile.Food);

            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;

            Tile[,] map = new Tile[,]
            {
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,f,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_}
            };

            dut.Propagate(map, 5);

            Assert.AreEqual(0, dut.GetDistance(2, 2));
            
            Assert.AreEqual(1, dut.GetDistance(3, 2));
            Assert.AreEqual(2, dut.GetDistance(4, 2));

            Assert.AreEqual(1, dut.GetDistance(2, 3));
            Assert.AreEqual(2, dut.GetDistance(2, 4));

            Assert.AreEqual(1, dut.GetDistance(2, 1));
            Assert.AreEqual(2, dut.GetDistance(2, 0));

            Assert.AreEqual(1, dut.GetDistance(1, 2));
            Assert.AreEqual(2, dut.GetDistance(0, 2));
        }


        [TestMethod]
        public void TestBasic_NotSquare()
        {
            DistanceField dut = new DistanceField(7, 5, Tile.Food);

            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;

            Tile[,] map = new Tile[,]
            {
                {_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_},
                {_,_,f,_,_,_,_},
                {_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_}
            };

            dut.Propagate(map, 10);

            Assert.AreEqual(0, dut.GetDistance(2, 2));

            Assert.AreEqual(1, dut.GetDistance(3, 2));
            Assert.AreEqual(2, dut.GetDistance(4, 2));

            Assert.AreEqual(1, dut.GetDistance(2, 3));
            Assert.AreEqual(2, dut.GetDistance(2, 4));

            Assert.AreEqual(1, dut.GetDistance(2, 1));
            Assert.AreEqual(2, dut.GetDistance(2, 0));

            Assert.AreEqual(1, dut.GetDistance(1, 2));
            Assert.AreEqual(2, dut.GetDistance(0, 2));
        }

        [TestMethod]
        public void TestWrap()
        {
            DistanceField dut = new DistanceField(7, 5, Tile.Food);

            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;
            const Tile w = Tile.Water;

            Tile[,] map = new Tile[,]
            {
                {f,_,_,_,_,_,_},
                {_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_},
                {_,_,_,_,_,_,_}
            };

            dut.Propagate(map, 20);

            Assert.AreEqual(0, dut.GetDistance(0, 0));
            Assert.AreEqual(1, dut.GetDistance(0, 1));
            Assert.AreEqual(1, dut.GetDistance(1, 0));
            Assert.AreEqual(1, dut.GetDistance(0, 4));
            Assert.AreEqual(1, dut.GetDistance(6, 0));
        }


        [TestMethod]
        public void TestWater()
        {
            DistanceField dut = new DistanceField(7, 5, Tile.Food);

            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;
            const Tile w = Tile.Water;

            Tile[,] map = new Tile[,]
            {
                {_,w,_,w,_,w,_},
                {_,w,_,w,_,w,_},
                {_,w,f,w,_,_,_},
                {_,_,w,w,w,w,_},
                {_,w,_,_,_,w,_}
            };

            dut.Propagate(map, 20);

            Assert.AreEqual(0, dut.GetDistance(2, 2));
            Assert.AreEqual(1, dut.GetDistance(2, 1));
            Assert.AreEqual(2, dut.GetDistance(2, 0));
            Assert.AreEqual(12, dut.GetDistance(6, 4));
        }

        [TestMethod]
        public void TestDescent()
        {
            DistanceField dut = new DistanceField(7, 5, Tile.Food);

            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;
            const Tile w = Tile.Water;

            Tile[,] map = new Tile[,]
            {
                {_,w,_,w,_,w,_},
                {_,w,_,w,_,w,_},
                {_,w,f,w,_,_,_},
                {_,_,w,w,w,w,_},
                {_,w,_,_,_,w,_}
            };

            dut.Propagate(map, 20);

            {
                Vector2i r = dut.GetDescent(6, 4).First();
                Assert.AreEqual(-1, r.y);
                Assert.AreEqual(0, r.x);
            }

            {
                Vector2i r = dut.GetDescent(6, 3).First();
                Assert.AreEqual(-1, r.y);
                Assert.AreEqual(0, r.x);
            }

            {
                Vector2i r = dut.GetDescent(6, 2).First();
                Assert.AreEqual(-1, r.x);
                Assert.AreEqual(0, r.y);
            }

            {
                Vector2i r = dut.GetDescent(5, 2).First();
                Assert.AreEqual(-1, r.x);
                Assert.AreEqual(0, r.y);
            }

            {
                Vector2i r = dut.GetDescent(4, 2).First();
                Assert.AreEqual(0, r.x);
                Assert.AreEqual(-1, r.y);
            }

            {
                Vector2i r = dut.GetDescent(2, 1).First();
                Assert.AreEqual(0, r.x);
                Assert.AreEqual(1, r.y);
            }


        }

        [TestMethod]
        public void TestAscent()
        {
            DistanceField dut = new DistanceField(7, 5, Tile.Food);

            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;
            const Tile w = Tile.Water;

            Tile[,] map = new Tile[,]
            {
                {_,w,_,w,_,w,_},
                {_,w,_,w,_,w,_},
                {_,w,f,w,_,_,_},
                {_,_,w,w,w,w,_},
                {_,w,_,_,_,w,_}
            };

            dut.Propagate(map, 20);

            {
                Vector2i r = dut.GetAscent(6, 4).First();
                Assert.AreEqual(1, r.x);
                Assert.AreEqual(0, r.y);
            }


            {
                Vector2i r = dut.GetAscent(5, 2).First();
                Assert.AreEqual(1, r.x);
                Assert.AreEqual(0, r.y);
            }

            {
                Vector2i r = dut.GetAscent(4, 2).First();
                Assert.AreEqual(1, r.x);
                Assert.AreEqual(0, r.y);
            }

            {
                Vector2i r = dut.GetAscent(2, 1).First();
                Assert.AreEqual(0, r.x);
                Assert.AreEqual(-1, r.y);
            }


        }

        [TestMethod]
        public void TestWrap1x()
        {
            DistanceField dut = new DistanceField(5, 7, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(2, 3));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap2x()
        {
            DistanceField dut = new DistanceField(5, 7, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(5, 3));
            Assert.AreEqual(0, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap3x()
        {
            DistanceField dut = new DistanceField(5, 7, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(-1, 3));
            Assert.AreEqual(4, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap4x()
        {
            DistanceField dut = new DistanceField(5, 7, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(0, 3));
            Assert.AreEqual(0, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap5x()
        {
            DistanceField dut = new DistanceField(5, 7, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(4, 3));
            Assert.AreEqual(4, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap1y()
        {
            DistanceField dut = new DistanceField(5, 7, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(2, 3));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap2y()
        {
            DistanceField dut = new DistanceField(5, 7, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(2, 7));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(0, result.y);
        }

        [TestMethod]
        public void TestWrap3y()
        {
            DistanceField dut = new DistanceField(5, 7, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(2, -1));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(6, result.y);
        }

        [TestMethod]
        public void TestWrap4y()
        {
            DistanceField dut = new DistanceField(5, 7, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(2, 0));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(0, result.y);
        }

        [TestMethod]
        public void TestWrap5y()
        {
            DistanceField dut = new DistanceField(5, 7, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(2, 6));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(6, result.y);
        }






    }
}
