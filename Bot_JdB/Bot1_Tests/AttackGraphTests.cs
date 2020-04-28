using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants;
using System;
using System.Collections.Generic;

namespace Bot1_Tests
{
    [TestClass]
    public class AttackGraphTests
    {

        List<AttackGraph.LinkOption> options = new List<AttackGraph.LinkOption>(5);
        [TestInitialize]
        public void Initialize()
        {
            for(int i = 0; i < 5; i++)
                options.Add(new AttackGraph.LinkOption());

            options[(int)Direction.Halt] = new AttackGraph.LinkOption()
            {
                available = true,
                destination = Vector2i.Zero,
                probability = 0.5f,
            };

            options[(int)Direction.North] = new AttackGraph.LinkOption()
            {
                available = true,
                destination = Vector2i.Up,
                probability = 1.0f,
            };

            options[(int)Direction.West] = new AttackGraph.LinkOption()
            {
                available = true,
                destination = Vector2i.Left,
                probability = 0.25f,
            };

            options[(int)Direction.East] = new AttackGraph.LinkOption()
            {
                available = true,
                destination = Vector2i.Right,
                probability = 0.75f,
            };

            options[(int)Direction.South] = new AttackGraph.LinkOption()
            {
                available = true,
                destination = Vector2i.Down,
                probability = 0.0f,
            };
        }

        public AttackGraph Setup1v1()
        {
            AttackGraph result = new AttackGraph();

            result.AddNodeGroup(0, 0, options);

            return result;
        }

        public AttackGraph Setup2v1()
        {
            AttackGraph result = new AttackGraph();

            result.AddNodeGroup(0, 0, options);
            result.AddNodeGroup(1, 0, options);

            return result;
        }


        [TestMethod]
        public void TestGetEngagement_1v1()
        {
            var dut = Setup1v1();


            dut.SetDirection(0, Direction.North);
            Assert.AreEqual(1.0f, dut.GetEngagement(0), 0.001f);

            dut.SetDirection(0, Direction.Halt);
            Assert.AreEqual(0.5f, dut.GetEngagement(0), 0.001f);

            dut.SetDirection(0, Direction.West);
            Assert.AreEqual(0.25f, dut.GetEngagement(0), 0.001f);

            dut.SetDirection(0, Direction.East);
            Assert.AreEqual(0.75f, dut.GetEngagement(0), 0.001f);

            dut.SetDirection(0, Direction.South);
            Assert.AreEqual(0.0f, dut.GetEngagement(0), 0.001f);
        }

        [TestMethod]
        public void TestGetSurvivability_1v1()
        {
            var dut = Setup1v1();

            dut.SetDirection(0, Direction.North);
            Assert.AreEqual(0.0, dut.GetMySurvivalRate(0));
            Assert.AreEqual(0.0, dut.GetTheirSurvivalRate(0));

            dut.SetDirection(0, Direction.Halt);
            Assert.AreEqual(0.5f, dut.GetMySurvivalRate(0), 0.001f);
            Assert.AreEqual(0.5f, dut.GetTheirSurvivalRate(0), 0.001f);
            
            dut.SetDirection(0, Direction.West);
            Assert.AreEqual(0.75f, dut.GetMySurvivalRate(0), 0.001f);
            Assert.AreEqual(0.75f, dut.GetTheirSurvivalRate(0), 0.001f);
            
            dut.SetDirection(0, Direction.South);
            Assert.AreEqual(1.0f, dut.GetMySurvivalRate(0), 0.001f);
            Assert.AreEqual(1.0f, dut.GetTheirSurvivalRate(0), 0.001f);
        }

        [TestMethod]
        public void TestGetSurvivability_2v1()
        {
            var dut = Setup2v1();

            dut.SetDirection(0, Direction.North);
            dut.SetDirection(1, Direction.North);
            Assert.AreEqual(1.0f, dut.GetMySurvivalRate(0), 0.001f);
            Assert.AreEqual(1.0f, dut.GetMySurvivalRate(1), 0.001f);
            Assert.AreEqual(0.0f, dut.GetTheirSurvivalRate(0), 0.001f);
            Assert.AreEqual(AttackGraph.KillValue, dut.GetValue(), 0.001f);

            dut.SetDirection(0, Direction.Halt);
            dut.SetDirection(1, Direction.North);
            Assert.AreEqual(1.0f, dut.GetMySurvivalRate(0), 0.001f);
            Assert.AreEqual(0.5f, dut.GetMySurvivalRate(1), 0.001f);
            Assert.AreEqual(0.0f, dut.GetTheirSurvivalRate(0), 0.001f);
            Assert.AreEqual(AttackGraph.KillValue + 0.5f * AttackGraph.LossValue, dut.GetValue(), 0.001f);

            dut.SetDirection(0, Direction.Halt);
            dut.SetDirection(1, Direction.Halt);
            Assert.AreEqual(0.75f, dut.GetMySurvivalRate(0), 0.001f);
            Assert.AreEqual(0.75f, dut.GetMySurvivalRate(1), 0.001f);
            Assert.AreEqual(0.25f, dut.GetTheirSurvivalRate(0), 0.001f);
            Assert.AreEqual(0.75f * AttackGraph.KillValue + 0.5f * AttackGraph.LossValue, dut.GetValue(), 0.001f);

            dut.SetDirection(0, Direction.West);
            dut.SetDirection(1, Direction.Halt);
            Assert.AreEqual(0.875f, dut.GetMySurvivalRate(0), 0.001f);
            Assert.AreEqual(0.625f, dut.GetMySurvivalRate(1), 0.001f);
            Assert.AreEqual(0.375f, dut.GetTheirSurvivalRate(0), 0.001f);

        }

        [TestMethod]
        public void TestGetProjectedPosition()
        {
            AttackGraph dut = Setup1v1();

            dut.SetDirection(0, Direction.North);
            Assert.AreEqual(new Vector2i(0, -1), dut.GetProjectedPosition(0));
            Assert.AreEqual(Direction.North, dut.GetDirection(0));

            dut.SetDirection(0, Direction.Halt);
            Assert.AreEqual(new Vector2i(0, 0), dut.GetProjectedPosition(0));
            Assert.AreEqual(Direction.Halt, dut.GetDirection(0));

            dut.SetDirection(0, Direction.West);
            Assert.AreEqual(new Vector2i(-1, 0), dut.GetProjectedPosition(0));
            Assert.AreEqual(Direction.West, dut.GetDirection(0));


        }





    }
}
