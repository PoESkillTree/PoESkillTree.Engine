using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Effects;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Core;
using PoESkillTree.Engine.Computation.IntegrationTests.Core;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.Utils.Extensions;

namespace PoESkillTree.Engine.Computation.IntegrationTests
{
    [TestFixture]
    public class MechanicsTest : CompositionRootTestBase
    {
#pragma warning disable 8618 // Initialized in ClassInit
        private static IReadOnlyList<Modifier> _givenMods;
        private static IBuilderFactories _builderFactories;
        private static IMetaStatBuilders _metaStats;
#pragma warning restore 8618

        private const double BasePhysicalDamage = 5000;
        private const double Accuracy = (-2 + 2 * 90) + 1000;
        private const double EffectiveDamageMultiplierWithNonCrits = 0.4 * 1.2 * 1.75;

        private static double ChanceToHit(ICalculator calculator)
        {
            var enemyEvasionStat =
                Build(_builderFactories.StatBuilders.Evasion.For(_builderFactories.EntityBuilders.Enemy)).Single();
            var enemyEvasion = calculator.NodeRepository.GetNode(enemyEvasionStat).Value.Single();
            return 1.15 * Accuracy / (Accuracy + Math.Pow(enemyEvasion / 4.0, 0.8));
        }

        private static double DamageReductionFromArmour(ICalculator calculator, double additionalDamageMultiplier = 1)
        {
            var enemyArmourStat = Build(_builderFactories.StatBuilders.Armour.For(_builderFactories.EntityBuilders.Enemy)).Single();
            var enemyArmour = calculator.NodeRepository.GetNode(enemyArmourStat).Value.Single();
            var physicalDamageStat = BuildMainHandSkillSingle(_builderFactories.DamageTypeBuilders.Physical.Damage);
            var physicalDamage = calculator.NodeRepository.GetNode(physicalDamageStat).Value.Single();
            var damageMultiplierStat = BuildMainHandSkillSingle(_builderFactories.DamageTypeBuilders.Physical.DamageMultiplierWithNonCrits);
            var damageMultiplier = calculator.NodeRepository.GetNode(damageMultiplierStat).Value.Single() / 100;
            return enemyArmour / (enemyArmour + 10 * physicalDamage * damageMultiplier * additionalDamageMultiplier);
        }

        private static double EffectiveDamageMultiplierWithNonCritsIncludingArmour(ICalculator calculator)
            => (1 - (0.6 + DamageReductionFromArmour(calculator))) * 1.2 * 1.75;

        [OneTimeSetUp]
        public static async Task ClassInit()
        {
            _builderFactories = await BuilderFactoriesTask.ConfigureAwait(false);
            _metaStats = _builderFactories.MetaStatBuilders;
            var parser = await ParserTask.ConfigureAwait(false);
            var modSource = new ModifierSource.Global();
            _givenMods = parser.ParseGivenModifiers()
                .Append(
                    new Modifier(
                        Build(_builderFactories.DamageTypeBuilders.Physical.Damage.Taken
                            .For(_builderFactories.EntityBuilders.Enemy)),
                        Form.Increase, new Constant(20), modSource),
                    new Modifier(
                        Build(_builderFactories.DamageTypeBuilders.Physical.Resistance
                            .For(_builderFactories.EntityBuilders.Enemy)),
                        Form.BaseSet, new Constant(60), modSource),
                    new Modifier(
                        Build(_builderFactories.DamageTypeBuilders.Physical.DamageMultiplier),
                        Form.BaseAdd, new Constant(75), modSource),
                    new Modifier(
                        Build(_builderFactories.StatBuilders.Level),
                        Form.BaseSet, new Constant(90), modSource),
                    new Modifier(
                        Build(_builderFactories.DamageTypeBuilders.Physical.Damage.WithSkills),
                        Form.BaseSet, new Constant(BasePhysicalDamage), modSource),
                    new Modifier(
                        Build(_builderFactories.StatBuilders.Accuracy),
                        Form.BaseAdd, new Constant(1000), modSource),
                    new Modifier(
                        Build(_metaStats.SkillHitDamageSource),
                        Form.BaseSet, new Constant((int) DamageSource.Attack), modSource),
                    new Modifier(
                        Build(_metaStats.SkillUsesHand(AttackDamageHand.MainHand)),
                        Form.BaseSet, new Constant(true), modSource),
                    new Modifier(
                        Build(_metaStats.SkillUsesHand(AttackDamageHand.OffHand)),
                        Form.BaseSet, new Constant(true), modSource),
                    new Modifier(
                        Build(_builderFactories.EquipmentBuilders.Equipment[ItemSlot.MainHand].ItemTags),
                        Form.TotalOverride, new Constant(Tags.Sword.EncodeAsDouble()), modSource))
                .ToList();
        }

