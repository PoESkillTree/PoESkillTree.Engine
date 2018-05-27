﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;
using NUnit.Framework;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Damage;
using PoESkillTree.Computation.Common.Builders.Forms;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.Computation.Common.Builders.Values;
using PoESkillTree.Computation.Console;
using PoESkillTree.Computation.Console.Builders;
using PoESkillTree.Computation.Parsing;

namespace PoESkillTree.Computation.IntegrationTests
{
    [TestFixture]
    public class ParsingTest
    {
        private static IParser _parser;

        [OneTimeSetUp]
        public static void ClassInit()
        {
            _parser = Program.CreateParser();
        }

        [Test, TestCaseSource(nameof(ReadParsableStatLines))]
        public void Parses(string statLine)
        {
            var (success, remaining, result) = _parser.Parse(statLine);

            Assert.IsTrue(success, $"{remaining}\nResult:\n  {string.Join("\n  ", result)}");
            CollectionAssert.IsEmpty(remaining);
            foreach (var modifier in result)
            {
                Assert.NotNull(modifier);
                Assert.NotNull(modifier.Stats);
                CollectionAssert.IsNotEmpty(modifier.Stats);
                Assert.NotNull(modifier.Form);
                Assert.NotNull(modifier.Value);
                Assert.NotNull(modifier.Source);
                var s = modifier.ToString();
                // Assert it has no unresolved references or values
                StringAssert.DoesNotContain("References", s);
                StringAssert.DoesNotContain("Values", s);
            }
        }

        [Test, TestCaseSource(nameof(ReadUnparsableStatLines))]
        public void DoesNotParse(string statLine)
        {
            var (success, remaining, result) = _parser.Parse(statLine);

            Assert.IsFalse(success, $"{remaining}\nResult:\n  {string.Join("\n  ", result)}");
            foreach (var modifier in result)
            {
                var s = modifier?.ToString();
                StringAssert.DoesNotContain("References", s);
                StringAssert.DoesNotContain("Values", s);
            }
        }

        private static string[] ReadParsableStatLines()
        {
            var unparsable = ReadUnparsableStatLines().ToHashSet();

            return ReadStatLines("AllSkillTreeStatLines")
                .Concat(ReadStatLines("ParsableStatLines"))
                .Where(s => !unparsable.Contains(s))
                .ToArray();
        }

        private static string[] ReadUnparsableStatLines()
        {
            return ReadStatLines("UnparsableStatLines")
                .Concat(ReadStatLines("NotYetParsableStatLines"))
                .ToArray();
        }

        private static IEnumerable<string> ReadStatLines(string fileName)
        {
            return File.ReadAllLines(TestContext.CurrentContext.TestDirectory + $"/Data/{fileName}.txt")
                .Where(s => !s.StartsWith("//", StringComparison.Ordinal))
                .Distinct()
                .Select(s => s.ToLowerInvariant());
        }

        [Test, TestCaseSource(nameof(ParsingReturnsCorrectModifiers_TestCases))]
        public IEnumerable<Modifier> ParsingReturnsCorrectModifiers(string statLine)
        {
            var (_, _, result) = _parser.Parse(statLine);
            return result;
        }

