using System.Linq;
using Moq;
using NUnit.Framework;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Data.Collections
{
    [TestFixture]
    public class ValueConversionMatcherCollectionTest
    {
        private const string Regex = "regex";

        [Test]
        public void IsEmpty()
        {
            var sut = new ValueConversionMatcherCollection(new ModifierBuilderStub());

            Assert.IsEmpty(sut);
        }

        [Test]
        public void Add()
        {
            var inputValue = new ValueBuilder(Mock.Of<IValueBuilder>());
            var expectedValue = new ValueBuilder(Mock.Of<IValueBuilder>());
            var sut = new ValueConversionMatcherCollection(new ModifierBuilderStub())
            {
                {Regex, _ => expectedValue}
            };

            var builder = sut.AssertSingle(Regex);
            var actualValue = builder.ValueConverter!(inputValue);
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void AddManyAddsToCount()
        {
            var sut = new ValueConversionMatcherCollection(new ModifierBuilderStub())
            {
                {Regex, Funcs.Identity}, {Regex, Funcs.Identity}, {Regex, Funcs.Identity}
            };

            Assert.AreEqual(3, sut.Count());
        }
    }
}