using NUnit.Framework;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Options
{
    public class JsonJewelHighlightTest
    {
        [TestCase(100, 0.5f)]
        [TestCase(10, 0.3835f)]
        [TestCase(5, 0.25f)]
        public void JsonJewelHighlight(int diameter, float zoomLevel)
        {
            var jewelHighlight = new JsonJewelHighlight
            {
                Diameter = diameter,
                ZoomLevel = zoomLevel
            };

            Assert.AreEqual(diameter, jewelHighlight.Diameter);
            Assert.AreEqual(diameter / 2, jewelHighlight.Radius);
            Assert.AreEqual(diameter / zoomLevel, jewelHighlight.ZoomLevelDiameter);
            Assert.AreEqual(diameter / zoomLevel / 2, jewelHighlight.ZoomLevelRadius);
        }

    }
}
