using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants;
using System;

namespace Bot1_Tests
{
    [TestClass]
    public class Vector2Tests
    {
        [TestMethod]
        public void TestMagnitude1()
        {
            Vector2 dut = new Vector2(2, 2);
            Assert.AreEqual(2*1.414, dut.magnitude, 0.002);
        }

        [TestMethod]
        public void TestMagnitude2()
        {
            Vector2 dut = new Vector2(1, 1);
            Assert.AreEqual(1.414, dut.magnitude, 0.002);
            //Assert.Fail();
        }

        [TestMethod]
        public void TestMagnitude3()
        {
            Vector2 dut = new Vector2(3, 0);
            Assert.AreEqual(3.0, dut.magnitude, 0.002);
        }

        [TestMethod]
        public void TestMagnitude4()
        {
            Vector2 dut = new Vector2(0, 4);
            Assert.AreEqual(4.0, dut.magnitude, 0.002);
            //Assert.Fail();
        }


        


    }
}
