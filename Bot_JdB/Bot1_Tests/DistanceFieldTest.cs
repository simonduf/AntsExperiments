using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants;
using System;
using System.Linq;

namespace Bot1_Tests
{
    [TestClass]
    public class DistanceFieldTest
    {
        static GameState TestGameState(int width, int height)
        {
            return new GameState(width, height, 1, 1, 1, 1, 1);
        }

        [TestMethod]
        public void TestBasic()
        {
            GameState gameState = TestGameState(5, 5);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);

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

            gameState.Set(map);
            

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
            GameState gameState = TestGameState(5, 5);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);

            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;

            gameState.Set(new Tile[,]
            {
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,f,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_}
            });
            dut.Propagate(5);

            gameState.Set(new Tile[,]
            {
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_}
            });
            dut.Propagate(1);

            

            Assert.AreEqual(DistanceField<GameState.Tile>.Max, dut.GetDistance(2, 2));

            Assert.AreEqual(DistanceField<GameState.Tile>.Max, dut.GetDistance(3, 2));
            Assert.AreEqual(DistanceField<GameState.Tile>.Max, dut.GetDistance(4, 2));

            
        }

        

        [TestMethod]
        public void TestBasic_NotSquare()
        {
            GameState gameState = TestGameState(7, 5);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);

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

            gameState.Set(map);

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
            GameState gameState = TestGameState(7, 5);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);

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
            gameState.Set(map);
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
            GameState gameState = TestGameState(7, 5);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);

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

            gameState.Set(map);
            dut.Propagate(20);

            Assert.AreEqual(0, dut.GetDistance(2, 2));
            Assert.AreEqual(1, dut.GetDistance(2, 1));
            Assert.AreEqual(2, dut.GetDistance(2, 0));
            Assert.AreEqual(12, dut.GetDistance(6, 4));
        }

        

        [TestMethod]
        public void TestDescent()
        {
            GameState gameState = TestGameState(7, 5);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);

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

            gameState.Set(map);
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
            GameState gameState = TestGameState(7, 5);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);

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

            gameState.Set(map);
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
            GameState gameState = TestGameState(7, 10);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.terrain == GameState.Terrain.Unknown);

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

            gameState.Set(map);
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
            GameState gameState = TestGameState(7, 10);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.terrain == GameState.Terrain.Unknown);

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

            gameState.Set(map);
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
            GameState gameState = TestGameState(7, 10);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.terrain == GameState.Terrain.Unknown);

            const Tile u = Tile.Unseen;
            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;
            const Tile w = Tile.Water;

 


            gameState.Set(new Tile[,]
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

            gameState.Set(new Tile[,]
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


            gameState.Set(new Tile[,]
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
            GameState gameState = TestGameState(5, 5);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);

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

            gameState.Set(map);


            dut.Propagate(1);

            Assert.AreEqual(0, dut.GetDistance(2, 2));

            Assert.AreEqual(1, dut.GetDistance(3, 2));
            Assert.AreEqual(DistanceField<GameState.Tile>.Max, dut.GetDistance(4, 2));

            Assert.AreEqual(1, dut.GetDistance(2, 3));
            Assert.AreEqual(DistanceField<GameState.Tile>.Max, dut.GetDistance(2, 4));

            Assert.AreEqual(1, dut.GetDistance(2, 1));
            Assert.AreEqual(DistanceField<GameState.Tile>.Max, dut.GetDistance(2, 0));

            Assert.AreEqual(1, dut.GetDistance(1, 2));
            Assert.AreEqual(DistanceField<GameState.Tile>.Max, dut.GetDistance(0, 2));

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
            GameState gameState = TestGameState(7, 10);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);

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

            gameState.Set(map);
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
            GameState gameState = TestGameState(7, 10);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);

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

            gameState.Set(map);
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
            GameState gameState = TestGameState(7, 10);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isEnemyHill);

            const Tile f = Tile.Food;
            const Tile _ = Tile.Land;
            const Tile w = Tile.Water;
            const Tile h = Tile.TheirHill;
            const Tile u = Tile.Unseen;

            gameState.Set(new Tile[,]
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



            gameState.Set(new Tile[,]
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
            GameState gameState = TestGameState(5, 7);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);
            Vector2i result = dut.Wrap(new Vector2i(2, 3));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap2x()
        {
            GameState gameState = TestGameState(5, 7);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);
            Vector2i result = dut.Wrap(new Vector2i(5, 3));
            Assert.AreEqual(0, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap3x()
        {
            GameState gameState = TestGameState(5, 7);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);
            Vector2i result = dut.Wrap(new Vector2i(-1, 3));
            Assert.AreEqual(4, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap4x()
        {
            GameState gameState = TestGameState(5, 7);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);
            Vector2i result = dut.Wrap(new Vector2i(0, 3));
            Assert.AreEqual(0, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap5x()
        {
            GameState gameState = TestGameState(5, 7);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);
            Vector2i result = dut.Wrap(new Vector2i(4, 3));
            Assert.AreEqual(4, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap1y()
        {
            GameState gameState = TestGameState(5, 7);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);
            Vector2i result = dut.Wrap(new Vector2i(2, 3));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(3, result.y);
        }

        [TestMethod]
        public void TestWrap2y()
        {
            GameState gameState = TestGameState(5, 7);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);
            Vector2i result = dut.Wrap(new Vector2i(2, 7));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(0, result.y);
        }

        [TestMethod]
        public void TestWrap3y()
        {
            GameState gameState = TestGameState(5, 7);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);
            Vector2i result = dut.Wrap(new Vector2i(2, -1));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(6, result.y);
        }

        [TestMethod]
        public void TestWrap4y()
        {
            GameState gameState = TestGameState(5, 7);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);
            Vector2i result = dut.Wrap(new Vector2i(2, 0));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(0, result.y);
        }

        [TestMethod]
        public void TestWrap5y()
        {
            GameState gameState = TestGameState(5, 7);
            DistanceField<GameState.Tile> dut = new DistanceField<GameState.Tile>(gameState, gameState.map, tile => tile.isFood);
            Vector2i result = dut.Wrap(new Vector2i(2, 6));
            Assert.AreEqual(2, result.x);
            Assert.AreEqual(6, result.y);
        }







    }
}