        private static IEnumerable<TestCaseData> ParsingReturnsCorrectModifiers_TestCases()
        {
            var f = new BuilderFactories();

            yield return new TestCaseData("+10 to Dexterity").Returns(new[]
            {
                CreateModifier(
                    f.StatBuilders.Attribute.Dexterity,
                    f.FormBuilders.BaseAdd,
                    f.ValueBuilders.Create(10))
            });

            yield return new TestCaseData("Gain 30 Mana per Grand Spectrum").Returns(new[]
            {
                CreateModifier(
                    f.StatBuilders.GrandSpectrumJewelsSocketed,
                    f.FormBuilders.BaseAdd,
                    f.ValueBuilders.Create(1)),
                CreateModifier(
                    f.StatBuilders.Pool.Mana,
                    f.FormBuilders.BaseAdd,
                    f.ValueBuilders.Create(30).Multiply(f.StatBuilders.GrandSpectrumJewelsSocketed.Value)),
            });

            yield return new TestCaseData(
                    "With 5 Corrupted Items Equipped: 50% of Chaos Damage does not bypass Energy Shield, and 50% of Physical Damage bypasses Energy Shield")
                .Returns(new[]
                {
                    CreateModifier(
                        f.DamageTypeBuilders.Chaos.Damage.TakenFrom(f.StatBuilders.Pool.EnergyShield)
                            .Before(f.StatBuilders.Pool.Life),
                        f.FormBuilders.BaseAdd,
                        f.ValueBuilders.Create(50),
                        f.EquipmentBuilders.Equipment.Count(e => e.IsCorrupted) >= 5),
                    CreateModifier(
                        f.DamageTypeBuilders.Physical.Damage.TakenFrom(f.StatBuilders.Pool.EnergyShield)
                            .Before(f.StatBuilders.Pool.Life),
                        f.FormBuilders.BaseSubtract,
                        f.ValueBuilders.Create(50),
                        f.EquipmentBuilders.Equipment.Count(e => e.IsCorrupted) >= 5)
                });

            yield return new TestCaseData(
                    "Life Leeched per Second is doubled.\nMaximum Life Leech Rate is doubled.\nLife Regeneration has no effect.")
                .Returns(new[]
                {
                    CreateModifier(
                        f.StatBuilders.Pool.Life.Leech.Rate,
                        f.FormBuilders.PercentMore,
                        f.ValueBuilders.Create(100)),
                    CreateModifier(
                        f.StatBuilders.Pool.Life.Leech.RateLimit,
                        f.FormBuilders.PercentMore,
                        f.ValueBuilders.Create(100)),
                    CreateModifier(
                        f.StatBuilders.Pool.Life.Regen,
                        f.FormBuilders.BaseOverride,
                        f.ValueBuilders.Create(0))
                });

            Modifier ParagonOfCalamityFor(IDamageTypeBuilder damageType) =>
                CreateModifier(
                    damageType.Damage.Taken,
                    f.FormBuilders.PercentReduce,
                    f.ValueBuilders.Create(8),
                    f.ActionBuilders.Hit.With(damageType).By(f.EntityBuilders.Enemy).Recently);

            yield return new TestCaseData(
                    "For each Element you've been hit by Damage of Recently, 8% reduced Damage taken of that Element")
                .Returns(new[]
                {
                    ParagonOfCalamityFor(f.DamageTypeBuilders.Fire),
                    ParagonOfCalamityFor(f.DamageTypeBuilders.Lightning),
                    ParagonOfCalamityFor(f.DamageTypeBuilders.Cold),
                });

            yield return new TestCaseData(
                    "Auras you Cast grant 3% increased Attack and Cast Speed to you and Allies")
                .Returns(new[]
                {
                    CreateModifier(
                        f.BuffBuilders.Buffs(f.EntityBuilders.Self, f.EntityBuilders.Self, f.EntityBuilders.Ally)
                            .With(f.KeywordBuilders.Aura).Without(f.KeywordBuilders.Curse)
                            .AddStat(f.StatBuilders.CastSpeed),
                        f.FormBuilders.PercentIncrease,
                        f.ValueBuilders.Create(3))
                });
        }

        private static Modifier CreateModifier(
            IStatBuilder statBuilder, IFormBuilder formBuilder, IValueBuilder valueBuilder)
        {
            var (stats, sourceConverter, statValueConverter) = statBuilder.Build();
            var (form, formValueConverter) = formBuilder.Build();
            var value = formValueConverter(statValueConverter(valueBuilder)).Build();
            return new Modifier(stats, form, value, sourceConverter(new ModifierSource.Global()));
        }

        private static Modifier CreateModifier(
            IStatBuilder statBuilder, IFormBuilder formBuilder, IValueBuilder valueBuilder, 
            IConditionBuilder conditionBuilder)
        {
            return CreateModifier(statBuilder.WithCondition(conditionBuilder), formBuilder, valueBuilder);
        }
    }
}