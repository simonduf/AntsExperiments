using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ants;
using System;

namespace Bot1_Tests
{
    [TestClass]
    public class Vector2iTests
    {
        [TestMethod]
        public void TestSum()
        {
            Vector2i a = new Vector2i(1, 2);
            Vector2i b = new Vector2i(2, 3);
            Vector2i result = a + b;
            Assert.AreEqual(3, result.x);
            Assert.AreEqual(5, result.y);
        }

        [TestMethod]
        public void TestSub()
        {
            Vector2i a = new Vector2i(1, 2);
            Vector2i b = new Vector2i(2, 4);
            Vector2i result = a - b;
            Assert.AreEqual(-1, result.x);
            Assert.AreEqual(-2, result.y);
        }

        [TestMethod]
        public void TestCenter1()
        {
            Vector2i a = new Vector2i(0, 0);
            Vector2i b = new Vector2i(5, 7);
            Vector2i result = a.Centered(b);
            Assert.AreEqual(0, result.x);
            Assert.AreEqual(0, result.y);
        }

        [TestMethod]
        public void TestCenter2()
        {
            Vector2i a = new Vector2i(-1, -2);
            Vector2i b = new Vector2i(5, 7);
            Vector2i result = a.Centered(b);
            Assert.AreEqual(-1, result.x);
            Assert.AreEqual(-2, result.y);
        }

        [TestMethod]
        public void TestCenter3()
        {
            Vector2i a = new Vector2i(4, 6);
            Vector2i b = new Vector2i(5, 7);
            Vector2i result = a.Centered(b);
            Assert.AreEqual(-1, result.x);
            Assert.AreEqual(-1, result.y);
        }

        [TestMethod]
        public void TestEquals()
        {
            Vector2i a = new Vector2i(1, 2);
            Vector2i b = new Vector2i(1, 2);
            Vector2i c = new Vector2i(1, 3);
            object d = new Vector2i(1, 2);
            object e = new Vector2i(1, 3);
            object f = 5.0f;


            Assert.AreEqual(a, b);
            Assert.AreNotEqual(a, c);
            Assert.AreEqual(a, d);
            Assert.AreNotEqual(a, e);
            Assert.AreNotEqual(a, f);
        }

    }
}
