using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Effects;
using PoESkillTree.Engine.Computation.Common.Builders.Forms;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.Computation.Data.Collections;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.Utils.Extensions;

namespace PoESkillTree.Engine.Computation.Data.GivenStats
{
    public class DamageMechanics : DataDrivenMechanicsBase
    {
        private readonly Lazy<IReadOnlyList<IIntermediateModifier>> _lazyGivenStats;

        public DamageMechanics(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories, modifierBuilder)
        {
            _lazyGivenStats = new Lazy<IReadOnlyList<IIntermediateModifier>>(
                () => CollectionToList(CreateCollection()));
        }

        public override IReadOnlyList<Entity> AffectedEntities { get; } = new[] {GameModel.Entity.Character};

        public override IReadOnlyList<IIntermediateModifier> GivenModifiers => _lazyGivenStats.Value;

        private DataDrivenMechanicCollection CreateCollection()
            => new DataDrivenMechanicCollection(ModifierBuilder, BuilderFactories)
            {
                // skill hit damage
                // - DPS
                {
                    TotalOverride, MetaStats.SkillDpsWithHits,
                    MetaStats.AverageHitDamage.Value *
                    ValueFactory.If(MetaStats.SkillDpsWithHitsCalculationMode.Value.Eq((double) DpsCalculationMode.HitRateBased))
                        .Then(Stat.HitRate.Value)
                        .ElseIf(MetaStats.SkillDpsWithHitsCalculationMode.Value.Eq((double) DpsCalculationMode.CooldownBased))
                        .Then(1000 / Stat.Cooldown.Value * Stat.SkillNumberOfHitsPerCast.Value)
                        .ElseIf(MetaStats.SkillDpsWithHitsCalculationMode.Value.Eq((double) DpsCalculationMode.AverageCast))
                        .Then(Stat.SkillNumberOfHitsPerCast.Value)
                        .Else(MetaStats.CastRate.Value * Stat.SkillNumberOfHitsPerCast.Value)
                },
                {
                    BaseSet, MetaStats.SkillDpsWithHitsCalculationMode,
                    ValueFactory.If(Stat.HitRate.IsSet)
                        .Then((double) DpsCalculationMode.HitRateBased)
                        .ElseIf(And(Stat.Cooldown.Value > 0, Not(MetaStats.CanBypassSkillCooldown.IsTrue.And(Flag.BypassSkillCooldown))))
                        .Then((double) DpsCalculationMode.CooldownBased)
                        .ElseIf(And(MetaStats.SkillHitDamageSource.Value.Eq((double) DamageSource.Spell),
                            Stat.BaseCastTime.With(DamageSource.Spell).Value <= 0))
                        .Then((double) DpsCalculationMode.AverageCast)
                        .ElseIf(And(MetaStats.SkillHitDamageSource.Value.Eq((double) DamageSource.Secondary),
                            Stat.BaseCastTime.With(DamageSource.Secondary).Value <= 0))
                        .Then((double) DpsCalculationMode.AverageCast)
                        .ElseIf(MetaStats.MainSkillHasKeyword(GameModel.Skills.Keyword.Triggered).IsTrue)
                        .Then((double) DpsCalculationMode.AverageCast)
                        .Else((double) DpsCalculationMode.CastRateBased)
                },
                // - average damage
                {
                    TotalOverride, MetaStats.AverageHitDamage,
                    CombineSource(MetaStats.AverageDamage.WithHits, CombineHandsForHitDamage)
                },
                // - average damage per source
                {
                    TotalOverride, MetaStats.AverageDamage.WithHits.With(AttackDamageHand.MainHand),
                    MetaStats.AverageDamagePerHit.With(AttackDamageHand.MainHand).Value *
                    Stat.ChanceToHit.With(AttackDamageHand.MainHand).Value.AsPercentage
                },
                {
                    TotalOverride, MetaStats.AverageDamage.WithHits.With(AttackDamageHand.OffHand),
                    MetaStats.AverageDamagePerHit.With(AttackDamageHand.OffHand).Value *
                    Stat.ChanceToHit.With(AttackDamageHand.OffHand).Value.AsPercentage
                },
                {
                    TotalOverride, MetaStats.AverageDamage.WithHits.With(DamageSource.Spell),
                    MetaStats.AverageDamagePerHit.With(DamageSource.Spell).Value
                },
                {
                    TotalOverride, MetaStats.AverageDamage.WithHits.With(DamageSource.Secondary),
                    MetaStats.AverageDamagePerHit.With(DamageSource.Secondary).Value
                },
                // - average damage of a successful hit per source
                {
                    TotalOverride, MetaStats.AverageDamagePerHit,
                    MetaStats.DamageWithNonCrits().WithHits,
                    MetaStats.DamageWithCrits().WithHits,
                    MetaStats.EffectiveCritChance,
                    (nonCritDamage, critDamage, critChance)
                        => nonCritDamage.Value.Average * (1 - critChance.Value) +
                           critDamage.Value.Average * critChance.Value
                },
                // - crit/non-crit damage per source and type
                {
                    TotalOverride, dt => MetaStats.DamageWithNonCrits(dt).WithHits,
                    dt => MetaStats.Damage(dt).WithHits,
                    dt => MetaStats.EffectiveDamageMultiplierWithNonCrits(dt).WithHits,
                    dt => MetaStats.Damage(dt).WithHits.ChanceToDouble,
                    (_, damage, mult, chanceToDouble)
                        => damage.Value * mult.Value * (1 + chanceToDouble.Value.AsPercentage)
                },
                {
                    TotalOverride, dt => MetaStats.DamageWithCrits(dt).WithHits,
                    dt => MetaStats.Damage(dt).WithHits,
                    dt => MetaStats.EffectiveDamageMultiplierWithCrits(dt).WithHits,
                    dt => MetaStats.Damage(dt).WithHits.ChanceToDouble,
                    (_, damage, mult, chanceToDouble)
                        => damage.Value * mult.Value * (1 + chanceToDouble.Value.AsPercentage)
                },
                // - effective crit/non-crit damage multiplier per source and type
                {
                    BaseSet, dt => MetaStats.EffectiveDamageMultiplierWithNonCrits(dt).WithHits,
                    dt => MetaStats.EnemyResistanceAgainstNonCrits(dt),
                    dt => DamageTaken(dt).WithHits.For(Enemy),
                    dt => DamageMultiplierWithNonCrits(dt).WithHits,
                    _ => MetaStats.EffectiveImpaleDamageMultiplierAgainstNonCrits,
                    (dt, resistance, damageTaken, damageMulti, impaleMulti) =>
                        ((1 - resistance.Value.AsPercentage) + impaleMulti.Value) * damageTaken.Value * damageMulti.Value.AsPercentage
                },
                {
                    BaseSet, dt => MetaStats.EffectiveDamageMultiplierWithCrits(dt).WithHits,
                    dt => MetaStats.EnemyResistanceAgainstCrits(dt),
                    dt => DamageTaken(dt).WithHits.For(Enemy),
                    dt => DamageMultiplierWithCrits(dt).WithHits,
                    _ => MetaStats.EffectiveImpaleDamageMultiplierAgainstNonCrits,
                    _ => CriticalStrike.Multiplier.WithHits,
                    (_, resistance, damageTaken, damageMulti, impaleMulti, critMulti) =>
                        ((1 - resistance.Value.AsPercentage) + impaleMulti.Value) * damageTaken.Value * damageMulti.Value.AsPercentage
                        * EffectiveCriticalStrikeMultiplier(critMulti)
                },
                // - enemy resistance against crit/non-crit hits per source and type
                {
                    BaseSet, dt => MetaStats.EnemyResistanceAgainstNonCrits(dt),
                    dt => DamageTypeBuilders.From(dt).IgnoreResistanceWithNonCrits,
                    dt => DamageTypeBuilders.From(dt).PenetrationWithNonCrits,
                    _ => MetaStats.EnemyResistanceFromArmourAgainstNonCrits,
                    EnemyResistanceAgainstHits
                },
                { TotalOverride, MetaStats.EnemyResistanceAgainstNonCrits(DamageType.Physical).Minimum, 0 },
                { TotalOverride, MetaStats.EnemyResistanceAgainstNonCrits(DamageType.Physical).Maximum, 100 },
                {
                    BaseSet, dt => MetaStats.EnemyResistanceAgainstCrits(dt),
                    dt => DamageTypeBuilders.From(dt).IgnoreResistanceWithCrits,
                    dt => DamageTypeBuilders.From(dt).PenetrationWithCrits,
                    _ => MetaStats.EnemyResistanceFromArmourAgainstCrits,
                    EnemyResistanceAgainstHits
                },
                { TotalOverride, MetaStats.EnemyResistanceAgainstCrits(DamageType.Physical).Minimum, 0 },
                { TotalOverride, MetaStats.EnemyResistanceAgainstCrits(DamageType.Physical).Maximum, 100 },
                // - enemy resistance from armour against crit/non-crit hits per source (physical damage only)
                {
                    TotalOverride, MetaStats.EnemyResistanceFromArmourAgainstNonCrits,
                    Physical.Damage.WithHits,
                    DamageMultiplierWithNonCrits(DamageType.Physical).WithHits,
                    (damage, multi) =>
                        PhysicalDamageReductionFromArmour(Armour.For(Enemy).Value,
                            damage.Value * multi.Value.AsPercentage)
                },
                {
                    TotalOverride, MetaStats.EnemyResistanceFromArmourAgainstCrits,
                    Physical.Damage.WithHits,
                    DamageMultiplierWithCrits(DamageType.Physical).WithHits,
                    CriticalStrike.Multiplier.WithHits,
                    (damage, multi, critMulti) =>
                        PhysicalDamageReductionFromArmour(Armour.For(Enemy).Value,
                            damage.Value * multi.Value.AsPercentage * EffectiveCriticalStrikeMultiplier(critMulti))
                },

                // skill damage over time
                // - DPS = average damage = non-crit damage
                {
                    TotalOverride, MetaStats.SkillDpsWithDoTs,
                    MetaStats.AverageDamage.WithSkills(DamageSource.OverTime).Value
                },
                {
                    TotalOverride, MetaStats.AverageDamage.WithSkills(DamageSource.OverTime),
                    MetaStats.DamageWithNonCrits().WithSkills(DamageSource.OverTime).Value
                },
                // - damage per type
                {
                    TotalOverride, dt => MetaStats.DamageWithNonCrits(dt).WithSkills(DamageSource.OverTime),
                    dt => MetaStats.Damage(dt).WithSkills(DamageSource.OverTime).Value *
                          MetaStats.EffectiveDamageMultiplierWithNonCrits(dt).WithSkills(DamageSource.OverTime).Value
                },
                // - effective damage multiplier per type
                {
                    BaseSet,
                    dt => MetaStats.EffectiveDamageMultiplierWithNonCrits(dt).WithSkills(DamageSource.OverTime),
                    dt => EnemyDamageTakenMultiplier(dt, DamageTaken(dt).WithSkills(DamageSource.OverTime))
                          * DamageMultiplierWithNonCrits(dt).WithSkills(DamageSource.OverTime).Value.AsPercentage
                },

                // ailment damage (modifiers for EffectiveDamageMultiplierWith[Non]Crits() and Damage() are added below
                // this collection initializer)
                // - DPS
                {
                    TotalOverride, MetaStats.AilmentDps,
                    ailment => MetaStats.AverageAilmentDamage(ailment).Value *
                               MetaStats.AilmentEffectiveInstances(ailment).Value *
                               Ailment.From(ailment).TickRateModifier.Value
                },
                // - average damage
                {
                    TotalOverride, ailment => MetaStats.AverageAilmentDamage(ailment),
                    ailment => CombineSource(MetaStats.AverageDamage.With(Ailment.From(ailment)),
                        CombineHandsForAverageAilmentDamage(ailment))
                },
                // - lifetime damage of one instance
                {
                    TotalOverride, MetaStats.AilmentInstanceLifetimeDamage,
                    ailment => MetaStats.AverageAilmentDamage(ailment).Value * Ailment.From(ailment).Duration.Value
                },
                // - average damage per source
                {
                    TotalOverride, ailment => MetaStats.AverageDamage.With(Ailment.From(ailment)),
                    ailment => MetaStats.DamageWithNonCrits().With(Ailment.From(ailment)),
                    ailment => MetaStats.DamageWithCrits().With(Ailment.From(ailment)),
                    _ => MetaStats.EffectiveCritChance,
                    ailment => Ailment.From(ailment).Chance,
                    ailment => MetaStats.AilmentChanceWithCrits(ailment),
                    AverageAilmentDamageFromCritAndNonCrit
                },
                // - crit/non-crit damage per source and type
                {
                    TotalOverride, (a, dt) => MetaStats.DamageWithNonCrits(dt).With(Ailment.From(a)),
                    (a, dt) => MetaStats.Damage(dt).With(Ailment.From(a)),
                    (a, dt) => MetaStats.EffectiveDamageMultiplierWithNonCrits(dt).With(Ailment.From(a)),
                    (damage, mult) => damage.Value * mult.Value
                },
                {
                    TotalOverride, (a, dt) => MetaStats.DamageWithCrits(dt).With(Ailment.From(a)),
                    (a, dt) => MetaStats.Damage(dt).With(Ailment.From(a)),
                    (a, dt) => MetaStats.EffectiveDamageMultiplierWithCrits(dt).With(Ailment.From(a)),
                    (damage, mult) => damage.Value * mult.Value
                },

                // speed
                {
                    // Attack is set through ItemPropertyParser if the slot is not empty
                    BaseSet, Stat.CastRate.With(AttackDamageHand.MainHand),
                    Stat.BaseCastTime.With(AttackDamageHand.MainHand).Value.Invert,
                    Not(MainHand.HasItem)
                },
                {
                    BaseSet, Stat.CastRate.With(DamageSource.Spell),
                    Stat.BaseCastTime.With(DamageSource.Spell).Value.Invert
                },
                {
                    BaseSet, Stat.CastRate.With(DamageSource.Secondary),
                    Stat.BaseCastTime.With(DamageSource.Secondary).Value.Invert
                },
                {
                    BaseSet, MetaStats.CastRate,
                    CombineSourceDefaultingToSpell(Stat.CastRate, CombineHandsByAverage)
                },
                { BaseAdd, MetaStats.CastRate, Stat.AdditionalCastRate.Value },
                { TotalOverride, MetaStats.CastTime, MetaStats.CastRate.Value.Invert },
                // chance to hit
                {
                    BaseSet, Stat.ChanceToHit.With(AttackDamageHand.MainHand),
                    ChanceToHitValue(Stat.Accuracy.With(AttackDamageHand.MainHand), Evasion.For(Enemy),
                        Buff.Blind.IsOn(Self))
                },
                {
                    BaseSet, Stat.ChanceToHit.With(AttackDamageHand.OffHand),
                    ChanceToHitValue(Stat.Accuracy.With(AttackDamageHand.OffHand), Evasion.For(Enemy),
                        Buff.Blind.IsOn(Self))
                },
                // crit
                {
                    TotalOverride, MetaStats.EffectiveCritChance.With(AttackDamageHand.MainHand),
                    CalculateLuckyCriticalStrikeChance(CriticalStrike.Chance.With(AttackDamageHand.MainHand)) *
                    Stat.ChanceToHit.With(AttackDamageHand.MainHand).Value.AsPercentage
                },
                {
                    TotalOverride, MetaStats.EffectiveCritChance.With(AttackDamageHand.OffHand),
                    CalculateLuckyCriticalStrikeChance(CriticalStrike.Chance.With(AttackDamageHand.OffHand)) *
                    Stat.ChanceToHit.With(AttackDamageHand.OffHand).Value.AsPercentage
                },
                {
                    TotalOverride, MetaStats.EffectiveCritChance.With(DamageSource.Spell),
                    CalculateLuckyCriticalStrikeChance(CriticalStrike.Chance.With(DamageSource.Spell))
                },
                {
                    TotalOverride, MetaStats.EffectiveCritChance.With(DamageSource.Secondary),
                    CalculateLuckyCriticalStrikeChance(CriticalStrike.Chance.With(DamageSource.Secondary))
                },
                // leech
                { TotalOverride, MetaStats.EffectiveLeechRate, p => p.Leech.Rate.Value * p.RecoveryRate.Value },
                {
                    TotalOverride, MetaStats.AbsoluteLeechRate,
                    p => Stat.Pool.From(p).Value * MetaStats.EffectiveLeechRate(p).Value.AsPercentage
                },
                {
                    TotalOverride, MetaStats.AbsoluteLeechRateLimit,
                    p => Stat.Pool.From(p).Value * Stat.Pool.From(p).Leech.RateLimit.Value.AsPercentage
                },
                {
                    TotalOverride, MetaStats.TimeToReachLeechRateLimit,
                    p => p.Leech.RateLimit.Value / p.Leech.Rate.Value /
                         (MetaStats.CastRate.Value * Stat.SkillNumberOfHitsPerCast.Value)
                },
                // ailments
                {
                    TotalOverride, MetaStats.AilmentCombinedEffectiveChance,
                    ailment => CombineSource(MetaStats.AilmentEffectiveChance(ailment), CombineHandsByAverage)
                },
                {
                    TotalOverride, MetaStats.AilmentEffectiveChance,
                    ailment => Ailment.From(ailment).Chance,
                    ailment => MetaStats.AilmentChanceWithCrits(ailment),
                    _ => MetaStats.EffectiveCritChance,
                    (ailment, ailmentChance, ailmentChanceWithCrits, critChance)
                        => (ailmentChance.Value.AsPercentage * (1 - critChance.Value) +
                            ailmentChanceWithCrits.Value.AsPercentage * critChance.Value) *
                           (1 - Ailment.From(ailment).Avoidance.For(Enemy).Value.AsPercentage)
                },
                {
                    TotalOverride, MetaStats.AilmentChanceWithCrits,
                    ailment => Ailment.From(ailment).Chance,
                    (ailment, ailmentChance) => ValueFactory
                        .If(Ailment.From(ailment).CriticalStrikesAlwaysInflict.IsTrue).Then(100)
                        .Else(ailmentChance.Value)
                },
                // - AilmentEffectiveInstances
                {
                    TotalOverride, MetaStats.AilmentEffectiveInstances(Common.Builders.Effects.Ailment.Ignite),
                    Ailment.Ignite.InstancesOn(Enemy).Maximum.Value
                },
                {
                    TotalOverride, MetaStats.AilmentEffectiveInstances(Common.Builders.Effects.Ailment.Bleed),
                    Ailment.Bleed.InstancesOn(Enemy).Maximum.Value
                },
                {
                    TotalOverride, MetaStats.AilmentEffectiveInstances(Common.Builders.Effects.Ailment.Poison),
                    Ailment.Poison.Duration.Value * MetaStats.CastRate.Value *
                    Stat.SkillNumberOfHitsPerCast.Value *
                    CombineSource(MetaStats.AilmentEffectiveChance(Common.Builders.Effects.Ailment.Poison),
                        CombineHandsForAilmentEffectiveInstances(Common.Builders.Effects.Ailment.Poison))
                },
                // Impale
                {
                    TotalOverride, MetaStats.EffectiveImpaleDamageMultiplierAgainstNonCrits,
                    MetaStats.ImpaleDamageMultiplier,
                    MetaStats.EnemyResistanceAgainstNonCritImpales,
                    (multi, resistance) => multi.Value * (1 - resistance.Value.AsPercentage)
                },
                {
                    TotalOverride, MetaStats.EffectiveImpaleDamageMultiplierAgainstCrits,
                    MetaStats.ImpaleDamageMultiplier,
                    MetaStats.EnemyResistanceAgainstCritImpales,
                    (multi, resistance) => multi.Value * (1 - resistance.Value.AsPercentage)
                },
                {
                    TotalOverride, MetaStats.ImpaleDamageMultiplier,
                    Buff.Impale.Chance,
                    chance => ValueFactory.If(Buff.Impale.IsOn(Self, Enemy))
                        .Then(MetaStats.ImpaleRecordedDamage.Value * Buff.Impale.StackCount.For(Enemy).Value
                              * chance.WithCondition(Hit.On).Value.AsPercentage)
                        .Else(0)
                },
                {
                    BaseSet, MetaStats.EnemyResistanceAgainstNonCritImpales,
                    Physical.Damage.WithHits,
                    DamageMultiplierWithNonCrits(DamageType.Physical).WithHits,
                    (damage, multi) =>
                        DamageTypeBuilders.From(DamageType.Physical).Resistance.For(Enemy).Value
                        + PhysicalDamageReductionFromArmour(Armour.For(Enemy).Value,
                            MetaStats.ImpaleRecordedDamage.Value * damage.Value * multi.Value.AsPercentage)
                        - Buff.Impale.Penetration.Value
                },
                {
                    BaseSet, MetaStats.EnemyResistanceAgainstCritImpales,
                    Physical.Damage.WithHits,
                    DamageMultiplierWithCrits(DamageType.Physical).WithHits,
                    CriticalStrike.Multiplier.WithHits,
                    (damage, multi, critMulti) =>
                        DamageTypeBuilders.From(DamageType.Physical).Resistance.For(Enemy).Value
                        + PhysicalDamageReductionFromArmour(Armour.For(Enemy).Value,
                            MetaStats.ImpaleRecordedDamage.Value * damage.Value * multi.Value.AsPercentage * EffectiveCriticalStrikeMultiplier(critMulti))
                        - Buff.Impale.Penetration.Value
                },
                { TotalOverride, MetaStats.EnemyResistanceAgainstNonCritImpales.Minimum, 0 },
                { TotalOverride, MetaStats.EnemyResistanceAgainstNonCritImpales.Maximum, 100 },
                { TotalOverride, MetaStats.ImpaleRecordedDamage, 0.1 * Buff.Impale.EffectOn(Enemy).Value },
                { TotalOverride, Buff.Impale.Chance.WithCondition(Hit.On).Maximum, 100 },
                // stun (see https://pathofexile.gamepedia.com/Stun)
                { PercentMore, Effect.Stun.Duration, 100 / Effect.Stun.Recovery.For(Enemy).Value - 100 },
                {
                    BaseSet, Effect.Stun.Chance,
                    MetaStats.AverageDamage.WithHits, MetaStats.EffectiveStunThreshold.For(Enemy),
                    (damage, threshold)
                        => 200 * damage.Value / (Life.For(Enemy).ValueFor(NodeType.Subtotal) * threshold.Value)
                },
                // flags
                {
                    PercentMore, Damage.WithSkills(DamageSource.Attack).With(Keyword.Projectile),
                    30 * ValueFactory.LinearScale(Projectile.TravelDistance, (35, 0), (70, 1)),
                    Flag.FarShot.IsTrue
                },
                // repeats
                { BaseSet, Stat.SkillRepeats, 0 },
                {
                    PercentMore, Damage,
                    ValueFactory.If(Stat.SkillRepeats.Value.Eq(0)).Then(0)
                        .Else(Stat.DamageMultiplierOverRepeatCycle.Value.AsPercentage / Stat.SkillRepeats.Value)
                },
                // other
                { BaseSet, Stat.SkillNumberOfHitsPerCast, 1 },
                { BaseSet, Stat.MainSkillPart, 0 },
            };

