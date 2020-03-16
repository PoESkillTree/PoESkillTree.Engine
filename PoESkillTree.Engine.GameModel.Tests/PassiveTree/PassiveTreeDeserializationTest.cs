using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using PoESkillTree.Engine.GameModel.PassiveTree.Base;

namespace PoESkillTree.Engine.GameModel.PassiveTree
{
    [TestFixture]
    public class PassiveTreeDeserializationTest
    {
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
            var json = $"{{ \"min_x\": {minX}, \"min_y\": {minY}, \"max_x\": {maxX}, \"max_y\": {maxY} }}";
            var passiveTree = JsonConvert.DeserializeObject<JsonPassiveTree>(json);

            Assert.AreEqual(passiveTree.MinX, passiveTree.Bounds.Left, $"{nameof(passiveTree.MinX)} doesn't equal {nameof(passiveTree.Bounds)}.{nameof(passiveTree.Bounds.Left)}");
            Assert.AreEqual(passiveTree.MinY, passiveTree.Bounds.Top, $"{nameof(passiveTree.MinY)} doesn't equal {nameof(passiveTree.Bounds)}.{nameof(passiveTree.Bounds.Top)}");
            Assert.AreEqual(passiveTree.MaxX, passiveTree.Bounds.Right, $"{nameof(passiveTree.MaxX)} doesn't equal {nameof(passiveTree.Bounds)}.{nameof(passiveTree.Bounds.Right)}");
            Assert.AreEqual(passiveTree.MaxY, passiveTree.Bounds.Bottom, $"{nameof(passiveTree.MaxY)} doesn't equal {nameof(passiveTree.Bounds)}.{nameof(passiveTree.Bounds.Bottom)}");
        }

        [Test]
        public void JsonPassiveNodeGroup_AssignedToPassiveNode()
        {
            var json = "{\"groups\":{\"1\":{\"x\":0,\"y\":0,\"orbits\":[0],\"nodes\":[\"1\"]}},\"nodes\":{\"1\":{\"skill\":1,\"name\":\"test_name\",\"icon\":\"test_icon\",\"stats\":[\"test_stat_1\",\"test_stat_2\"],\"group\":1,\"orbit\":0,\"orbitIndex\":0,\"out\":[],\"in\":[]}}}";
            var passiveTree = JsonConvert.DeserializeObject<JsonPassiveTree>(json);

            Assert.AreEqual(1, passiveTree.PassiveNodeGroups.Count);
            Assert.AreEqual(1, passiveTree.PassiveNodes.Count);
            Assert.IsTrue(passiveTree.PassiveNodeGroups[1] == passiveTree.PassiveNodes[1].PassiveNodeGroup);
        }
    }
}