        [Test]
        public void SkillDpsWithHits()
        {
            var calculator = Calculator.Create();
            var nodes = calculator.NodeRepository;

            calculator.NewBatchUpdate()
                .AddModifiers(_givenMods)
                .AddModifier(Build(_builderFactories.StatBuilders.CastRate.With(DamageSource.Attack)),
                    Form.BaseSet, 2)
                .AddModifier(Build(_builderFactories.ActionBuilders.CriticalStrike.Chance), Form.BaseSet, 10)
                .AddModifier(Build(_builderFactories.DamageTypeBuilders.Physical.Penetration), Form.BaseAdd, 10)
                .AddModifier(Build(_builderFactories.DamageTypeBuilders.Physical.Damage.ChanceToDouble), Form.BaseAdd,
                    20)
                .AddModifier(Build(_metaStats.DamageBaseSetEffectiveness), Form.BaseSet, 2)
                .DoUpdate();

            var chanceToHit = ChanceToHit(calculator);
            var actual = nodes
                .GetNode(BuildMainHandSkillSingle(_metaStats.EnemyResistanceAgainstNonCrits(DamageType.Physical)))
                .Value.Single();
            var expectedEnemyResistanceAgainstNonCrits = 60 + 100 * DamageReductionFromArmour(calculator) - 10;
            Assert.AreEqual(expectedEnemyResistanceAgainstNonCrits, actual, 1e-10);
            actual = nodes
                .GetNode(BuildMainHandSkillSingle(_metaStats.EnemyResistanceAgainstCrits(DamageType.Physical)))
                .Value.Single();
            var expectedEnemyResistanceAgainstCrits = 60 + 100 * DamageReductionFromArmour(calculator, 1.5) - 10;
            Assert.AreEqual(expectedEnemyResistanceAgainstCrits, actual, 1e-10);
            actual = nodes
                .GetNode(
                    BuildMainHandSkillSingle(_metaStats.EffectiveDamageMultiplierWithNonCrits(DamageType.Physical)))
                .Value.Single();
            var expectedEffectiveDamageMultiplierWithNonCrits = (1 - expectedEnemyResistanceAgainstNonCrits / 100d) * 1.2 * 1.75;
            Assert.AreEqual(expectedEffectiveDamageMultiplierWithNonCrits, actual, 1e-10);
            actual = nodes
                .GetNode(BuildMainHandSkillSingle(_metaStats.EffectiveDamageMultiplierWithCrits(DamageType.Physical)))
                .Value.Single();
            var expectedEffectiveDamageMultiplierWithCrits = (1 - expectedEnemyResistanceAgainstCrits / 100d) * 1.2 * 1.75 * 1.5;
            Assert.AreEqual(expectedEffectiveDamageMultiplierWithCrits, actual, 1e-10);
            actual = nodes
                .GetNode(BuildMainHandSkillSingle(_metaStats.DamageWithNonCrits(DamageType.Physical)))
                .Value.Single();

            var baseDamage = BasePhysicalDamage * 2;
            var doubleDamageMultiplier = 1.2;
            var expectedDamageWithNonCrits =
                baseDamage * expectedEffectiveDamageMultiplierWithNonCrits * doubleDamageMultiplier;
            Assert.AreEqual(expectedDamageWithNonCrits, actual, 1e-10);
            actual = nodes
                .GetNode(BuildMainHandSkillSingle(_metaStats.DamageWithCrits(DamageType.Physical)))
                .Value.Single();
            var expectedDamageWithCrits =
                baseDamage * expectedEffectiveDamageMultiplierWithCrits * doubleDamageMultiplier;
            Assert.AreEqual(expectedDamageWithCrits, actual, 1e-10);
            actual = nodes
                .GetNode(BuildMainHandSkillSingle(_builderFactories.StatBuilders.ChanceToHit))
                .Value.Single();
            Assert.AreEqual(chanceToHit * 100, actual, 1e-10);
            actual = nodes
                .GetNode(BuildMainHandSkillSingle(_metaStats.AverageDamagePerHit))
                .Value.Single();
            var effectiveCritChance = 0.1 * chanceToHit;
            var expectedAverageDamagePerHit = expectedDamageWithNonCrits * (1 - effectiveCritChance) +
                                              expectedDamageWithCrits * effectiveCritChance;
            Assert.AreEqual(expectedAverageDamagePerHit, actual, 1e-10);
            actual = nodes
                .GetNode(BuildMainHandSkillSingle(_metaStats.AverageDamage))
                .Value.Single();
            var expectedAverageDamage = expectedAverageDamagePerHit * chanceToHit;
            Assert.AreEqual(expectedAverageDamage, actual, 1e-10);
            actual = nodes
                .GetNode(Build(_metaStats.SkillDpsWithHits).Single())
                .Value.Single();
            var expectedSkillDpsWithHits = expectedAverageDamage * 2;
            Assert.AreEqual(expectedSkillDpsWithHits, actual, 1e-10);
        }

