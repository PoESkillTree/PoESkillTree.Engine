using System.Linq;
using NUnit.Framework;

namespace PoESkillTree.Engine.Computation.Data.Collections
{
    [TestFixture]
    public class StatReplacerCollectionTest
    {
        [Test]
        public void IsEmpty()
        {
            var sut = new StatReplacerCollection();

            Assert.AreEqual(0, sut.Count());
        }

        [Test]
        public void AddAddsCorrectData()
        {
            var sut = new StatReplacerCollection {{"originalStat", "r1", "r2", "r3"}};

            var data = sut.Single();

            Assert.AreEqual("originalStat", data.OriginalStatRegex);
            CollectionAssert.AreEqual(new[] { "r1", "r2", "r3" }, data.Replacements);
        }

        [Test]
        public void AddManyAddsToCount()
        {
            var sut = new StatReplacerCollection {"1", {"2", "r1"}, {"3", "r1", "r2"}};

            Assert.AreEqual(3, sut.Count());
        }
    }
}