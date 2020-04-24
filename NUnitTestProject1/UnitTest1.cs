using Ants;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace Tests
{
    public class AntBehaviorTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetFood()
        {
            using (var consoleOutput = new ConsoleOutput())
            {

                var state = new GameState(32,32,int.MaxValue,100,25,5,1);
                var bot = new MyBot();


                state.StartNewTurn();
                state.AddAnt(16, 16, 0);
                state.AddFood(17, 16);

                Func<Location, Ant> test = x => state.MyAnts.Select(ant => (x.Row== ant.Row && x.Col ==ant.Col) ? ant : null).Where(y => y != null).FirstOrDefault();

                Assert.NotNull(test(state.MyAnts[0]));
                Assert.NotNull(test(new Ant(16,16,0)));
                Assert.NotNull(test(new Location(16,16)));

                var result = MyBot.FindClosest(state, state.FoodTiles[0] ,test , out Direction dir);
                Assert.NotNull(result);


                bot.DoTurn(state);

                Assert.AreEqual("o 16 16 s\r\n", consoleOutput.GetOuput());

                consoleOutput.Clear();


                state.StartNewTurn();
                state.AddAnt(16, 16, 0);
                state.AddFood(15, 16);

                bot.DoTurn(state);

                Assert.AreEqual("o 16 16 n\r\n", consoleOutput.GetOuput());
            }
        }

        [Test]
        public void TestApplyTurn()
        {

                    var state = new GameState(32, 32, int.MaxValue, 100, 25, 5, 1);
                    state.AddAnt(1, 2, 0);
                    state.AddAnt(2, 3, 0);

                    Assert.Contains(new Ant(1, 2, 0), state.MyAnts);
                    Assert.Contains(new Ant(2, 3, 0), state.MyAnts);

                    ApplyTurn(state, "o 1 2 s\r\no 2 3 n\r\n");

                    Assert.Contains(new Ant(2, 2, 0), state.MyAnts);
                    Assert.Contains(new Ant(1, 3, 0), state.MyAnts);

        }


        [Test]
        public void TestKillEnemy()
        {
            using (var consoleOutput = new ConsoleOutput())
            {

                var state = new GameState(32, 32, int.MaxValue, 100, 25, 5, 1);
                var bot = new MyBot();

                state.StartNewTurn();
                state.AddAnt(16, 15, 0);
                state.AddAnt(16, 16, 0);
                state.AddAnt(16, 17, 0);
                state.AddAnt(12, 16, 1);
                var initDist = state.MyAnts.Select(ant => state.EnemyAnts.Select(x => state.GetDistance(ant, x)).Min()).ToList();


                bot.DoTurn(state);

                ApplyTurn(state, consoleOutput.GetOuput());
                consoleOutput.Clear();



                //Assert.Contains(new Ant(15, 15, 0), state.MyAnts);
                //Assert.Contains(new Ant(15, 16, 0), state.MyAnts);
                //Assert.Contains(new Ant(15, 17, 0), state.MyAnts);




                bot.DoTurn(state);

                ApplyTurn(state, consoleOutput.GetOuput());
                consoleOutput.Clear();

                var finalDist = state.MyAnts.Select(ant => state.EnemyAnts.Select(x => state.GetDistance(ant, x)).Min()).ToList();

                for (int i = 0; i < initDist.Count; i++)
                {
                    Assert.Greater(initDist[i], finalDist[i]);
                }

                //Assert.Contains(new Ant(14, 15, 0), state.MyAnts);
                //Assert.Contains(new Ant(14, 16, 0), state.MyAnts);
                //Assert.Contains(new Ant(14, 17, 0), state.MyAnts);
            }
        }


        [Test]
        public void TestRegroup()
        {
            using (var consoleOutput = new ConsoleOutput())
            {

                var state = new GameState(32, 32, int.MaxValue, 100, 25, 5, 1);
                var bot = new MyBot();

                state.StartNewTurn();
                state.AddAnt(18, 15, 0);
                state.AddAnt(16, 16, 0);
                state.AddAnt(18, 17, 0);

                state.AddAnt(12, 15, 1);
                state.AddAnt(12, 17, 1);

                {//Extra test for platoon + BuildFrontLine
                    List<Ant> Platoon = new List<Ant>();
                    Battle.BuildPlatoon(state, Platoon, new Ant(16, 16, 0));
                    Assert.AreEqual(3, Platoon.Count);

                    List<Ant> EPlatoon = new List<Ant>();
                    Battle.BuildPlatoon(state, EPlatoon, new Ant(12, 15, 1));
                    Assert.AreEqual(2, EPlatoon.Count);

                    var AllyDist = Battle.BuildFrontline(state, Platoon, EPlatoon, out int qty, out int distance);
                    Assert.AreEqual(1,qty);
                    Assert.AreEqual(6, AllyDist[Platoon.IndexOf(new Ant(18,15,0))]);
                    Assert.AreEqual(5, AllyDist[Platoon.IndexOf(new Ant(16,16,0))]);
                    Assert.AreEqual(5, distance);

                    var EnemyDist = Battle.BuildFrontline(state,EPlatoon , Platoon, out int Eqty, out int Edistance);
                    Assert.AreEqual(2, Eqty);

                }

                bot.DoTurn(state);
                consoleOutput.originalOutput.Write(consoleOutput.GetOuput());
                ApplyTurn(state, consoleOutput.GetOuput());
                consoleOutput.Clear();

                Assert.Contains(new Ant(17, 15, 0), state.MyAnts);
                Assert.Contains(new Ant(16, 16, 0), state.MyAnts);
                Assert.Contains(new Ant(17, 17, 0), state.MyAnts);

            }
        }

        [Test]
        public void TestRetreat()
        {
            using (var consoleOutput = new ConsoleOutput())
            {

                var state = new GameState(32, 32, int.MaxValue, 100, 25, 5, 1);
                var bot = new MyBot();
                
                state.StartNewTurn();
                state.AddAnt(16, 15, 0);
                state.AddAnt(12, 16, 1);
                state.AddAnt(12, 15, 1);
                state.AddAnt(12, 17, 1);

                var initDist = state.EnemyAnts.Select(x => state.GetDistance(state.MyAnts[0], x)).ToList();

                bot.DoTurn(state);

                ApplyTurn(state, consoleOutput.GetOuput());
                consoleOutput.Clear();


                var finalDist = state.EnemyAnts.Select(x => state.GetDistance(state.MyAnts[0], x)).ToList();


                for (int i = 0; i < initDist.Count; i++)
                {
                    Assert.Greater(finalDist[i], initDist[i]);
                }

            }
        }

        [Test]
        public void TestBestDir()
        {

            const int mapSize = 32;

            var state = new GameState(mapSize, mapSize, int.MaxValue, 100, 25, 5, 1);
            var baseLoc = new Location(5, 8);
            {
                var loc2 = new Location(7, 9);

                Assert.AreEqual(1, state.getHorizontalDelta(baseLoc, loc2));
                Assert.AreEqual(2, state.getVerticalDelta(baseLoc, loc2));
                Assert.AreEqual(-1, state.getHorizontalDelta(loc2, baseLoc));
                Assert.AreEqual(-2, state.getVerticalDelta(loc2, baseLoc));
            }

            {
                var loc3 = new Location(1, 2);
                var loc4 = new Location(mapSize - 1, mapSize - 3);

                Assert.AreEqual(5, state.getHorizontalDelta(loc3, loc4));
                Assert.AreEqual(2, state.getVerticalDelta(loc3, loc4));
                Assert.AreEqual(-5, state.getHorizontalDelta(loc4, loc3));
                Assert.AreEqual(-2, state.getVerticalDelta(loc4, loc3));
            }


            foreach (var dir in Ants.Ants.Aim.Keys)
            {
                var loc = state.GetDestination(baseLoc, dir);
                Assert.AreEqual(dir, state.GetBestDirection(baseLoc, loc));
            }

           
        }

        public class ConsoleOutput : IDisposable
        {
            private StringWriter stringWriter;
            public readonly TextWriter originalOutput;

            public ConsoleOutput()
            {
                stringWriter = new StringWriter();
                originalOutput = Console.Out;
                Console.SetOut(stringWriter);
            }
            public void Clear()
            {
                var buf = stringWriter.GetStringBuilder();
                buf.Clear();
            }

            public string GetOuput()
            {
                return stringWriter.ToString();
            }

            public void Dispose()
            {
                Console.SetOut(originalOutput);
                stringWriter.Dispose();
            }
        }

        public static void  ApplyTurn(GameState state, string cmds)
        {
            //Backup

            var backup = new Backup(state);



            string pat = @"o\s(\d+)\s(\d+)\s(\w)";
            Regex r = new Regex(pat, RegexOptions.IgnoreCase);

            Match m = r.Match(cmds);
            while (m.Success)
            {
                int row = int.Parse( m.Groups[1].Captures[0].Value);
                int col = int.Parse( m.Groups[2].Captures[0].Value);
                var ant = new Ant(row, col,0);


                var dir = DirectionExtensions.FromChar(m.Groups[3].Captures[0].Value[0]);

                MoveAnt(backup,state, ant, dir);

                m = m.NextMatch();

            }

            backup.Restore(state);
        }

        public class Backup
        {
            public List<Ant> Ants = new List<Ant>();
            public List<AntHill> Hills = new List<AntHill>();
            public List<Location> Food = new List<Location>();

            public Backup(GameState state)
            {
                Ants.AddRange(state.MyAnts);
                Ants.AddRange(state.EnemyAnts);
                Hills.AddRange(state.MyHills);
                Hills.AddRange(state.EnemyHills);
                Food.AddRange(state.FoodTiles);
            }

            public void Restore(GameState state)
            {
                state.StartNewTurn();
                foreach (var loc in Ants) state.AddAnt(loc.Row, loc.Col, loc.Team);
                foreach (var loc in Hills) state.AntHill(loc.Row, loc.Col, loc.Team);
                foreach (var loc in Food) state.AddFood(loc.Row, loc.Col);
            }
        }

        public static void MoveAnt(Backup backup, GameState state, Ant ant, Direction dir)
        {



            if (backup.Ants.Remove(ant))
            {
                var loc = state.GetDestination(ant, dir);
                backup.Ants.Add(new Ant(loc, ant.Team));
                Console.WriteLine(ant.ToString() + dir + " => " + loc);
            }
            else
                throw new Exception("Attemtp to move unexisting ant!");

        }

        public static void displayMatch(Match m, int matchCount = -1)
        {
            if(matchCount!=-1) Console.WriteLine("Match" + matchCount);

            for (int i = 1; i < m.Groups.Count; i++)
            {
                Group g = m.Groups[i];
                Console.WriteLine("Group" + i + "='" + g + "'");
                CaptureCollection cc = g.Captures;
                for (int j = 0; j < cc.Count; j++)
                {
                    Capture c = cc[j];
                    System.Console.WriteLine("Capture" + j + "='" + c + "', Position=" + c.Index);
                }
            }
        }
        public static int displayAllMatch(Match m)
        {
            int matchCount = 0;
            while (m.Success)
            {
                displayMatch(m,++matchCount );
                m = m.NextMatch();
            }
            return matchCount;
        }
    }
}