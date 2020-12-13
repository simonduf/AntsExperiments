using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants;
using System;

namespace Bot1_Tests
{
    [TestClass]
    public class ExplorationMapTests
    {
        const bool v = true;
        const bool _ = false;

        ExplorationMap.Configuration defaultConfig = new ExplorationMap.Configuration()
        {
            width = 5,
            height = 5,
            noCriticalStartup = 5,
            visitDelay = 10,
        };

        [TestMethod]
        public void TestBasicExploration()
        {
            bool[,] map = new bool[5, 5];
            ExplorationMap.Configuration config = defaultConfig;
            config.visibilityCheck = v => map[v.y, v.x];
            ExplorationMap dut = new ExplorationMap(config);

            Copy(new bool[5,5]
            {
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            },map,5,5);


            dut.Update();
            Assert.IsTrue(dut.map[2, 2].needsVisit);

            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,v,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);


            dut.Update();
            Assert.IsFalse(dut.map[2, 2].needsVisit);

            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);


            for (int i = 0; i < 10; i++)
            {
                dut.Update();
                Assert.IsFalse(dut.map[2, 2].needsVisit);
            }

            dut.Update();
            Assert.IsTrue(dut.map[2, 2].needsVisit);

        }



        [TestMethod]
        public void TestCritical()
        {
            bool[,] map = new bool[5, 5];
            ExplorationMap.Configuration config = defaultConfig;
            config.visibilityCheck = v => map[v.y, v.x];
            ExplorationMap dut = new ExplorationMap(config);

            dut.SetCriticalZone(new Vector2i(3,2), 1);

            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);

            dut.Update();
            Assert.IsTrue(dut.map[3, 2].needsVisit);
            Assert.IsTrue(dut.map[3, 1].needsVisit);
            Assert.IsTrue(dut.map[1, 1].needsVisit);


            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,v,_,v,_},
                {_,_,_,v,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);

            dut.Update();
            Assert.IsFalse(dut.map[3, 2].needsVisit);
            Assert.IsFalse(dut.map[3, 1].needsVisit);
            Assert.IsFalse(dut.map[1, 1].needsVisit);

            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,v,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);

            dut.Update();
            Assert.IsFalse(dut.map[3, 2].needsVisit);
            Assert.IsFalse(dut.map[3, 1].needsVisit);
            Assert.IsFalse(dut.map[1, 1].needsVisit);


            for (int i = 0; i < 2; i++)
                dut.Update();

            //
            //      CRITICAL ZONE ACTIVATES HERE
            //

            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);

            dut.Update();
            Assert.IsTrue(dut.map[3, 2].needsVisit);
            Assert.IsTrue(dut.map[3, 1].needsVisit);
            Assert.IsFalse(dut.map[1, 1].needsVisit);


            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,v,_,v,_},
                {_,_,_,v,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);

            dut.Update();
            Assert.IsFalse(dut.map[3, 2].needsVisit);
            Assert.IsFalse(dut.map[3, 1].needsVisit);
            Assert.IsFalse(dut.map[1, 1].needsVisit);

            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,v,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);

            dut.Update();
            Assert.IsTrue(dut.map[3, 2].needsVisit);
            Assert.IsTrue(dut.map[3, 1].needsVisit);
            Assert.IsFalse(dut.map[1, 1].needsVisit);


            for (int i = 0; i < 2; i++)
                dut.Update();


        }


        [TestMethod]
        public void TestOnlyCriticalZone()
        {
            bool[,] map = new bool[5, 5];
            ExplorationMap.Configuration config = defaultConfig;
            config.visibilityCheck = v => map[v.y, v.x];
            config.visitDelay = int.MaxValue;
            ExplorationMap dut = new ExplorationMap(config);

            dut.SetCriticalZone(new Vector2i(3, 2), 1);

            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);

            dut.Update();
            Assert.IsFalse(dut.map[3, 2].needsVisit);
            Assert.IsFalse(dut.map[3, 1].needsVisit);
            Assert.IsFalse(dut.map[1, 1].needsVisit);


            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,v,_,v,_},
                {_,_,_,v,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);

            dut.Update();
            Assert.IsFalse(dut.map[3, 2].needsVisit);
            Assert.IsFalse(dut.map[3, 1].needsVisit);
            Assert.IsFalse(dut.map[1, 1].needsVisit);

            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,v,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);

            dut.Update();
            Assert.IsFalse(dut.map[3, 2].needsVisit);
            Assert.IsFalse(dut.map[3, 1].needsVisit);
            Assert.IsFalse(dut.map[1, 1].needsVisit);


            for (int i = 0; i < 2; i++)
                dut.Update();

            //
            //      CRITICAL ZONE ACTIVATES HERE
            //

            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);

            dut.Update();
            Assert.IsTrue(dut.map[3, 2].needsVisit);
            Assert.IsTrue(dut.map[3, 1].needsVisit);
            Assert.IsFalse(dut.map[1, 1].needsVisit);


            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,v,_,v,_},
                {_,_,_,v,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);

            dut.Update();
            Assert.IsFalse(dut.map[3, 2].needsVisit);
            Assert.IsFalse(dut.map[3, 1].needsVisit);
            Assert.IsFalse(dut.map[1, 1].needsVisit);

            Copy(new bool[5, 5]
            {
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
                {_,_,_,_,_},
            }, map, 5, 5);

            dut.Update();
            Assert.IsTrue(dut.map[3, 2].needsVisit);
            Assert.IsTrue(dut.map[3, 1].needsVisit);
            Assert.IsFalse(dut.map[1, 1].needsVisit);


            for (int i = 0; i < 2; i++)
                dut.Update();

        }

        [TestMethod]
        public void TestCriticalWrap()
        {

            
            bool[,] map = new bool[5, 5];
            ExplorationMap.Configuration config = defaultConfig;
            config.visibilityCheck = v => map[v.x, v.y];
            ExplorationMap dut = new ExplorationMap(config);

            dut.SetCriticalZone(new Vector2i(2, 0), 1);

            Assert.IsTrue(dut.map[2, 4].isCritical);
            Assert.IsTrue(dut.map[2, 0].isCritical);



        }


        public static void Copy(bool[,] src, bool[,] dst, int width, int height)
        {
            var coords = Vector2i.GenerateCoords(Vector2i.Zero, new Vector2i(width, height));

            foreach (var c in coords)
                dst[c.x, c.y] = src[c.x, c.y];
        }


    }



}
