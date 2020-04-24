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
            PersistentGameState gameState = new PersistentGameState(5, 5);
            DistanceField dut = new DistanceField(gameState, Tile.Food);

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

            gameState.Update(map);
            

            dut.Propagate(5);

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
        public void TestDropout()
        {
            PersistentGameState gameState = new PersistentGameState(5, 5);
            DistanceField dut = new DistanceField(gameState, Tile.Food);

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

            gameState.Update(map);
            dut.Propagate(5);

            gameState.Update(new Tile[,]
            {
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_}
            });
            dut.Propagate(1);

            

            Assert.AreEqual(DistanceField.Max, dut.GetDistance(2, 2));

            Assert.AreEqual(DistanceField.Max, dut.GetDistance(3, 2));
            Assert.AreEqual(DistanceField.Max, dut.GetDistance(4, 2));

            
        }



        [TestMethod]
        public void TestBasic_NotSquare()
        {
            PersistentGameState gameState = new PersistentGameState(7, 5);
            DistanceField dut = new DistanceField(gameState, Tile.Food);

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

            gameState.Update(map);

            dut.Propagate(10);

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
            PersistentGameState gameState = new PersistentGameState(7, 5);
            DistanceField dut = new DistanceField(gameState, Tile.Food);

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
            gameState.Update(map);
            dut.Propagate(20);

            Assert.AreEqual(0, dut.GetDistance(0, 0));
            Assert.AreEqual(1, dut.GetDistance(0, 1));
            Assert.AreEqual(1, dut.GetDistance(1, 0));
            Assert.AreEqual(1, dut.GetDistance(0, 4));
            Assert.AreEqual(1, dut.GetDistance(6, 0));
        }


        [TestMethod]
        public void TestWater()
        {
            PersistentGameState gameState = new PersistentGameState(7, 5);
            DistanceField dut = new DistanceField(gameState, Tile.Food);

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

            gameState.Update(map);
            dut.Propagate(20);

            Assert.AreEqual(0, dut.GetDistance(2, 2));
            Assert.AreEqual(1, dut.GetDistance(2, 1));
            Assert.AreEqual(2, dut.GetDistance(2, 0));
            Assert.AreEqual(12, dut.GetDistance(6, 4));
        }

        [TestMethod]
        public void TestDescent()
        {
            PersistentGameState gameState = new PersistentGameState(7, 5);
            DistanceField dut = new DistanceField(gameState, Tile.Food);

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

            gameState.Update(map);
            dut.Propagate(20);

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
            PersistentGameState gameState = new PersistentGameState(7, 5);
            DistanceField dut = new DistanceField(gameState, Tile.Food);

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

            gameState.Update(map);
            dut.Propagate(20);

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
        public void Exploration1()
        {
            PersistentGameState gameState = new PersistentGameState(7, 10);
            DistanceField dut = new DistanceField(gameState, Tile.Unseen, terrain:true);

            const Tile u = Tile.Unseen;
            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;
            const Tile w = Tile.Water;

            Tile[,] map = new Tile[,]
            {
                {w,w,w,w,w,w,w},
                {w,_,_,_,_,_,_},
                {w,_,_,_,_,_,_},
                {w,_,_,_,_,_,_},
                {w,_,_,_,_,_,_},
                {w,_,_,_,_,_,_},
                {w,_,_,_,_,_,_},
                {w,_,_,_,u,_,_},
                {w,_,_,_,_,_,_},
                {w,_,_,_,_,_,_},
            };

            gameState.Update(map);
            dut.Propagate(20);

            {
                Assert.AreEqual(0, dut.GetDistance(4, 7));
                Assert.AreEqual(1, dut.GetDistance(3, 7));
                Assert.AreEqual(2, dut.GetDistance(2, 7));
                Assert.AreEqual(3, dut.GetDistance(2, 6));
                Assert.AreEqual(4, dut.GetDistance(2, 5));
            }
        }

        [TestMethod]
        public void Exploration2()
        {
            PersistentGameState gameState = new PersistentGameState(7, 10);
            DistanceField dut = new DistanceField(gameState, Tile.Unseen, terrain: true);

            const Tile u = Tile.Unseen;
            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;
            const Tile w = Tile.Water;

            Tile[,] map = new Tile[,]
            {
                {w,w,w,w,w,w,w},
                {w,_,_,_,_,_,u},
                {w,_,_,_,_,_,u},
                {w,_,_,_,_,_,u},
                {w,_,_,_,_,_,u},
                {w,_,_,_,_,_,u},
                {w,_,_,_,_,_,u},
                {w,w,w,w,w,_,u},
                {w,_,_,_,_,_,u},
                {w,u,u,u,u,u,u},
            };

            gameState.Update(map);
            dut.Propagate(20);

            {
                Assert.AreEqual(5, dut.GetDistance(1, 1));
                Assert.AreEqual(5, dut.GetDistance(1, 2));
                Assert.AreEqual(0, dut.GetDistance(6, 2));
                Assert.AreEqual(1, dut.GetDistance(5, 2));
                Assert.AreEqual(5, dut.GetDistance(1, 6));
            }

            {
                Vector2i result = dut.GetDescent(1, 6).First();
                Assert.AreEqual(1, result.x);
                Assert.AreEqual(0, result.y);
            }
        }


        [TestMethod]
        public void Exploration3()
        {
            PersistentGameState gameState = new PersistentGameState(7, 10);
            DistanceField dut = new DistanceField(gameState, Tile.Unseen, terrain: true);

            const Tile u = Tile.Unseen;
            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;
            const Tile w = Tile.Water;

            /*
            gameState.Update(new Tile[,]
            {
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {w,w,w,w,w,w,w},
            });
            */


            gameState.Update(new Tile[,]
            {
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u},
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u}
            });
            dut.Propagate(20);
            {
                Assert.AreEqual(0, dut.GetDistance(3, 2));
                Assert.AreEqual(1, dut.GetDistance(3, 3));
                Assert.AreEqual(2, dut.GetDistance(3, 4));
                Assert.AreEqual(1, dut.GetDistance(3, 5));
                Assert.AreEqual(0, dut.GetDistance(3, 6));
            }

            gameState.Update(new Tile[,]
            {
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u},
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u}
            });
            dut.Propagate(20);
            {
                Assert.AreEqual(0, dut.GetDistance(3, 1));
                Assert.AreEqual(1, dut.GetDistance(3, 2));
                Assert.AreEqual(2, dut.GetDistance(3, 3));
                Assert.AreEqual(2, dut.GetDistance(3, 4));
                Assert.AreEqual(1, dut.GetDistance(3, 5));
                Assert.AreEqual(0, dut.GetDistance(3, 6));
                

                Vector2i r = dut.GetDescent(3, 2).First();
                Assert.AreEqual(0, r.x);
                Assert.AreEqual(-1, r.y);
            }


            gameState.Update(new Tile[,]
            {
                {u,u,u,u,u,u,u},
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {w,w,w,_,w,w,w},
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u},
                {u,u,u,u,u,u,u}
            });
            dut.Propagate(20);
            {
                Assert.AreEqual(0, dut.GetDistance(3, 0));
                Assert.AreEqual(1, dut.GetDistance(3, 1));
                Assert.AreEqual(2, dut.GetDistance(3, 2));
                Assert.AreEqual(3, dut.GetDistance(3, 3));
                Assert.AreEqual(2, dut.GetDistance(3, 4));
                Assert.AreEqual(1, dut.GetDistance(3, 5));
                Assert.AreEqual(0, dut.GetDistance(3, 6));

                Vector2i r = dut.GetDescent(3, 2).First();
                Assert.AreEqual(0, r.x);
                Assert.AreEqual(-1, r.y);
            }

        }


        [TestMethod]
        public void PartialPropagation()
        {
            PersistentGameState gameState = new PersistentGameState(5, 5);
            DistanceField dut = new DistanceField(gameState, Tile.Food);

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

            gameState.Update(map);


            dut.Propagate(1);

            Assert.AreEqual(0, dut.GetDistance(2, 2));

            Assert.AreEqual(1, dut.GetDistance(3, 2));
            Assert.AreEqual(DistanceField.Max, dut.GetDistance(4, 2));

            Assert.AreEqual(1, dut.GetDistance(2, 3));
            Assert.AreEqual(DistanceField.Max, dut.GetDistance(2, 4));

            Assert.AreEqual(1, dut.GetDistance(2, 1));
            Assert.AreEqual(DistanceField.Max, dut.GetDistance(2, 0));

            Assert.AreEqual(1, dut.GetDistance(1, 2));
            Assert.AreEqual(DistanceField.Max, dut.GetDistance(0, 2));

            dut.Propagate(1);

            Assert.AreEqual(0, dut.GetDistance(2, 2));

            Assert.AreEqual(1, dut.GetDistance(3, 2));
            Assert.AreEqual(2, dut.GetDistance(4, 2));

            Assert.AreEqual(1, dut.GetDistance(2, 3));
            Assert.AreEqual(2, dut.GetDistance(2, 4));

            Assert.AreEqual(1, dut.GetDistance(2, 1));
            Assert.AreEqual(2, dut.GetDistance(2, 0));

            Assert.AreEqual(1, dut.GetDistance(1, 2));
            Assert.AreEqual(2, dut.GetDistance(0, 2));

            dut.Propagate(1);

            Assert.AreEqual(0, dut.GetDistance(2, 2));

            Assert.AreEqual(1, dut.GetDistance(3, 2));
            Assert.AreEqual(2, dut.GetDistance(4, 2));
            Assert.AreEqual(3, dut.GetDistance(4, 3));

            Assert.AreEqual(1, dut.GetDistance(2, 3));
            Assert.AreEqual(2, dut.GetDistance(2, 4));
            Assert.AreEqual(3, dut.GetDistance(3, 4));

            Assert.AreEqual(1, dut.GetDistance(2, 1));
            Assert.AreEqual(2, dut.GetDistance(2, 0));
            Assert.AreEqual(3, dut.GetDistance(3, 0));

            Assert.AreEqual(1, dut.GetDistance(1, 2));
            Assert.AreEqual(2, dut.GetDistance(0, 2));
            Assert.AreEqual(3, dut.GetDistance(0, 3));
        }

        [TestMethod]
        public void ProblemCase1()
        {
            PersistentGameState gameState = new PersistentGameState(7, 10);
            DistanceField dut = new DistanceField(gameState, Tile.Food);

            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;
            const Tile w = Tile.Water;
            const Tile u = Tile.Unseen;

            Tile[,] map = new Tile[,]
            {
                {_,_,_,u,_,_,w},
                {_,f,_,u,_,_,w},
                {_,_,_,w,_,_,w},
                {_,_,_,w,_,_,w},
                {_,_,_,w,_,_,w},
                {_,_,_,w,w,_,w},
                {_,_,_,_,w,f,w},
                {_,_,_,w,w,_,w},
                {_,_,_,_,w,_,w},
                {w,w,w,w,w,w,w},
            };

            gameState.Update(map);
            dut.Propagate(20);

            {
                Vector2i r = dut.GetDescent(3, 6).First();
                Assert.AreEqual(-1, r.x);
                Assert.AreEqual(0, r.y);
            }
        }

        [TestMethod]
        public void ProblemCase2()
        {
            PersistentGameState gameState = new PersistentGameState(7, 10);
            DistanceField dut = new DistanceField(gameState, Tile.Food);

            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;
            const Tile w = Tile.Water;
            const Tile u = Tile.Unseen;

            Tile[,] map = new Tile[,]
            {
                {_,w,w,_,_,_,w},
                {_,w,w,f,_,_,w},
                {_,w,w,_,_,_,w},
                {w,w,w,_,_,_,w},
                {w,w,w,_,_,_,w},
                {_,_,_,_,_,_,w},
                {_,_,_,_,_,_,w},
                {_,_,_,_,_,_,w},
                {_,_,_,_,_,_,w},
                {w,w,w,w,w,w,w},
            };

            gameState.Update(map);
            dut.Propagate(20);

            {
                Vector2i r = dut.GetDescent(2, 5).First();
                Assert.AreEqual(1, r.x);
                Assert.AreEqual(0, r.y);
            }

            {
                Vector2i r = dut.GetDescent(1, 5).First();
                Assert.AreEqual(1, r.x);
                Assert.AreEqual(0, r.y);
            }
        }

        [TestMethod]
        public void ProblemCase3_AntHills()
        {
            PersistentGameState gameState = new PersistentGameState(7, 10);
            DistanceField dut = new DistanceField(gameState, Tile.TheirHill, terrain:true);

            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;
            const Tile w = Tile.Water;
            const Tile h = Tile.TheirHill;
            const Tile u = Tile.Unseen;

            gameState.Update(new Tile[,]
            {
                {_,w,w,_,_,_,w},
                {_,w,w,h,_,_,w},
                {_,w,w,_,_,_,w},
                {w,w,w,_,_,_,w},
                {w,w,w,_,_,_,w},
                {_,_,_,_,_,_,w},
                {_,_,_,_,_,_,w},
                {_,_,_,_,_,_,w},
                {_,_,_,_,_,_,w},
                {w,w,w,w,w,w,w},
            });

            dut.Propagate(20);

            Assert.AreEqual(1, dut.GetDistance(3, 2));

            {
                Vector2i r = dut.GetDescent(2, 5).First();
                Assert.AreEqual(1, r.x);
                Assert.AreEqual(0, r.y);
            }

            {
                Vector2i r = dut.GetDescent(1, 5).First();
                Assert.AreEqual(1, r.x);
                Assert.AreEqual(0, r.y);
            }



            gameState.Update(new Tile[,]
            {
                {_,w,w,_,_,_,w},
                {_,w,w,u,_,_,w},
                {_,w,w,_,_,_,w},
                {w,w,w,_,_,_,w},
                {w,w,w,_,_,_,w},
                {_,_,_,_,_,_,w},
                {_,_,_,_,_,_,w},
                {_,_,_,_,_,_,w},
                {_,_,_,_,_,_,w},
                {w,w,w,w,w,w,w},
            });

            dut.Propagate(20);

            Assert.AreEqual(1, dut.GetDistance(3, 2));

            {
                Vector2i r = dut.GetDescent(2, 5).First();
                Assert.AreEqual(1, r.x);
                Assert.AreEqual(0, r.y);
            }

            {
                Vector2i r = dut.GetDescent(1, 5).First();
                Assert.AreEqual(1, r.x);
                Assert.AreEqual(0, r.y);
            }
        }

        [TestMethod]
        public void TestWrap1x()
        {
            PersistentGameState gameState = new PersistentGameState(5, 7);
            DistanceField dut = new DistanceField(gameState, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(2, 3));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap2x()
        {
            PersistentGameState gameState = new PersistentGameState(5, 7);
            DistanceField dut = new DistanceField(gameState, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(5, 3));
            Assert.AreEqual(0, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap3x()
        {
            PersistentGameState gameState = new PersistentGameState(5, 7);
            DistanceField dut = new DistanceField(gameState, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(-1, 3));
            Assert.AreEqual(4, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap4x()
        {
            PersistentGameState gameState = new PersistentGameState(5, 7);
            DistanceField dut = new DistanceField(gameState, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(0, 3));
            Assert.AreEqual(0, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap5x()
        {
            PersistentGameState gameState = new PersistentGameState(5, 7);
            DistanceField dut = new DistanceField(gameState, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(4, 3));
            Assert.AreEqual(4, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap1y()
        {
            PersistentGameState gameState = new PersistentGameState(5, 7);
            DistanceField dut = new DistanceField(gameState, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(2, 3));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap2y()
        {
            PersistentGameState gameState = new PersistentGameState(5, 7);
            DistanceField dut = new DistanceField(gameState, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(2, 7));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(0, result.y);
        }

        [TestMethod]
        public void TestWrap3y()
        {
            PersistentGameState gameState = new PersistentGameState(5, 7);
            DistanceField dut = new DistanceField(gameState, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(2, -1));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(6, result.y);
        }

        [TestMethod]
        public void TestWrap4y()
        {
            PersistentGameState gameState = new PersistentGameState(5, 7);
            DistanceField dut = new DistanceField(gameState, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(2, 0));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(0, result.y);
        }

        [TestMethod]
        public void TestWrap5y()
        {
            PersistentGameState gameState = new PersistentGameState(5, 7);
            DistanceField dut = new DistanceField(gameState, Tile.Food);
            Vector2i result = dut.Wrap(new Vector2i(2, 6));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(6, result.y);
        }






    }
}