        private static ValueBuilder AverageAilmentDamageFromCritAndNonCrit(
            IStatBuilder nonCritDamage, IStatBuilder critDamage, IStatBuilder critChance,
            IStatBuilder nonCritAilmentChance, IStatBuilder critAilmentChance)
        {
            return CombineByWeightedAverage(
                nonCritDamage.Value.Average, (1 - critChance.Value) * nonCritAilmentChance.Value.AsPercentage,
                critDamage.Value.Average, critChance.Value * critAilmentChance.Value.AsPercentage);
        }

        private ValueBuilder EnemyResistanceAgainstHits(
            DamageType dt, IStatBuilder ignoreResistance, IStatBuilder penetration, IStatBuilder resistanceFromArmour)
        {
            var resistance = DamageTypeBuilders.From(dt).Resistance.For(Enemy).Value - penetration.Value;
            if (dt == DamageType.Physical)
            {
                resistance += resistanceFromArmour.Value;
            }
            return ValueFactory.If(ignoreResistance.IsTrue).Then(0)
                .Else(resistance);
        }

        private ValueBuilder EnemyDamageTakenMultiplier(DamageType resistanceType, IStatBuilder damageTaken)
            => DamageTakenMultiplier(DamageTypeBuilders.From(resistanceType).Resistance.For(Enemy),
                damageTaken.For(Enemy));

