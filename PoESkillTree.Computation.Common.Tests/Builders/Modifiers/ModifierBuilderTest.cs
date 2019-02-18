﻿using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Forms;
using PoESkillTree.Computation.Common.Builders.Modifiers;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.Computation.Common.Builders.Values;

namespace PoESkillTree.Computation.Common.Tests.Builders.Modifiers
{
    [TestFixture]
    public class ModifierBuilderTest
    {
        [Test]
        public void IsIModifierBuilder()
        {
            var sut = ModifierBuilder.Empty;

            Assert.IsInstanceOf<IModifierBuilder>(sut);
        }

        [Test]
        public void EntriesIsEmpty()
        {
            var sut = ModifierBuilder.Empty;

            CollectionAssert.IsEmpty(sut.Entries);
        }

        [Test]
        public void WithFormReturnsModifierBuilder()
        {
            var sut = ModifierBuilder.Empty;
            var form = Mock.Of<IFormBuilder>();

            var actual = sut.WithForm(form);

            Assert.IsInstanceOf<ModifierBuilder>(actual);
        }

        [Test]
        public void WithFormsReturnsModifierBuilder()
        {
            var sut = ModifierBuilder.Empty;

            var actual = sut.WithForms(new IFormBuilder[0]);

            Assert.IsInstanceOf<ModifierBuilder>(actual);
        }

        [Test]
        public void WithStatReturnsModifierBuilder()
        {
            var sut = ModifierBuilder.Empty;
            var stat = Mock.Of<IStatBuilder>();

            var actual = sut.WithStat(stat);

            Assert.IsInstanceOf<ModifierBuilder>(actual);
        }

        [Test]
        public void WithStatsReturnsModifierBuilder()
        {
            var sut = ModifierBuilder.Empty;

            var actual = sut.WithStats(new IStatBuilder[0]);

            Assert.IsInstanceOf<ModifierBuilder>(actual);
        }

        [Test]
        public void WithStatConverterReturnsModifierBuilder()
        {
            var sut = ModifierBuilder.Empty;

            var actual = sut.WithStatConverter(s => s);

            Assert.IsInstanceOf<ModifierBuilder>(actual);
        }

        [Test]
        public void WithValueReturnsModifierBuilder()
        {
            var sut = ModifierBuilder.Empty;
            var value = Mock.Of<IValueBuilder>();

            var actual = sut.WithValue(value);

            Assert.IsInstanceOf<ModifierBuilder>(actual);
        }

        [Test]
        public void WithValuesReturnsModifierBuilder()
        {
            var sut = ModifierBuilder.Empty;

            var actual = sut.WithValues(new IValueBuilder[0]);

            Assert.IsInstanceOf<ModifierBuilder>(actual);
        }

        [Test]
        public void WithValueConverterReturnsModifierBuilder()
        {
            var sut = ModifierBuilder.Empty;

            var actual = sut.WithValueConverter(v => v);

            Assert.IsInstanceOf<ModifierBuilder>(actual);
        }

        [Test]
        public void WithConditionReturnsModifierBuilder()
        {
            var sut = ModifierBuilder.Empty;
            var condition = Mock.Of<IConditionBuilder>();

            var actual = sut.WithCondition(condition);

            Assert.IsInstanceOf<ModifierBuilder>(actual);
        }

        [Test]
        public void WithConditionsReturnsModifierBuilder()
        {
            var sut = ModifierBuilder.Empty;

            var actual = sut.WithConditions(new IConditionBuilder[0]);

            Assert.IsInstanceOf<ModifierBuilder>(actual);
        }

        [Test]
        public void WithFormAddsCorrectEntryWhenEmpty()
        {
            var sut = ModifierBuilder.Empty;
            var form = Mock.Of<IFormBuilder>();

            sut = (ModifierBuilder) sut.WithForm(form);

            Assert.That(sut.Entries,
                Has.Exactly(1).EqualTo(Entry.WithForm(form)));
        }

        [Test]
        public void WithFormCalledTwiceThrows()
        {
            var sut = ModifierBuilder.Empty;
            var form = Mock.Of<IFormBuilder>();
            sut = (ModifierBuilder) sut.WithForm(form);

            Assert.Throws<InvalidOperationException>(() => sut.WithForm(form));
        }

        [Test]
        public void WithFormModifiesExistingEntriesCorrectly()
        {
            var sut = ModifierBuilder.Empty;
            var stats = Many<IStatBuilder>();
            sut = (ModifierBuilder) sut.WithStats(stats);
            var form = Mock.Of<IFormBuilder>();
            var expected =
                stats.Select(s => Entry.WithStat(s).WithForm(form)).ToList();

            sut = (ModifierBuilder) sut.WithForm(form);

            CollectionAssert.AreEqual(expected, sut.Entries);
        }

        [Test]
        public void WithFormsAddsCorrectEntriesWhenEmpty()
        {
            var sut = ModifierBuilder.Empty;
            var forms = Many<IFormBuilder>();
            var expected = forms.Select(f => Entry.WithForm(f)).ToList();

            sut = (ModifierBuilder) sut.WithForms(forms);

            CollectionAssert.AreEqual(expected, sut.Entries);
        }

