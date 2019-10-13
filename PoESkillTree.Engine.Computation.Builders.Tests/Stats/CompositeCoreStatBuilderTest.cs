using System.Linq;
using Moq;
using MoreLinq;
using NUnit.Framework;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Parsing;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Builders.Stats
{
    // CompositeCoreStatBuilder is also tested from StatBuilderTest.
    [TestFixture]
    public class CompositeCoreStatBuilderTest
    {
        [Test]
        public void BuildWithMultipleResultEntryReturnsCorrectResults()
        {
            var multiResults = new[]
            {
                new StatBuilderResult(new IStat[0], null!, Funcs.Identity),
                new StatBuilderResult(new IStat[0], null!, Funcs.Identity),
            };
            var singleResults = new[]
            {
                new StatBuilderResult(new IStat[0], null!, Funcs.Identity),
            };
            var multiCoreStatBuilder = Mock.Of<ICoreStatBuilder>(b => b.Build(default) == multiResults);
            var singleCoreStatBuilder =
                Mock.Of<ICoreStatBuilder>(b => b.Build(default) == singleResults);
            var sut = new CompositeCoreStatBuilder(multiCoreStatBuilder, singleCoreStatBuilder);

            var results = sut.Build(default).ToList();

            Assert.That(results, Has.Exactly(2).Items);
        }

        [Test]
        public void BuildThrowsIfBothItemsBuildToMultipleResults()
        {            
            var itemResults = new[]
            {
                new StatBuilderResult(new IStat[0], null!, Funcs.Identity),
                new StatBuilderResult(new IStat[0], null!, Funcs.Identity),
            };
            var leftCore = Mock.Of<ICoreStatBuilder>(b => b.Build(default) == itemResults);
            var rightCore = Mock.Of<ICoreStatBuilder>(b => b.Build(default) == itemResults);
            var sut = new CompositeCoreStatBuilder(leftCore, rightCore);

            Assert.Throws<ParseException>(() => sut.Build(default).Consume());
        }
    }
}