        private IDamageRelatedStatBuilder DamageMultiplierWithCrits(DamageType damageType)
            => DamageTypeBuilders.From(damageType).DamageMultiplierWithCrits;

        private IDamageRelatedStatBuilder DamageMultiplierWithNonCrits(DamageType damageType)
            => DamageTypeBuilders.From(damageType).DamageMultiplierWithNonCrits;

        private IValueBuilder EffectiveCriticalStrikeMultiplier(IStatBuilder critMultiStat)
        {
            var critMulti = critMultiStat.Value.AsPercentage;
            return ValueFactory.If(critMulti > 1)
                .Then(1 + (critMulti - 1) * CriticalStrike.ExtraDamageTaken.For(Enemy).Value)
                .Else(critMulti);
        }


        private IReadOnlyList<IIntermediateModifier> CollectionToList(DataDrivenMechanicCollection collection)
        {
            AddDamageWithNonCritsModifiers(collection);
            AddDamageWithCritsModifiers(collection);
            AddAilmentEffectiveDamageMultiplierModifiers(collection);
            AddAilmentSourceDamageTypeModifiers(collection);
            return collection.ToList();
        }

        private void AddAilmentEffectiveDamageMultiplierModifiers(DataDrivenMechanicCollection collection)
        {
            var ailmentsAndTypes = new[]
            {
                (Common.Builders.Effects.Ailment.Ignite, DamageType.Fire),
                (Common.Builders.Effects.Ailment.Bleed, DamageType.Physical),
                (Common.Builders.Effects.Ailment.Poison, DamageType.Chaos),
            };
            foreach (var (ailment, damageType) in ailmentsAndTypes)
            {
                AddEffectiveDamageMultiplierWithNonCritsModifiers(collection, ailment, damageType);
                AddEffectiveDamageMultiplierWithCritsModifiers(collection, ailment, damageType);
            }
        }

