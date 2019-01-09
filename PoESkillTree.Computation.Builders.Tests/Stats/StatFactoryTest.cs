﻿using System;
using System.Linq;
using NUnit.Framework;
using PoESkillTree.Computation.Builders.Behaviors;
using PoESkillTree.Computation.Builders.Stats;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders.Damage;
using PoESkillTree.Computation.Common.Builders.Effects;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.GameModel;
using PoESkillTree.GameModel.Items;

namespace PoESkillTree.Computation.Builders.Tests.Stats
{
    [TestFixture]
    public class StatFactoryTest
    {
        [Test]
        public void ConvertToHasCorrectBehaviors()
        {
            var source = new Stat("source");
            var target = new Stat("target");
            var sut = CreateSut();

            var stat = sut.ConvertTo(source, new[] { target });
            var convertTo = stat.Single(s => s.Identity.Contains("ConvertTo"));
            var actual = convertTo.Behaviors;

            Assert.That(actual, Has.Exactly(4).Items);
            var value = AssertIsConversionTargetPathTotalBehavior(actual[0]);
            Assert.AreSame(convertTo, value.ConvertTo);
            AssertTransformedValueIs<ConversionTargeUncappedSubtotalValue>(actual[1]);
            AssertTransformedValueIs<ConversionSourcePathTotalValue>(actual[2]);
            AssertTransformedValueIs<ConvertToUncappedSubtotalValue>(actual[3]);
            Assert.AreSame(convertTo, actual[3].AffectedStats.Single());
        }

        [Test]
        public void SkillConversionHasCorrectBehaviors()
        {
            var source = new Stat("source");
            var target = new Stat("target");
            var sut = CreateSut();

            var stat = sut.ConvertTo(source, new[] { target });
            var skillConversion = stat.Single(s => s.Identity.Contains("SkillConversion"));
            var actual = skillConversion.Behaviors;

            Assert.That(actual, Has.Exactly(1).Items);
            AssertTransformedValueIs<SkillConversionUncappedSubtotalValue>(actual[0]);
        }

        [Test]
        public void GainAsHasCorrectBehaviors()
        {
            var source = new Stat("source");
            var target = new Stat("target");
            var sut = CreateSut();

            var stat = sut.GainAs(source, new[] { target });
            var gainAs = stat.Single(s => s.Identity.Contains("GainAs"));
            var actual = gainAs.Behaviors;

            Assert.That(actual, Has.Exactly(2).Items);
            AssertTransformedValueIs<ConversionTargetPathTotalValue>(actual[0]);
            AssertTransformedValueIs<ConversionTargeUncappedSubtotalValue>(actual[1]);
        }

        [Test]
        public void GainAndConversionBehaviorsAreEqual()
        {
            var source = new Stat("source");
            var target = new Stat("target");
            var sut = CreateSut();

            var convertTo = sut.ConvertTo(source, target);
            var gainAs = sut.GainAs(source, target);

            Assert.AreEqual(convertTo.Behaviors.Take(2), gainAs.Behaviors);
        }

        [Test]
        public void BehaviorsWithDifferentParametersAreNotEqual()
        {
            var sut = CreateSut();

            var first = sut.SkillConversion(new Stat("a")).Behaviors[0];
            var second = sut.SkillConversion(new Stat("b")).Behaviors[0];

            Assert.AreNotEqual(first, second);
        }

        private static ConversionTargetPathTotalValue AssertIsConversionTargetPathTotalBehavior(Behavior actual)
        {
            Assert.AreEqual("target", actual.AffectedStats.Single().Identity);
            Assert.AreEqual(NodeType.PathTotal, actual.AffectedNodeTypes.Single());
            Assert.AreEqual(BehaviorPathInteraction.Conversion, actual.AffectedPaths);
            var typedValue = AssertTransformedValueIs<ConversionTargetPathTotalValue>(actual);
            Assert.AreEqual("source.ConvertTo(target)", typedValue.ConvertTo.Identity);
            Assert.AreEqual("source.GainAs(target)", typedValue.GainAs.Identity);
            return typedValue;
        }

        [Test]
        public void FromIdentityReturnsCorrectStat()
        {
            var sut = CreateSut();

            var actual = sut.FromIdentity("test", Entity.Enemy, typeof(int));

            Assert.AreEqual("test", actual.Identity);
            Assert.AreEqual(Entity.Enemy, actual.Entity);
            Assert.AreEqual(typeof(int), actual.DataType);
        }

