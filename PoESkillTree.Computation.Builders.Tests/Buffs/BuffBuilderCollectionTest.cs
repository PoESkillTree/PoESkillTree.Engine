﻿using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PoESkillTree.Computation.Builders.Buffs;
using PoESkillTree.Computation.Builders.Entities;
using PoESkillTree.Computation.Builders.Skills;
using PoESkillTree.Computation.Builders.Stats;
using PoESkillTree.Computation.Builders.Tests.Stats;
using PoESkillTree.Computation.Builders.Values;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders.Buffs;
using PoESkillTree.Computation.Common.Builders.Skills;
using PoESkillTree.GameModel;
using PoESkillTree.GameModel.Skills;

namespace PoESkillTree.Computation.Builders.Tests.Buffs
{
    [TestFixture]
    public class BuffBuilderCollectionTest
    {
        [Test]
        public void EffectBuildsToCorrectResults()
        {
            var sut = CreateSut(3);

            var stats = sut.Effect.BuildToStats();

            Assert.That(stats, Has.Exactly(3).Items);
            Assert.AreEqual($"b0.EffectOn({default(Entity)})", stats[0].Identity);
            Assert.AreEqual($"b1.EffectOn({default(Entity)})", stats[1].Identity);
            Assert.AreEqual($"b2.EffectOn({default(Entity)})", stats[2].Identity);
        }

        [Test]
        public void AddStatBuildsToCorrectResult()
        {
            var addedStat = StatBuilderUtils.FromIdentity(StatFactory, "s", null);
            var sut = CreateSut(3);

            var stats = sut.AddStat(addedStat).BuildToStats();

            Assert.That(stats, Has.Exactly(3).Items);
            Assert.AreEqual("s", stats[0].Identity);
        }

        [Test]
        public void ResolveEffectBuildsToCorrectResult()
        {
            var buff = new BuffBuilder(StatFactory, CoreBuilder.Create("b"));
            var unresolvedBuff = Mock.Of<IBuffBuilder>(b => b.Resolve(null) == buff);
            var sut = CreateSut(unresolvedBuff);

            var resolved = (IBuffBuilderCollection) sut.Resolve(null);
            var stats = resolved.Effect.BuildToSingleResult().Stats;

            Assert.AreEqual($"b.EffectOn({default(Entity)})", stats[0].Identity);
        }

        [Test]
        public void WithBuildsToCorrectResult()
        {
            var keyword = KeywordBuilder(1);
            var sut = CreateSut(3);

            var stats = sut.With(keyword).Effect.BuildToStats();

            Assert.That(stats, Has.Exactly(2).Items);
            Assert.AreEqual($"b0.EffectOn({default(Entity)})", stats[0].Identity);
            Assert.AreEqual($"b1.EffectOn({default(Entity)})", stats[1].Identity);
        }

        [Test]
        public void WithNotContainedKeywordBuildsToEmptyResults()
        {
            var keyword = KeywordBuilder(2);
            var sut = CreateSut(1);

            var results = sut.With(keyword).Effect.Build(default);

            Assert.IsEmpty(results);
        }

        [Test]
        public void WithoutBuildsToCorrectResult()
        {
            var keyword = KeywordBuilder(1);
            var sut = CreateSut(3);

            var stats = sut.Without(keyword).Effect.BuildToSingleResult().Stats;

            Assert.That(stats, Has.Exactly(1).Items);
            Assert.AreEqual($"b2.EffectOn({default(Entity)})", stats[0].Identity);
        }

        [Test]
        public void WithResolveEffectBuildsToCorrectResult()
        {
            var keyword = KeywordBuilder(1);
            var unresolved = Mock.Of<IKeywordBuilder>(b => b.Resolve(null) == keyword);
            var sut = CreateSut(3);

            var resolved = (IBuffBuilderCollection) sut.With(unresolved).Resolve(null);
            var stats = resolved.Effect.BuildToStats();

            Assert.That(stats, Has.Exactly(2).Items);
        }

        [Test]
        public void WithoutResolveEffectBuildsToCorrectResult()
        {
            var keyword = KeywordBuilder(1);
            var unresolved = Mock.Of<IKeywordBuilder>(b => b.Resolve(null) == keyword);
            var sut = CreateSut(3);

            var resolved = (IBuffBuilderCollection) sut.Without(unresolved).Resolve(null);
            var stats = resolved.Effect.BuildToSingleResult().Stats;

            Assert.That(stats, Has.Exactly(1).Items);
        }

        [Test]
        public void WithEffectResolveBuildsToCorrectResult()
        {
            var keyword = KeywordBuilder(1);
            var unresolved = Mock.Of<IKeywordBuilder>(b => b.Resolve(null) == keyword);
            var sut = CreateSut(3);

            var resolved = sut.With(unresolved).Effect.Resolve(null);
            var stats = resolved.BuildToStats();

            Assert.That(stats, Has.Exactly(2).Items);
        }

        [Test]
        public void EffectForEntityBuildsToCorrectResult()
        {
            var entityBuilder = new EntityBuilder(Entity.Enemy);
            var sut = CreateSut(1);

            var stats = sut.Effect.For(entityBuilder).BuildToSingleResult().Stats;

            Assert.AreEqual(Entity.Enemy, stats[0].Entity);
        }

