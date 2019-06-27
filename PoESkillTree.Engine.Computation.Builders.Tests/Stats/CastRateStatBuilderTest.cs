using Moq;
using MoreLinq;
using NUnit.Framework;
using PoESkillTree.Engine.Computation.Builders.Skills;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Builders.Stats
{
    [TestFixture]
    public class CastRateStatBuilderTest
    {
        [Test]
        public void WithKeywordCallsStatFactoryCorrectly()
        {
            var statFactoryMock = new Mock<IStatFactory>();
            var sut = new CastRateStatBuilder(statFactoryMock.Object);

            var withKeyword = sut.With(new KeywordBuilder(Keyword.Projectile));
            withKeyword.Build(default).Consume();

            statFactoryMock.Verify(f => f.MainSkillPartCastRateHasKeyword(default, Keyword.Projectile));
        }
    }
}