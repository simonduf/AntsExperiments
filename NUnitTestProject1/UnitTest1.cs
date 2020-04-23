using Ants;
using NUnit.Framework;
using System;
using System.IO;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            using (var consoleOutput = new ConsoleOutput())
            {

                var state = new GameState(32,32,int.MaxValue,100,25,5,1);
                var bot = new MyBot();


                state.StartNewTurn();
                state.AddAnt(16, 16, 0);
                state.AddFood(17, 16);

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


        public class ConsoleOutput : IDisposable
        {
            private StringWriter stringWriter;
            private TextWriter originalOutput;

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
    }
}