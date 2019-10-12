using NUnit.Framework;
using PoESkillTree.Engine.Computation.Common.Builders;

namespace PoESkillTree.Engine.Computation.Data.Collections
{
    [TestFixture]
    public class StatManipulatorMatcherCollectionTest
    {
        private const string Regex = "regex";

#pragma warning disable 8618 // Initialized in SetUp
        private StatManipulatorMatcherCollection _sut;
#pragma warning restore

        [SetUp]
        public void SetUp()
        {
            _sut = new StatManipulatorMatcherCollection(new ModifierBuilderStub());
        }

        [Test]
        public void IsEmpty()
        {
            Assert.IsEmpty(_sut);
        }

        [Test]
        public void AddWithoutSubstitution()
        {
            StatConverter manipulator = s => null;

            _sut.Add(Regex, manipulator);

            var builder = _sut.AssertSingle(Regex);
            Assert.AreSame(manipulator, builder.StatConverter);
        }

        [Test]
        public void AddWithSubstitution()
        {
            StatConverter manipulator = s => null;

            _sut.Add(Regex, manipulator, "substitution");

            var builder = _sut.AssertSingle(Regex, "substitution");
            Assert.AreSame(manipulator, builder.StatConverter);
        }
    }
}