        private void AddEffectiveDamageMultiplierWithNonCritsModifiers(
            DataDrivenMechanicCollection collection, Ailment ailment, DamageType damageType)
        {
            var ailmentBuilder = Ailment.From(ailment);
            collection.Add(BaseSet, dt => MetaStats.EffectiveDamageMultiplierWithNonCrits(dt).With(ailmentBuilder),
                _ => DamageTaken(damageType).With(ailmentBuilder),
                _ => DamageMultiplierWithNonCrits(damageType).With(ailmentBuilder),
                (_, damageTaken, damageMulti)
                    => EnemyDamageTakenMultiplier(damageType, damageTaken) * damageMulti.Value.AsPercentage);
        }

        private void AddEffectiveDamageMultiplierWithCritsModifiers(
            DataDrivenMechanicCollection collection, Ailment ailment, DamageType damageType)
        {
            var ailmentBuilder = Ailment.From(ailment);
            collection.Add(BaseSet, dt => MetaStats.EffectiveDamageMultiplierWithCrits(dt).With(ailmentBuilder),
                _ => DamageTaken(damageType).With(ailmentBuilder),
                _ => DamageMultiplierWithCrits(damageType).With(ailmentBuilder),
                (_, damageTaken, damageMulti)
                    => EnemyDamageTakenMultiplier(damageType, damageTaken) * damageMulti.Value.AsPercentage);
        }