        [TestCase(0)]
        [TestCase(0, 1, 2)]
        public void CountBuildsToCorrectValue(params int[] activeBuffs)
        {
            var expected = (activeBuffs.Contains(0) ? 1 : 0) + (activeBuffs.Contains(1) ? 1 : 0);
            var keyword = KeywordBuilder(1);
            var contextMock = new Mock<IValueCalculationContext>();
            foreach (var activeBuff in activeBuffs)
            {
                var activeStat = StatFactory.BuffIsActive(default, $"b{activeBuff}");
                contextMock.Setup(c => c.GetValue(activeStat, NodeType.Total, PathDefinition.MainPath))
                    .Returns((NodeValue?) true);
                var sourceStat = StatFactory.BuffSourceIs(default, default, $"b{activeBuff}");
                contextMock.Setup(c => c.GetValue(sourceStat, NodeType.Total, PathDefinition.MainPath))
                    .Returns((NodeValue?) true);
            }
            var sut = CreateSut(3);

            var value = sut.With(keyword).Count().Build();
            var actual = value.Calculate(contextMock.Object);

            Assert.AreEqual(new NodeValue(expected), actual);
        }

        [TestCase(0)]
        [TestCase(2)]
        public void AnyBuildsToCorrectResult(params int[] activeBuffs)
        {
            var expected = activeBuffs.Contains(0) || activeBuffs.Contains(1);
            var keyword = KeywordBuilder(1);
            var contextMock = new Mock<IValueCalculationContext>();
            foreach (var activeBuff in activeBuffs)
            {
                var activeStat = StatFactory.BuffIsActive(default, $"b{activeBuff}");
                contextMock.Setup(c => c.GetValue(activeStat, NodeType.Total, PathDefinition.MainPath))
                    .Returns((NodeValue?) true);
                var sourceStat = StatFactory.BuffSourceIs(default, default, $"b{activeBuff}");
                contextMock.Setup(c => c.GetValue(sourceStat, NodeType.Total, PathDefinition.MainPath))
                    .Returns((NodeValue?) true);
            }
            var sut = CreateSut(3);

            var result = sut.With(keyword).Any().Build();
            var actual = result.Value.Calculate(contextMock.Object);

            Assert.IsFalse(result.HasStatConverter);
            Assert.AreEqual((NodeValue?) expected, actual);
        }

        [Test]
        public void AnyResolveBuildsToCorrectResult()
        {
            var keyword = KeywordBuilder(2);
            var unresolved = Mock.Of<IKeywordBuilder>(b => b.Resolve(null) == keyword);
            var activeStat = StatFactory.BuffIsActive(default, "b0");
            var sourceStat = StatFactory.BuffSourceIs(default, default, "b0");
            var context = Mock.Of<IValueCalculationContext>(c =>
                c.GetValue(activeStat, NodeType.Total, PathDefinition.MainPath) == (NodeValue?) true &&
                c.GetValue(sourceStat, NodeType.Total, PathDefinition.MainPath) == (NodeValue?) true);
            var sut = CreateSut(1);

            var any = sut.With(unresolved).Any();
            var result = any.Resolve(null).Build();
            var actual = result.Value.Calculate(context);

            Assert.IsFalse(actual.IsTrue());
        }

        [Test]
        public void ApplyToEntityBuildsToCorrectResult()
        {
            var target = new EntityBuilder(Entity.Minion);
            var active = Enumerable.Range(0, 3).Select(i => new Stat($"b{i}.Active")).ToList();
            var source = Enumerable.Range(0, 3)
                .Select(i => StatFactory.BuffSourceIs(Entity.Character, default, $"b{i}")).ToList();
            var context = Mock.Of<IValueCalculationContext>(c =>
                c.GetValue(active[0], NodeType.Total, PathDefinition.MainPath) == (NodeValue?) true &&
                c.GetValue(active[1], NodeType.Total, PathDefinition.MainPath) == (NodeValue?) true &&
                c.GetValue(source[1], NodeType.Total, PathDefinition.MainPath) == (NodeValue?) true &&
                c.GetValue(source[2], NodeType.Total, PathDefinition.MainPath) == (NodeValue?) true);
            var inputValue = new ValueBuilderImpl(1);
            var sut = CreateSut(3);

            var results = sut.ApplyToEntity(target).Build(default).ToList();
            var values = results.Select(r => r.ValueConverter(inputValue).Build().Calculate(context)).ToList();

            Assert.That(results, Has.Exactly(3).Items);
            Assert.That(results[0].Stats, Has.Exactly(3).Items);
            Assert.AreEqual("Minion.b0.Active", results[0].Stats[0].ToString());
            Assert.AreEqual("Minion.b0.BuffSourceIs(Character)", results[0].Stats[2].ToString());
            Assert.IsFalse(values[0].IsTrue());
            Assert.IsTrue(values[1].IsTrue());
            Assert.IsFalse(values[2].IsTrue());
        }

        private static BuffBuilderCollection CreateSut(int buffCount)
        {
            var buffs =
                from i in Enumerable.Range(0, buffCount)
                let buff = new BuffBuilder(StatFactory, CoreBuilder.Create($"b{i}"))
                let keywords = new[] { Keyword(i), Keyword(i + 1) }
                select new BuffBuilderWithKeywords(buff, keywords);
            return CreateSut(buffs);
        }

        private static BuffBuilderCollection CreateSut(params IBuffBuilder[] buffs) =>
            CreateSut(buffs.Select(b => new BuffBuilderWithKeywords(b)));

        private static BuffBuilderCollection CreateSut(IEnumerable<BuffBuilderWithKeywords> buffs) =>
            new BuffBuilderCollection(StatFactory, buffs.ToList(), new ModifierSourceEntityBuilder(),
                new ModifierSourceEntityBuilder());

        private static readonly StatFactory StatFactory = new StatFactory();

        private static IKeywordBuilder KeywordBuilder(int ordinal) => new KeywordBuilder(Keyword(ordinal));

        private static Keyword Keyword(int ordinal) => (Keyword) ordinal;
    }
}