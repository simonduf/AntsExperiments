using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants;
using System;
using System.Linq;

namespace Bot1_Tests
{
    [TestClass]
    public class TacticalMapTests
    {


        [TestMethod]
        public void TestTacticalMap_1Enemy()
        {
            TacticalMap dut = new TacticalMap(20,20,5);

            dut.AddEnemy(new Vector2i(10, 10), 0);

            Assert.AreEqual(1, dut[10, 10].enemies.Count);
            Assert.AreEqual(1.0f, dut[10, 10].enemies[0].probability, 0.001f);
            
            Assert.AreEqual(1.0f, dut[9, 10].enemies[0].probability, 0.001f);
            Assert.AreEqual(0.8f, dut[8, 10].enemies[0].probability, 0.001f);
            Assert.AreEqual(0.2f, dut[7, 10].enemies[0].probability, 0.001f);

            Assert.AreEqual(1.0f, dut[10, 9].enemies[0].probability, 0.001f);
            Assert.AreEqual(0.8f, dut[10, 8].enemies[0].probability, 0.001f);
            Assert.AreEqual(0.2f, dut[10, 7].enemies[0].probability, 0.001f);


        }

        [TestMethod]
        public void TestTacticalMap_2Enemy()
        {
            TacticalMap dut = new TacticalMap(20, 20, 5);

            dut.AddEnemy(new Vector2i(10, 10), 0);
            dut.AddEnemy(new Vector2i(12, 10), 1);

            Assert.AreEqual(2, dut[10, 10].enemies.Count);
            Assert.AreEqual(2, dut[12, 10].enemies.Count);
            Assert.AreEqual(2, dut[9, 10].enemies.Count);
            Assert.AreEqual(1, dut[8, 10].enemies.Count);
            

            Assert.AreEqual(1.0f, dut[10, 10].enemies.Where(e => e.enemyId == 0).Single().probability, 0.001f);
            Assert.AreEqual(1.0f, dut[9, 10].enemies.Where(e => e.enemyId == 0).Single().probability, 0.001f);
            Assert.AreEqual(0.8f, dut[8, 10].enemies.Where(e => e.enemyId == 0).Single().probability, 0.001f);
            Assert.AreEqual(0.2f, dut[7, 10].enemies.Where(e => e.enemyId == 0).Single().probability, 0.001f);

            Assert.AreEqual(1.0f, dut[10, 9].enemies.Where(e => e.enemyId == 0).Single().probability, 0.001f);
            Assert.AreEqual(0.8f, dut[10, 8].enemies.Where(e => e.enemyId == 0).Single().probability, 0.001f);
            Assert.AreEqual(0.2f, dut[10, 7].enemies.Where(e => e.enemyId == 0).Single().probability, 0.001f);

            Assert.AreEqual(1.0f, dut[12, 10].enemies.Where(e => e.enemyId == 1).Single().probability, 0.001f);
            Assert.AreEqual(1.0f, dut[11, 10].enemies.Where(e => e.enemyId == 1).Single().probability, 0.001f);
            Assert.AreEqual(0.8f, dut[10, 10].enemies.Where(e => e.enemyId == 1).Single().probability, 0.001f);
            Assert.AreEqual(0.2f, dut[9, 10].enemies.Where(e => e.enemyId == 1).Single().probability, 0.001f);

            Assert.AreEqual(1.0f, dut[12, 9].enemies.Where(e => e.enemyId == 1).Single().probability, 0.001f);
            Assert.AreEqual(0.8f, dut[12, 8].enemies.Where(e => e.enemyId == 1).Single().probability, 0.001f);
            Assert.AreEqual(0.2f, dut[12, 7].enemies.Where(e => e.enemyId == 1).Single().probability, 0.001f);


        }


    }



}
