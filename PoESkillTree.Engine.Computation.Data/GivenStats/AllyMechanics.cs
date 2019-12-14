using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.Computation.Data.Collections;
using PoESkillTree.Engine.GameModel;

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
                { BaseSet, MetaStats.ResistanceAgainstHits(DamageType.Physical), Physical.Resistance.Value },
                { BaseSet, MetaStats.ResistanceAgainstHits(DamageType.Physical).Maximum, 90 },
                { TotalOverride, MetaStats.ResistanceAgainstHits(DamageType.Lightning), Lightning.Resistance.Value },
                { TotalOverride, MetaStats.ResistanceAgainstHits(DamageType.Cold), Cold.Resistance.Value },
                { TotalOverride, MetaStats.ResistanceAgainstHits(DamageType.Fire), Fire.Resistance.Value },
                { TotalOverride, MetaStats.ResistanceAgainstHits(DamageType.Chaos), Chaos.Resistance.Value },
                {
                    BaseAdd, MetaStats.ResistanceAgainstHits(DamageType.Physical),
                    PhysicalDamageReductionFromArmour(Armour.Value,
                        Physical.Damage.WithSkills.With(AttackDamageHand.MainHand).For(Enemy).Value)
                },
                // damage mitigation (1 - (1 - resistance / 100) * damage taken)
                {
                    TotalOverride, MetaStats.MitigationAgainstHits,
                    dt => 1 - DamageTakenMultiplier(MetaStats.ResistanceAgainstHits(dt),
                              DamageTaken(dt).WithSkills(DamageSource.Secondary))
                },
                {
                    TotalOverride, MetaStats.MitigationAgainstDoTs,
                    dt => 1 - DamageTakenMultiplier(DamageTypeBuilders.From(dt).Resistance,
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
                { TotalOverride, MetaStats.EffectiveRecharge, p => p.Recharge.Value * p.RecoveryRate.Value },
                { TotalOverride, MetaStats.RechargeStartDelay, p => 2 / p.Recharge.Start.Value },
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