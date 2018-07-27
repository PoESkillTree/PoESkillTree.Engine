﻿using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Damage;
using PoESkillTree.Computation.Common.Builders.Effects;
using PoESkillTree.Computation.Common.Builders.Modifiers;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.Computation.Common.Builders.Values;
using PoESkillTree.Computation.Common.Data;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;

namespace PoESkillTree.Computation.Data.GivenStats
{
    public class DataDrivenMechanics : UsesStatBuilders, IGivenStats
    {
        private readonly IModifierBuilder _modifierBuilder;
        private readonly IMetaStatBuilders _stat;
        private readonly Lazy<IReadOnlyList<IIntermediateModifier>> _lazyGivenStats;

        public DataDrivenMechanics(
            IBuilderFactories builderFactories, IModifierBuilder modifierBuilder, IMetaStatBuilders metaStatBuilders)
            : base(builderFactories)
        {
            _modifierBuilder = modifierBuilder;
            _stat = metaStatBuilders;
            _lazyGivenStats = new Lazy<IReadOnlyList<IIntermediateModifier>>(() => CreateCollection().ToList());
        }

        public IReadOnlyList<Entity> AffectedEntities { get; } = Enums.GetValues<Entity>().ToList();

        public IReadOnlyList<string> GivenStatLines { get; } = new string[0];

        public IReadOnlyList<IIntermediateModifier> GivenModifiers => _lazyGivenStats.Value;

        private IEnumerable<IIntermediateModifier> CreateCollection()
            => new DataDrivenMechanicCollection(_modifierBuilder, ValueFactory)
            {
                // pools
                { BaseAdd, Life.Regen, RegenFromPercentValue(Life) },
                { BaseAdd, Mana.Regen, RegenFromPercentValue(Mana) },
                { BaseAdd, EnergyShield.Regen, RegenFromPercentValue(EnergyShield) },
                // ailments
                {
                    TotalOverride, _stat.AilmentDealtDamageType(Common.Builders.Effects.Ailment.Ignite),
                    (int) DamageType.Fire
                },
                {
                    TotalOverride, _stat.AilmentDealtDamageType(Common.Builders.Effects.Ailment.Bleed),
                    (int) DamageType.Physical
                },
                {
                    TotalOverride, _stat.AilmentDealtDamageType(Common.Builders.Effects.Ailment.Ignite),
                    (int) DamageType.Chaos
                },
                // stun (see https://pathofexile.gamepedia.com/Stun)
                { PercentLess, Effect.Stun.Duration, Effect.Stun.Recovery.For(Enemy).Value * 100 },
                {
                    TotalOverride, _stat.EffectiveStunThreshold,
                    Effect.Stun.Threshold, EffectiveStunThresholdValue
                },
                {
                    BaseSet, Effect.Stun.Chance,
                    _stat.AverageEffectiveDamage, _stat.EffectiveStunThreshold, StunChanceValue
                },
                {
                    TotalOverride, _stat.StunAvoidanceWhileCasting,
                    1 -
                    (1 - Effect.Stun.Avoidance.Value) * (1 - Effect.Stun.ChanceToAvoidInterruptionWhileCasting.Value)
                },
            }.Concat(CreateAilmentSourceDamageTypeModifiers());

        private IValueBuilder RegenFromPercentValue(IPoolStatBuilder pool)
            => _stat.RegenTargetPoolValue(pool) * pool.Regen.Percent.Value / 100;

        private IEnumerable<IIntermediateModifier> CreateAilmentSourceDamageTypeModifiers()
        {
            foreach (var ailment in Enums.GetValues<Ailment>())
            {
                var ailmentBuilder = Ailment.From(ailment);
                foreach (var damageType in Enums.GetValues<DamageType>())
                {
                    var damageTypeBuilder = DamageTypeBuilders.From(damageType);
                    var builder = _modifierBuilder
                        .WithForm(TotalOverride)
                        .WithStat(damageTypeBuilder.Damage.With(ailmentBuilder))
                        .WithValue(ValueFactory.Create(0))
                        .WithCondition(ailmentBuilder.Source(damageTypeBuilder).IsSet.Not);
                    yield return builder.Build();
                }
            }
        }

        private IValueBuilder EffectiveStunThresholdValue(IStatBuilder stunThresholdStat)
        {
            // If stun threshold is less than 25%, it is scaled up.
            // See https://pathofexile.gamepedia.com/Stun#Stun_threshold
            var stunThreshold = stunThresholdStat.Value;
            return ValueFactory
                .If(stunThreshold >= 0.25).Then(stunThreshold)
                .Else(0.25 - 0.25 * (0.25 - stunThreshold) / (0.5 - stunThreshold));
        }

        private IValueBuilder StunChanceValue(IStatBuilder damageStat, IStatBuilder stunThresholdStat)
            => 200 * damageStat.Value /
               (Life.For(Enemy).ValueFor(NodeType.Subtotal) * stunThresholdStat.Value);
    }
}