        [Test]
        public void WithFormsCalledTwiceThrows()
        {
            var sut = ModifierBuilder.Empty;
            var forms = Many<IFormBuilder>();
            sut = (ModifierBuilder) sut.WithForms(forms);

            Assert.Throws<InvalidOperationException>(() => sut.WithForms(forms));
        }

        [Test]
        public void WithFormsModifiesExistingSingleEntryCorrectly()
        {
            var sut = ModifierBuilder.Empty;
            var stat = Mock.Of<IStatBuilder>();
            sut = (ModifierBuilder) sut.WithStat(stat);
            var forms = Many<IFormBuilder>();
            var expected = forms.Select(f => Entry.WithStat(stat).WithForm(f)).ToList();

            sut = (ModifierBuilder) sut.WithForms(forms);

            CollectionAssert.AreEqual(expected, sut.Entries);
        }

        [Test]
        public void WithFormsModifiesExistingMultipleEntriesCorrectly()
        {
            var sut = ModifierBuilder.Empty;
            var stats = Many<IStatBuilder>();
            sut = (ModifierBuilder) sut.WithStats(stats);
            var forms = Many<IFormBuilder>();
            var expected = forms.Zip(stats, (f, s) => Entry.WithForm(f).WithStat(s)).ToList();

            sut = (ModifierBuilder) sut.WithForms(forms);

            CollectionAssert.AreEqual(expected, sut.Entries);
        }

        [TestCase(2)]
        [TestCase(4)]
        public void WithFormsThrowsIfDifferentAmountOfExistingEntries(int existingCount)
        {
            var sut = ModifierBuilder.Empty;
            var stats = Many<IStatBuilder>(existingCount);
            sut = (ModifierBuilder) sut.WithStats(stats);
            var forms = Many<IFormBuilder>();

            Assert.Throws<ArgumentException>(() => sut.WithForms(forms));
        }

        [Test]
        public void WithFormsStatsValuesAndConditionsCreatesCorrectEntries()
        {
            var sut = ModifierBuilder.Empty;
            var forms = Many<IFormBuilder>();
            var stats = Many<IStatBuilder>();
            var values = Many<IValueBuilder>();
            var conditions = Many<IConditionBuilder>();
            var expected = forms.Select(f => Entry.WithForm(f))
                .Zip(stats, (e, s) => e.WithStat(s))
                .Zip(values, (e, v) => e.WithValue(v))
                .Zip(conditions, (e, c) => e.WithCondition(c))
                .ToList();

            sut = (ModifierBuilder) sut
                .WithForms(forms)
                .WithStats(stats)
                .WithValues(values)
                .WithConditions(conditions);

            CollectionAssert.AreEqual(expected, sut.Entries);
        }

        [Test]
        public void WithStatFormsValueAndConditionCreatesCorrectEntries()
        {
            var sut = ModifierBuilder.Empty;
            var forms = Many<IFormBuilder>();
            var stat = Mock.Of<IStatBuilder>();
            var value = Mock.Of<IValueBuilder>();
            var condition = Mock.Of<IConditionBuilder>();
            var expexted = forms
                .Select(f =>
                    Entry.WithForm(f).WithStat(stat).WithValue(value).WithCondition(condition))
                .ToList();

            sut = (ModifierBuilder) sut
                .WithStat(stat)
                .WithForms(forms)
                .WithValue(value)
                .WithCondition(condition);

            CollectionAssert.AreEqual(expexted, sut.Entries);
        }

        [Test]
        public void WithStatConverterSetsStatConverter()
        {
            var sut = ModifierBuilder.Empty;
            StatConverter statConverter = s => null;

            sut = (ModifierBuilder) sut.WithStatConverter(statConverter);

            Assert.AreSame(statConverter, sut.StatConverter);
        }

        [Test]
        public void WithValueConverterSetsValueConverter()
        {
            var sut = ModifierBuilder.Empty;
            ValueConverter valueConverter = v => null;

            sut = (ModifierBuilder) sut.WithValueConverter(valueConverter);

            Assert.AreSame(valueConverter, sut.ValueConverter);
        }

        [Test]
        public void InitialStatConverterIsIdentity()
        {
            var sut = ModifierBuilder.Empty;
            var stat = Mock.Of<IStatBuilder>();

            var actual = sut.StatConverter(stat);

            Assert.AreEqual(stat, actual);
        }

        [Test]
        public void InitialValueConverterIsIdentity()
        {
            var sut = ModifierBuilder.Empty;
            var value = Mock.Of<IValueBuilder>();

            var actual = sut.ValueConverter(value);

            Assert.AreEqual(value, actual);
        }

        [Test]
        public void IsIIntermediateModifier()
        {
            var sut = ModifierBuilder.Empty;

            Assert.IsInstanceOf<IIntermediateModifier>(sut);
        }

        [Test]
        public void CreateReturnsSelf()
        {
            var sut = ModifierBuilder.Empty;

            var actual = sut.Build();

            Assert.AreSame(sut, actual);
        }

        private static IntermediateModifierEntry Entry => new IntermediateModifierEntry();

        private static IReadOnlyList<T> Many<T>(int count = 3) where T : class =>
            Enumerable.Range(0, count).Select(_ => Mock.Of<T>()).ToList();
    }
}