        private void AddAilmentSourceDamageTypeModifiers(GivenStatCollection collection)
        {
            foreach (var ailment in Enums.GetValues<Ailment>())
            {
                var ailmentBuilder = Ailment.From(ailment);
                foreach (var damageType in Enums.GetValues<DamageType>())
                {
                    collection.Add(TotalOverride, MetaStats.Damage(damageType).With(ailmentBuilder), 0,
                        ailmentBuilder.Source(DamageTypeBuilders.From(damageType)).IsTrue.Not);
                }
            }
        }

        private void AddDamageWithNonCritsModifiers(GivenStatCollection collection)
        {
            AddDamageWithModifiers(collection, MetaStats.DamageWithNonCrits(), MetaStats.DamageWithNonCrits);
        }

        private void AddDamageWithCritsModifiers(GivenStatCollection collection)
        {
            AddDamageWithModifiers(collection, MetaStats.DamageWithCrits(), MetaStats.DamageWithCrits);
        }

        private void AddDamageWithModifiers(GivenStatCollection collection,
            IDamageRelatedStatBuilder damage, Func<DamageType, IDamageRelatedStatBuilder> damageForType)
        {
            var form = BaseAdd;
            foreach (var type in Enums.GetValues<DamageType>().Except(DamageType.RandomElement))
            {
                var forType = damageForType(type);
                AddForSkillAndAilments(collection, form, damage.With(AttackDamageHand.MainHand),
                    forType.With(AttackDamageHand.MainHand));
                AddForSkillAndAilments(collection, form, damage.With(AttackDamageHand.OffHand),
                    forType.With(AttackDamageHand.OffHand));
                AddForSkillAndAilments(collection, form, damage.With(DamageSource.Spell),
                    forType.With(DamageSource.Spell));
                AddForSkillAndAilments(collection, form, damage.With(DamageSource.Secondary),
                    forType.With(DamageSource.Secondary));
                collection.Add(form, damage.WithSkills(DamageSource.OverTime),
                    forType.WithSkills(DamageSource.OverTime).Value);
            }
        }

