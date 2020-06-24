using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.Computation.Data.Collections;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.Utils.Extensions;

namespace PoESkillTree.Engine.Computation.Data.GivenStats
{
    public class AllyMechanics : DataDrivenMechanicsBase
    {
        private readonly Lazy<IReadOnlyList<IIntermediateModifier>> _lazyGivenStats;

        public AllyMechanics(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories, modifierBuilder)
        {
            _lazyGivenStats = new Lazy<IReadOnlyList<IIntermediateModifier>>(
                () => CreateCollection().ToList());
        }

        public override IReadOnlyList<Entity> AffectedEntities { get; } = new[]
        {
            GameModel.Entity.Character, GameModel.Entity.Totem, GameModel.Entity.Minion,
        };

        public override IReadOnlyList<IIntermediateModifier> GivenModifiers => _lazyGivenStats.Value;

        private DataDrivenMechanicCollection CreateCollection()
            => new DataDrivenMechanicCollection(ModifierBuilder, BuilderFactories)
            {
                // resistances/damage reduction
                {
                    BaseSet, dt => DamageTypeBuilders.From(dt).ResistanceAgainstHits,
                    dt => DamageTypeBuilders.From(dt).Resistance.Value
                },
                {
                    BaseSet, dt => DamageTypeBuilders.From(dt).ResistanceAgainstDoTs,
                    dt => DamageTypeBuilders.From(dt).Resistance.Value
                },
                // damage reduction
                {
                    BaseSet, dt => DamageTypeBuilders.From(dt).DamageReductionIncludingArmour,
                    dt => DamageTypeBuilders.From(dt).DamageReduction.Value
                },
                {
                    BaseAdd, Physical.DamageReductionIncludingArmour,
                    PhysicalDamageReductionFromArmour(Armour,
                        Physical.Damage.WithSkills.With(AttackDamageHand.MainHand).For(Enemy).Value)
                },
                // damage mitigation (resistance, damage reduction, damage taken)
                {
                    TotalOverride, MetaStats.MitigationAgainstHits,
                    dt => 1 - DamageTakenMultiplier(
                        DamageTypeBuilders.From(dt).ResistanceAgainstHits,
                        DamageTypeBuilders.From(dt).DamageReductionIncludingArmour,
                        DamageTaken(dt).WithSkills(DamageSource.Secondary))
                },
                {
                    TotalOverride, MetaStats.MitigationAgainstDoTs,
                    dt => 1 - DamageTakenMultiplier(
                        DamageTypeBuilders.From(dt).ResistanceAgainstDoTs,
                        DamageTypeBuilders.From(dt).DamageReduction,
                        DamageTaken(dt).WithSkills(DamageSource.OverTime))
                },
                // chance to evade
                {
                    BaseSet, Evasion.Chance,
                    100 - ChanceToHitValue(Stat.Accuracy.With(AttackDamageHand.MainHand).For(Enemy), Evasion,
                        Buff.Blind.IsOn(Enemy))
                },
                // chance to avoid
                {
                    TotalOverride, MetaStats.ChanceToAvoidMeleeAttacks,
                    100 - 100 * (FailureProbability(Evasion.ChanceAgainstMeleeAttacks) *
                                 FailureProbability(Stat.Dodge.AttackChance) * FailureProbability(Block.AttackChance))
                },
                {
                    TotalOverride, MetaStats.ChanceToAvoidProjectileAttacks,
                    100 - 100 * (FailureProbability(Evasion.ChanceAgainstProjectileAttacks) *
                                 FailureProbability(Stat.Dodge.AttackChance) * FailureProbability(Block.AttackChance))
                },
                {
                    TotalOverride, MetaStats.ChanceToAvoidSpells,
                    100 - 100 * (FailureProbability(Stat.Dodge.SpellChance) * FailureProbability(Block.SpellChance))
                },
                // pools
                {
                    BaseAdd, p => Stat.Pool.From(p).Regen,
                    p => MetaStats.RegenTargetPoolValue(p) * Stat.Pool.From(p).Regen.Percent.Value.AsPercentage
                },
                { TotalOverride, MetaStats.EffectiveRegen, p => p.Regen.Value * p.RecoveryRate.Value },
                {
                    TotalOverride, MetaStats.EffectiveDegeneration,
                    p => Enums.GetValues<DamageType>().Except(DamageType.RandomElement)
                        .Select(dt => MetaStats.EffectiveDegeneration(p, dt).Value)
                        .Aggregate((v1, v2) => v1 + v2)
                },
                { TotalOverride, MetaStats.NetRegen, p => MetaStats.EffectiveRegen(p).Value - MetaStats.EffectiveDegeneration(p).Value },
                { TotalOverride, MetaStats.EffectiveRecharge, p => p.Recharge.Value * p.RecoveryRate.Value },
                { TotalOverride, MetaStats.RechargeStartDelay, p => 2 / p.Recharge.Start.Value },
                {
                    TotalOverride, MetaStats.EffectiveDegeneration,
                    (p, dt) => p.Degeneration(DamageTypeBuilders.From(dt)).Value * MetaStats.MitigationAgainstDoTs(dt).Value
                },
                // ailments
                { PercentMore, a => Ailment.From(a).Duration, a => 100 / Effect.ExpirationModifier.For(Enemy).Value - 100 },
                // stun (see https://pathofexile.gamepedia.com/Stun)
                {
                    TotalOverride, MetaStats.StunAvoidanceWhileCasting,
                    1 - (1 - Effect.Stun.Avoidance.Value) * (1 - Effect.Stun.ChanceToAvoidInterruptionWhileCasting.Value)
                },
            };

        private static ValueBuilder FailureProbability(IStatBuilder percentageChanceStat)
            => 1 - percentageChanceStat.Value.AsPercentage;
    }
}