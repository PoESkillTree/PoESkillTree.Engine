using NUnit.Framework;
using System.Drawing;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Options
{
    public class JsonAscendancyClassOptionTest
    {
        [TestCase(25f, 25f, 50f, 50f)]
        [TestCase(0.25f, 0.50f, 150f, 50f)]
        [TestCase(0.50f, 0.25f, 100f, 50f)]
        public void JsonAscendancyClassOption_FlavourTextBounds(float x, float y, float width, float height)
        {
            var ascendancy = new JsonAscendancyClassOption
            {
                FlavourTextBoundsString = $"{x},{y},{width},{height}"
            };

            Assert.AreEqual(x, ascendancy.FlavourTextBounds.X);
            Assert.AreEqual(y, ascendancy.FlavourTextBounds.Y);
            Assert.AreEqual(width, ascendancy.FlavourTextBounds.Width);
            Assert.AreEqual(height, ascendancy.FlavourTextBounds.Height);
        }

        [TestCase("50,50,50,50,50")]
        [TestCase("50,50,50")]
        [TestCase("50,50")]
        [TestCase("50")]
        [TestCase("")]
        public void JsonAscendancyClassOption_FlavourTextBounds_Empty(string boundsString)
        {
            var ascendancy = new JsonAscendancyClassOption
            {
                FlavourTextBoundsString = boundsString
            };

            Assert.AreEqual(RectangleF.Empty, ascendancy.FlavourTextBounds);
        }

        [TestCase(128, 128, 128)]
        [TestCase(128, 128, 128)]
        [TestCase(128, 128, 128)]
        public void JsonAscendancyClassOption_FlavourTextColour(byte red, byte green, byte blue)
        {
            var ascendancy = new JsonAscendancyClassOption
            {
                FlavourTextColourString = $"{red},{green},{blue}"
            };

            Assert.AreEqual(red, ascendancy.FlavourTextColour.R);
            Assert.AreEqual(green, ascendancy.FlavourTextColour.G);
            Assert.AreEqual(blue, ascendancy.FlavourTextColour.B);
        }

        [TestCase("128,128,128,128")]
        [TestCase("128,128")]
        [TestCase("128")]
        [TestCase("")]
        public void JsonAscendancyClassOption_FlavourTextColour_White(string colourString)
        {
            var ascendancy = new JsonAscendancyClassOption
            {
                FlavourTextColourString = colourString
            };

            Assert.AreEqual(Color.White, ascendancy.FlavourTextColour);
        }
    }
}