        [TestCase(typeof(string))]
        [TestCase(typeof(object))]
        [TestCase(typeof(float))]
        public void FromIdentityThrowsIfDataTypeIsInvalid(Type dataType)
        {
            var sut = CreateSut();

            Assert.Throws<ArgumentException>(() => sut.FromIdentity("", default, dataType));
        }
        
        [TestCase(typeof(double))]
        [TestCase(typeof(int))]
        [TestCase(typeof(uint))]
        [TestCase(typeof(bool))]
        [TestCase(typeof(Tags))]
        public void FromIdentityDoesNotThrowIfDataTypeIsValid(Type dataType)
        {
            var sut = CreateSut();

            Assert.DoesNotThrow(() => sut.FromIdentity("", default, dataType));
        }

        [Test]
        public void FromIdentityMaximumIsNullWithBoolDataType()
        {
            var sut = CreateSut();

            var actual = sut.FromIdentity("test", Entity.Character, typeof(bool)).Maximum;

            Assert.IsNull(actual);
        }

        [Test]
        public void RegenReturnsCorrectStat()
        {
            var sut = CreateSut();

            var actual = sut.Regen(default, Pool.Life);

            Assert.AreEqual("Life.Regen", actual.Identity);
            Assert.AreEqual(typeof(double), actual.DataType);
        }

        [Test]
        public void RegenHasCorrectBehaviors()
        {
            var sut = CreateSut();

            var actual = sut.Regen(default, Pool.Life).Behaviors;

            Assert.That(actual, Has.One.Items);
            AssertTransformedValueIs<RegenUncappedSubtotalValue>(actual[0]);
        }

        [Test]
        public void ConcretizeDamageHasCorrectBehaviorsIfStatIsHitDamage()
        {
            var inputStat = new StatFactory().Damage(default, DamageType.Fire);
            var spec = new SkillDamageSpecification(DamageSource.Spell);
            var sut = CreateSut();

            var actual = sut.ConcretizeDamage(inputStat, spec).Behaviors;

            Assert.That(actual, Has.One.Items);
            AssertTransformedValueIs<DamageEffectivenessBaseValue>(actual[0]);
        }

        [TestCase(DamageType.Fire)]
        [TestCase(DamageType.Cold)]
        public void ConcretizeDamageHasCorrectBehaviorsIfStatIsAilmentDamage(DamageType damageType)
        {
            var inputStat = new StatFactory().Damage(default, damageType);
            var spec = new AilmentDamageSpecification(DamageSource.Spell, Ailment.Bleed);
            var sut = CreateSut();

            var actual = sut.ConcretizeDamage(inputStat, spec).Behaviors;

            Assert.That(actual, Has.Exactly(3).Items);
            AssertTransformedValueIs<AilmentDamageUncappedSubtotalValue>(actual[0]);
            AssertTransformedValueIs<AilmentDamageBaseValue>(actual[1]);
            AssertTransformedValueIs<AilmentDamageIncreaseMoreValue>(actual[2]);
        }

        [Test]
        public void ConcretizeDamageHasNoBehaviorsIfSpecificationIsSkillDot()
        {
            var inputStat = new StatFactory().Damage(default, DamageType.Fire);
            var spec = new SkillDamageSpecification(DamageSource.OverTime);
            var sut = CreateSut();

            var actual = sut.ConcretizeDamage(inputStat, spec).Behaviors;

            CollectionAssert.IsEmpty(actual);
        }

        [Test]
        public void ConcretizeDamageHasNoBehaviorsIfStatIsNotDamage()
        {
            var inputStat = new Stat("CritChance");
            var spec = new AilmentDamageSpecification(DamageSource.Spell, Ailment.Bleed);
            var sut = CreateSut();

            var actual = sut.ConcretizeDamage(inputStat, spec).Behaviors;

            CollectionAssert.IsEmpty(actual);
        }

        private static T AssertTransformedValueIs<T>(Behavior actual) where T : IValue
        {
            var value = actual.Transformation.Transform(null);
            Assert.IsInstanceOf<T>(value);
            return (T) value;
        }

        private static StatFactory CreateSut() => new StatFactory();
    }
}