        private void AddForSkillAndAilments(GivenStatCollection collection,
            IFormBuilder form, IDamageRelatedStatBuilder stat, IDamageRelatedStatBuilder valueStat)
        {
            collection.Add(form, stat.WithSkills, valueStat.WithSkills.Value);
            foreach (var ailment in Enums.GetValues<Ailment>())
            {
                var ailmentBuilder = Ailment.From(ailment);
                collection.Add(form, stat.With(ailmentBuilder), valueStat.With(ailmentBuilder).Value);
            }
        }

        private ValueBuilder CalculateLuckyCriticalStrikeChance(IStatBuilder critChance)
        {
            var critValue = critChance.Value.AsPercentage;
            return ValueFactory.If(Flag.CriticalStrikeChanceIsLucky.IsTrue)
                .Then(1 - (1 - critValue) * (1 - critValue))
                .Else(critValue);
        }

        private ValueBuilder CombineSource(
            IDamageRelatedStatBuilder statToCombine, Func<IDamageRelatedStatBuilder, IValueBuilder> handCombiner)
            => ValueFactory.If(MetaStats.SkillHitDamageSource.Value.Eq((int) DamageSource.Attack))
                .Then(handCombiner(statToCombine))
                .ElseIf(MetaStats.SkillHitDamageSource.Value.Eq((int) DamageSource.Spell))
                .Then(statToCombine.With(DamageSource.Spell).Value)
                .ElseIf(MetaStats.SkillHitDamageSource.Value.Eq((int) DamageSource.Secondary))
                .Then(statToCombine.With(DamageSource.Secondary).Value)
                .Else(0);

