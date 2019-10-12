using System.Linq;
using Moq;
using NUnit.Framework;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;

namespace PoESkillTree.Engine.Computation.Data.Collections
{
    [TestFixture]
    public class ConditionMatcherCollectionTest
    {
        private const string Regex = "regex";

#pragma warning disable 8618 // Initialized in SetUp
        private ConditionMatcherCollection _sut;
#pragma warning restore

        [SetUp]
        public void SetUp()
        {
            _sut = new ConditionMatcherCollection(new ModifierBuilderStub());
        }

        [Test]
        public void IsEmpty()
        {
            Assert.IsEmpty(_sut);
        }

        [Test]
        public void Add()
        {
            var condition = Mock.Of<IConditionBuilder>();

            _sut.Add(Regex, condition);

            var builder = _sut.AssertSingle(Regex);
            Assert.That(builder.Conditions, Has.Exactly(1).SameAs(condition));
        }

        [Test]
        public void AddManyAddsToCount()
        {
            var condition = Mock.Of<IConditionBuilder>();

            _sut.Add(Regex, condition);
            _sut.Add(Regex, condition);
            _sut.Add(Regex, (condition, condition));

            Assert.AreEqual(3, _sut.Count());
        }
    }
}