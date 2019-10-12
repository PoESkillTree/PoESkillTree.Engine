using NUnit.Framework;

namespace PoESkillTree.Engine.Computation.Builders.Resolving
{
    [TestFixture]
    public class UnresolvedBuilderTest
    {
        [Test]
        public void BuildThrowsParseException()
        {
            var sut = CreateSut();

            Assert.Throws<UnresolvedException>(() => sut.Build(default));
        }

        [Test]
        public void ResolveUsesInjectedFunction()
        {
            var expected = "5";
            var sut = CreateSut(expected);

            var actual = sut.Resolve(BuildersHelper.MockResolveContext());

            Assert.AreEqual(expected, actual);
        }

        private static UnresolvedBuilder<string, string> CreateSut(string resolved = "0") => 
            new UnresolvedBuilder<string, string>("", _ => resolved);
    }
}