        private ValueBuilder CombineSourceDefaultingToSpell(
            IDamageRelatedStatBuilder statToCombine, Func<IDamageRelatedStatBuilder, IValueBuilder> handCombiner)
            => ValueFactory.If(MetaStats.SkillHitDamageSource.Value.Eq((int) DamageSource.Attack))
                .Then(handCombiner(statToCombine))
                .ElseIf(MetaStats.SkillHitDamageSource.Value.Eq((int) DamageSource.Secondary))
                .Then(statToCombine.With(DamageSource.Secondary).Value)
                .Else(statToCombine.With(DamageSource.Spell).Value);

        private ValueBuilder CombineHandsByAverage(IDamageRelatedStatBuilder statToCombine)
        {
            var mhWeight = SkillUsesHandAsMultiplier(AttackDamageHand.MainHand);
            var ohWeight = SkillUsesHandAsMultiplier(AttackDamageHand.OffHand);
            return CombineByWeightedAverage(
                statToCombine.With(AttackDamageHand.MainHand).Value, mhWeight,
                statToCombine.With(AttackDamageHand.OffHand).Value, ohWeight);
        }

        private Func<IDamageRelatedStatBuilder, ValueBuilder> CombineHandsForAverageAilmentDamage(
            Ailment ailment)
        {
            var ailmentChance = MetaStats.AilmentEffectiveChance(ailment);
            var mhWeight = CalculateAilmentHandWeight(ailmentChance, AttackDamageHand.MainHand);
            var ohWeight = CalculateAilmentHandWeight(ailmentChance, AttackDamageHand.OffHand);
            return statToCombine =>
            {
                var mhDamage = statToCombine.With(AttackDamageHand.MainHand).Value;
                var ohDamage = statToCombine.With(AttackDamageHand.OffHand).Value;
                return CombineByWeightedAverage(
                    mhDamage, ValueFactory.If(mhDamage > 0).Then(mhWeight).Else(0),
                    ohDamage, ValueFactory.If(ohDamage > 0).Then(ohWeight).Else(0));
            };
        }

