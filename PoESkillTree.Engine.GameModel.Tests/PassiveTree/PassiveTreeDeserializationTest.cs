using Newtonsoft.Json;
using NUnit.Framework;
using PoESkillTree.Engine.GameModel.PassiveTree.Base;
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
            var NUM_TESTS = 1;
            var sw = new Stopwatch();
            var times = new long[NUM_TESTS];
            var newJson = TestUtils.ReadDataFile("skilltree_3.10.0_new.min.json");
            Assert.IsNotEmpty(newJson);

            var oldJson = TestUtils.ReadDataFile("skilltree_3.10.0_old.min.json");
            Assert.IsNotEmpty(oldJson);

            for (var i = 0; i < NUM_TESTS; i++)
            {
                sw.Restart();
                var newPassiveTree = JsonConvert.DeserializeObject<JsonPassiveTree>(newJson);
                Console.WriteLine(JsonConvert.SerializeObject(newPassiveTree));

                var oldPassiveTree = JsonConvert.DeserializeObject<JsonPassiveTree>(oldJson);
                Console.WriteLine(JsonConvert.SerializeObject(oldPassiveTree));
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