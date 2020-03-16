using NUnit.Framework;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveNodeGroupTest
    {
        [TestCase(0, 0, 0.3835f)]
        [TestCase(0, 1, 0.3835f)]
        [TestCase(1, 0, 0.3835f)]
        [TestCase(1, 1, 0.3835f)]
        public void JsonPassiveNodeGroup_Position(float x, float y, float zoomLevel)
        {
            var group = new JsonPassiveNodeGroup()
            {
                OriginalX = x,
                OriginalY = y,
                ZoomLevel = zoomLevel,
            };

            Assert.AreEqual(x * zoomLevel, group.Position.X);
            Assert.AreEqual(y * zoomLevel, group.Position.Y);
        }
    }
}