        private ValueBuilder CalculateAilmentHandWeight(IDamageRelatedStatBuilder ailmentChance, AttackDamageHand hand)
            => ailmentChance.With(hand).Value *
               Stat.ChanceToHit.With(hand).Value.AsPercentage *
               SkillUsesHandAsMultiplier(hand);

        private Func<IDamageRelatedStatBuilder, ValueBuilder> CombineHandsForAilmentEffectiveInstances(
            Ailment ailment)
        {
            var ailmentDamage = MetaStats.AverageDamage.With(Ailment.From(ailment));
            var mhDamage = ailmentDamage.With(AttackDamageHand.MainHand).Value;
            var ohDamage = ailmentDamage.With(AttackDamageHand.OffHand).Value;
            var mhWeight = SkillUsesHandAsMultiplier(AttackDamageHand.MainHand);
            var ohWeight = SkillUsesHandAsMultiplier(AttackDamageHand.OffHand);
            return s => CombineByWeightedAverage(
                s.With(AttackDamageHand.MainHand).Value *
                Stat.ChanceToHit.With(AttackDamageHand.MainHand).Value.AsPercentage,
                ValueFactory.If(mhDamage > 0).Then(mhWeight).Else(0),
                s.With(AttackDamageHand.OffHand).Value *
                Stat.ChanceToHit.With(AttackDamageHand.OffHand).Value.AsPercentage,
                ValueFactory.If(ohDamage > 0).Then(ohWeight).Else(0));
        }

        private ValueBuilder CombineHandsForHitDamage(IDamageRelatedStatBuilder statToCombine)
        {
            var usesMh = SkillUsesHandAsMultiplier(AttackDamageHand.MainHand);
            var usesOh = SkillUsesHandAsMultiplier(AttackDamageHand.OffHand);
            var sumOfHands = statToCombine.With(AttackDamageHand.MainHand).Value * usesMh +
                             statToCombine.With(AttackDamageHand.OffHand).Value * usesOh;
            return ValueFactory.If(MetaStats.SkillDoubleHitsWhenDualWielding.IsTrue)
                .Then(sumOfHands)
                .Else(sumOfHands / (usesMh + usesOh));
        }

        private static ValueBuilder CombineByWeightedAverage(
            ValueBuilder left, ValueBuilder leftWeight, ValueBuilder right, ValueBuilder rightWeight)
            => (left * leftWeight + right * rightWeight) / (leftWeight + rightWeight);

        private ValueBuilder SkillUsesHandAsMultiplier(AttackDamageHand hand)
            => ValueFactory.If(MetaStats.SkillUsesHand(hand).IsTrue).Then(1).Else(0);
    }
}