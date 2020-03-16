using NUnit.Framework;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreeExtraImageTest
    {
        [TestCase(0, 0, 0.3835f)]
        [TestCase(0, 1, 0.3835f)]
        [TestCase(1, 0, 0.3835f)]
        [TestCase(1, 1, 0.3835f)]
        public void JsonPassiveTreeExtraImage_Position(float x, float y, float zoomLevel)
        {
            var extraImage = new JsonPassiveTreeExtraImage()
            {
                OriginalX = x,
                OriginalY = y,
                ZoomLevel = zoomLevel,
            };

            Assert.AreEqual(x * zoomLevel, extraImage.Position.X);
            Assert.AreEqual(y * zoomLevel, extraImage.Position.Y);
        }
    }
}
