﻿using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PoESkillTree.Computation.Builders.Stats;
using PoESkillTree.Computation.Common.Builders.Skills;
using PoESkillTree.GameModel.Skills;

namespace PoESkillTree.Computation.Builders.Skills
{
    [TestFixture]
    public class SkillBuilderCollectionTest
    {
        [Test]
        public void CombinedInstancesBuildsToCorrectResult()
        {
            var keywords = new[] { Keyword.Aura, Keyword.Melee };
            var sut = CreateSut(keywords.Select(k => new KeywordBuilder(k)));

            var stat = sut.CombinedInstances.BuildToSingleStat();

            Assert.AreEqual("Skills[Aura, Melee].Instances", stat.Identity);
        }

        [Test]
        public void ResolveCombinedInstancesBuildsToCorrectResult()
        {
            var keywords = new[] { Keyword.Projectile };
            var unresolved = new[] { Mock.Of<IKeywordBuilder>(b => b.Resolve(null).Build(default) == keywords[0]) };
            var sut = CreateSut(unresolved);

            var stat = sut.Resolve(null).CombinedInstances.BuildToSingleStat();

            Assert.AreEqual("Skills[Projectile].Instances", stat.Identity);
        }

        [Test]
        public void CombinedInstancesResolveBuildsToCorrectResult()
        {
            var keywords = new[] { Keyword.Projectile };
            var unresolved = new[] { Mock.Of<IKeywordBuilder>(b => b.Resolve(null).Build(default) == keywords[0]) };
            var sut = CreateSut(unresolved);

            var stat = sut.CombinedInstances.Resolve(null).BuildToSingleStat();

            Assert.AreEqual("Skills[Projectile].Instances", stat.Identity); 
        }

        [Test]
        public void CastBuildsToCorrectResult()
        {
            var keywords = new[] { Keyword.Aura, Keyword.Melee };
            var sut = CreateSut(keywords.Select(k => new KeywordBuilder(k)));

            var actual = sut.Cast.Build(default);

            Assert.AreEqual("Skills[Aura, Melee].Cast", actual);
        }

        private static SkillBuilderCollection CreateSut(IEnumerable<IKeywordBuilder> keywords) =>
            new SkillBuilderCollection(new StatFactory(), keywords, Enumerable.Empty<SkillDefinition>());
    }
}