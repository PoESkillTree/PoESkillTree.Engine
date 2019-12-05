using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Linq;

namespace PoESkillTree.Engine.GameModel.PassiveTree
{
    [TestFixture]
    public class PassiveTreeDeserializationTest
    {
        [Test]
        public void PassiveTreeDeserializerWorks()
        {
            var NUM_TESTS = 100;
            var sw = new Stopwatch();
            var times = new long[NUM_TESTS];
            var json = TestUtils.ReadDataFile("skilltree_3.8.0.min.json");
            Assert.IsNotEmpty(json);
            
            for (var i = 0; i < NUM_TESTS; i++)
            {
                sw.Restart();
                var passiveTree = JsonConvert.DeserializeObject<JsonPassiveTree>(json);
                sw.Stop();
                times[i] = sw.ElapsedMilliseconds;
            }

            Console.WriteLine($"First: {times[0]}, Last: {times[NUM_TESTS - 1]}, Average: {times.Average()}");
        }

        [TestCase(10f, 10f, 10f, 10f)]
        [TestCase(10f, 10f, 10f, -10f)]
        [TestCase(10f, 10f, -10f, 10f)]
        [TestCase(10f, -10f, 10f, 10f)]
        [TestCase(-10f, 10f, 10f, 10f)]
        [TestCase(-10f, 10f, 10f, -10f)]
        [TestCase(-10f, 10f, -10f, 10f)]
        [TestCase(-10f, -10f, 10f, 10f)]
        [TestCase(-10f, -10f, 10f, -10f)]
        [TestCase(-10f, -10f, -10f, 10f)]
        [TestCase(-10f, -10f, -10f, -10f)]
        public void PassiveTreeBounds(float minX, float minY, float maxX, float maxY)
        {
            var passiveTree = new JsonPassiveTree();
            passiveTree.MinX = minX;
            passiveTree.MinY = minY;
            passiveTree.MaxX = maxX;
            passiveTree.MaxY = maxY;

            Assert.AreEqual(passiveTree.MinX, passiveTree.Bounds.Left, $"{nameof(passiveTree.MinX)} doesn't equal {nameof(passiveTree.Bounds)}.{nameof(passiveTree.Bounds.Left)}");
            Assert.AreEqual(passiveTree.MinY, passiveTree.Bounds.Top, $"{nameof(passiveTree.MinY)} doesn't equal {nameof(passiveTree.Bounds)}.{nameof(passiveTree.Bounds.Top)}");
            Assert.AreEqual(passiveTree.MaxX, passiveTree.Bounds.Right, $"{nameof(passiveTree.MaxX)} doesn't equal {nameof(passiveTree.Bounds)}.{nameof(passiveTree.Bounds.Right)}");
            Assert.AreEqual(passiveTree.MaxY, passiveTree.Bounds.Bottom, $"{nameof(passiveTree.MaxY)} doesn't equal {nameof(passiveTree.Bounds)}.{nameof(passiveTree.Bounds.Bottom)}");
        }
    }
}