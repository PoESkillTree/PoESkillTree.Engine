﻿using System;
using Moq;
using NUnit.Framework;
using PoESkillTree.Common.Utils;
using PoESkillTree.Computation.Builders.Stats;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Entities;
using PoESkillTree.Computation.Common.Builders.Values;
using PoESkillTree.Computation.Common.Parsing;
using PoESkillTree.Computation.Common.Tests;

namespace PoESkillTree.Computation.Builders.Tests.Stats
{
    // ConversionStatBuilder is also tested from StatBuilderTest.
    [TestFixture]
    public class ConversionStatBuilderTest
    {
        [Test]
        public void BuildThrowsIfSourceAndTargetEntityDoNotMatch()
        {
            var source = new LeafCoreStatBuilder("s", new EntityBuilder(Entity.Character));
            var target = new LeafCoreStatBuilder("t", new EntityBuilder(Entity.Enemy));
            var sut = new ConversionStatBuilder(source, target);

            Assert.Throws<ParseException>(() => sut.Build(ModifierSource, default));
        }

        [Test]
        public void BuildGroupsStatsByEntity()
        {
            var expected = new[]
                { new Stat("s.ConvertTo(t)", Entity.Character), new Stat("s.ConvertTo(t)", Entity.Enemy) };
            var source = new LeafCoreStatBuilder("s", new EntityBuilder(Entity.Character, Entity.Enemy));
            var target = new LeafCoreStatBuilder("t", new EntityBuilder(Entity.Enemy, Entity.Character));
            var sut = new ConversionStatBuilder(source, target);

            var actual = sut.Build(ModifierSource, default).Stats;

            Assert.AreEqual(6, actual.Count);
            CollectionAssert.IsSubsetOf(expected, actual);
        }

        [Test]
        public void BuildChainsValueConverters()
        {
            var valueBuilders = Helper.MockMany<IValueBuilder>();
            var source = MockStatBuilder(valueConverter: _ => valueBuilders[1]);
            var target = MockStatBuilder(valueConverter: _ => valueBuilders[2]);
            var sut = new ConversionStatBuilder(source, target);

            var valueConverter = sut.Build(ModifierSource, default).ValueConverter;
            var actual = valueConverter(valueBuilders[0]);

            Assert.AreEqual(valueBuilders[2], actual);
        }

        [Test]
        public void BuildThrowsIfSourceModifierSourceIsNotOriginal()
        {
            var source = MockStatBuilder(new ModifierSource.Local.Given());
            var target = MockStatBuilder();
            var sut = new ConversionStatBuilder(source, target);

            Assert.Throws<ParseException>(() => sut.Build(ModifierSource, default));
        }

        [Test]
        public void BuildThrowsIfTargetModifierSourceIsNotOriginal()
        {
            var source = MockStatBuilder();
            var target = MockStatBuilder(new ModifierSource.Local.Given());
            var sut = new ConversionStatBuilder(source, target);

            Assert.Throws<ParseException>(() => sut.Build(ModifierSource, default));
        }

        [Test]
        public void ResolveResolvesSourceAndTarget()
        {
            var source = new Mock<ICoreStatBuilder>();
            var target = new Mock<ICoreStatBuilder>();
            var sut = new ConversionStatBuilder(source.Object, target.Object);

            sut.Resolve(null);

            source.Verify(b => b.Resolve(null));
            target.Verify(b => b.Resolve(null));
        }

        [Test]
        public void WithEntityAppliesToSource()
        {
            var entityBuilder = Mock.Of<IEntityBuilder>();
            var source = new Mock<ICoreStatBuilder>();
            var sut = new ConversionStatBuilder(source.Object, MockStatBuilder());

            sut.WithEntity(entityBuilder);

            source.Verify(b => b.WithEntity(entityBuilder));
        }

        [Test]
        public void WithStatConverterAppliesToSource()
        {
            Func<IStat, IStat> statConverter = Funcs.Identity;
            var source = new Mock<ICoreStatBuilder>();
            var sut = new ConversionStatBuilder(source.Object, MockStatBuilder());

            sut.WithStatConverter(statConverter);

            source.Verify(b => b.WithStatConverter(statConverter));
        }

        [Test]
        public void BuildValueThrows()
        {
            var sut = new ConversionStatBuilder(MockStatBuilder(), MockStatBuilder());

            Assert.Throws<ParseException>(() => sut.BuildValue(default));
        }

        private static ICoreStatBuilder MockStatBuilder(
            ModifierSource modifierSource = null, ValueConverter valueConverter = null)
        {
            var result = new StatBuilderResult(new IStat[0], modifierSource ?? ModifierSource,
                valueConverter ?? Funcs.Identity);
            return Mock.Of<ICoreStatBuilder>(b => b.Build(ModifierSource, default) == result);
        }

        private static readonly ModifierSource ModifierSource = new ModifierSource.Global();
    }
}