        [Test]
        public void SkillDpsWithDoTs()
        {
            var calculator = Calculator.Create();
            var nodes = calculator.NodeRepository;

            calculator.NewBatchUpdate()
                .AddModifiers(_givenMods)
                .DoUpdate();

            var actual = nodes
                .GetNode(Build(_metaStats.EffectiveDamageMultiplierWithNonCrits(DamageType.Physical)
                    .WithSkills(DamageSource.OverTime)).Single())
                .Value.Single();
            Assert.AreEqual(EffectiveDamageMultiplierWithNonCrits, actual);
            actual = nodes
                .GetNode(Build(_metaStats.DamageWithNonCrits(DamageType.Physical)
                    .WithSkills(DamageSource.OverTime)).Single())
                .Value.Single();
            var expectedDamageWithNonCrits = BasePhysicalDamage * EffectiveDamageMultiplierWithNonCrits;
            Assert.AreEqual(expectedDamageWithNonCrits, actual);
            actual = nodes
                .GetNode(Build(_metaStats.SkillDpsWithDoTs).Single())
                .Value.Single();
            var expectedSkillDpsWithDoTs = expectedDamageWithNonCrits;
            Assert.AreEqual(expectedSkillDpsWithDoTs, actual);
        }

        [Test]
        public void BleedDps()
        {
            var calculator = Calculator.Create();
            var nodes = calculator.NodeRepository;

            calculator.NewBatchUpdate()
                .AddModifiers(_givenMods)
                .AddModifier(Build(_builderFactories.StatBuilders.CastRate.With(DamageSource.Attack)), Form.BaseSet, 2)
                .AddModifier(Build(_builderFactories.ActionBuilders.CriticalStrike.Chance), Form.BaseSet, 10)
                .AddModifier(Build(_builderFactories.EffectBuilders.Ailment.Bleed.Chance), Form.BaseSet, 10)
                .AddModifier(Build(_builderFactories.EffectBuilders.Ailment.Bleed.CriticalStrikesAlwaysInflict),
                    Form.BaseSet, 1)
                .AddModifier(Build(_builderFactories.DamageTypeBuilders.Physical.DamageMultiplierWithCrits
                        .With(_builderFactories.EffectBuilders.Ailment.Bleed)),
                    Form.BaseAdd, 50)
                .DoUpdate();

            var critChance = 0.1 * ChanceToHit(calculator);
            var ailmentChanceNonCrits = 0.1;
            var ailmentChanceCrits = 1;
            var baseDamage = BasePhysicalDamage * 0.7;
            var nonCritDamage = baseDamage * EffectiveDamageMultiplierWithNonCrits;
            var critDamage = baseDamage * 0.4 * 1.2 * 2.75;
            var expected = (nonCritDamage * (1 - critChance) * ailmentChanceNonCrits +
                            critDamage * critChance * ailmentChanceCrits) /
                           ((1 - critChance) * ailmentChanceNonCrits + critChance * ailmentChanceCrits);
            var actual = nodes
                .GetNode(Build(_metaStats.AilmentDps(Ailment.Bleed)).Single())
                .Value.Single();
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [Test]
        public void ResistanceAgainstPhysicalHits()
        {
            var calculator = Calculator.Create();
            var nodes = calculator.NodeRepository;

            calculator.NewBatchUpdate()
                .AddModifiers(_givenMods)
                .AddModifier(Build(_builderFactories.StatBuilders.Armour), Form.BaseAdd, 4000)
                .AddModifier(Build(_builderFactories.DamageTypeBuilders.Physical.Resistance), Form.BaseAdd, 50)
                .DoUpdate();

            var enemyDamageStat = BuildMainHandSkillSingle(
                _builderFactories.DamageTypeBuilders.Physical.Damage.For(_builderFactories.EntityBuilders.Enemy));
            var enemyDamage = nodes.GetNode(enemyDamageStat).Value.Single();
            var armour = 4000;
            var expected = 50 + 100 * armour / (armour + 10 * enemyDamage);
            var actual = nodes
                .GetNode(Build(_metaStats.ResistanceAgainstHits(DamageType.Physical)).Single())
                .Value.Single();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void StunChance()
        {
            var calculator = Calculator.Create();
            var nodes = calculator.NodeRepository;

            calculator.NewBatchUpdate()
                .AddModifiers(_givenMods)
                .AddModifier(Build(_builderFactories.StatBuilders.Armour), Form.BaseAdd, 4000)
                .AddModifier(
                    Build(_builderFactories.EffectBuilders.Stun.Threshold.For(_builderFactories.EntityBuilders.Enemy)),
                    Form.Increase, -100)
                .DoUpdate();

            var enemyLifeStat = Build(_builderFactories.StatBuilders.Pool.From(Pool.Life)
                .For(_builderFactories.EntityBuilders.Enemy)).Single();
            var enemyLife = nodes.GetNode(enemyLifeStat).Value.Single();
            var damage = BasePhysicalDamage * EffectiveDamageMultiplierWithNonCritsIncludingArmour(calculator) * ChanceToHit(calculator);
            var expected = 200 * damage / (enemyLife * 0.125);
            var actual = nodes
                .GetNode(Build(_builderFactories.EffectBuilders.Stun.Chance.With(AttackDamageHand.MainHand)).Single())
                .Value.Single();
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [Test]
        public void AffectedByMinionDamageAndAttackRateIncreases()
        {
            var calculator = Calculator.Create();
            var nodes = calculator.NodeRepository;

            var minion = _builderFactories.EntityBuilders.Minion;
            var physicalDamage = _builderFactories.DamageTypeBuilders.Physical.Damage;
            calculator.NewBatchUpdate()
                .AddModifiers(_givenMods)
                .AddModifier(Build(_builderFactories.StatBuilders.CastRate.With(DamageSource.Attack)), Form.BaseSet, 1)
                .AddModifier(Build(_builderFactories.StatBuilders.ChanceToHit), Form.TotalOverride, 100)
                .AddModifier(Build(physicalDamage.For(minion)), Form.Increase, 100)
                .AddModifier(Build(_builderFactories.StatBuilders.CastRate.For(minion)), Form.Increase, 100)
                .AddModifier(Build(_builderFactories.StatBuilders.Flag.IncreasesToSourceApplyToTarget(
                            physicalDamage.For(minion), physicalDamage)),
                    Form.TotalOverride, 1)
                .DoUpdate();

            var averageDamage = BasePhysicalDamage * 2 * EffectiveDamageMultiplierWithNonCritsIncludingArmour(calculator);
            var actual = nodes
                .GetNode(Build(_metaStats.SkillDpsWithHits).Single())
                .Value.Single();
            var expectedSkillDpsWithHits = averageDamage;
            Assert.AreEqual(expectedSkillDpsWithHits, actual);
        }

        private static IStat BuildMainHandSkillSingle(IDamageRelatedStatBuilder builder)
            => Build(builder.WithSkills.With(AttackDamageHand.MainHand)).Single();

        private static IReadOnlyList<IStat> Build(IStatBuilder builder)
            => builder.Build(default).SelectMany(r => r.Stats).ToList();
    }
}