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

namespace PoESkillTree.Engine.Computation.Data.GivenStats
{
    public class CommonMechanics : DataDrivenMechanicsBase
    {
        private readonly Lazy<IReadOnlyList<IIntermediateModifier>> _lazyGivenStats;

        public CommonMechanics(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories, modifierBuilder)
        {
            _lazyGivenStats = new Lazy<IReadOnlyList<IIntermediateModifier>>(
                () => CreateCollection().ToList());
        }

        public override IReadOnlyList<Entity> AffectedEntities { get; } = Enums.GetValues<Entity>().ToList();

        public override IReadOnlyList<IIntermediateModifier> GivenModifiers => _lazyGivenStats.Value;

        private DataDrivenMechanicCollection CreateCollection()
            => new DataDrivenMechanicCollection(ModifierBuilder, BuilderFactories)
            {
                // speed
                { PercentMore, Stat.MovementSpeed, ActionSpeedValueForPercentMore },
                {
                    PercentMore, Stat.CastRate, ActionSpeedValueForPercentMore,
                    Not(Or(With(Keyword.Totem), With(Keyword.Trap), With(Keyword.Mine)))
                },
                { PercentMore, Stat.Totem.Speed, ActionSpeedValueForPercentMore },
                { PercentMore, Stat.Trap.Speed, ActionSpeedValueForPercentMore },
                { PercentMore, Stat.Mine.Speed, ActionSpeedValueForPercentMore },
                // resistances/damage reduction
                {
                    BaseAdd, dt => DamageTypeBuilders.From(dt).Resistance,
                    dt => DamageTypeBuilders.From(dt).Exposure.Value
                },
                // ailments
                {
                    TotalOverride, MetaStats.AilmentDealtDamageType(Common.Builders.Effects.Ailment.Ignite),
                    (int) DamageType.Fire
                },
                {
                    TotalOverride, MetaStats.AilmentDealtDamageType(Common.Builders.Effects.Ailment.Bleed),
                    (int) DamageType.Physical
                },
                {
                    TotalOverride, MetaStats.AilmentDealtDamageType(Common.Builders.Effects.Ailment.Poison),
                    (int) DamageType.Chaos
                },
                { TotalOverride, Ailment.Chill.On(Self), 1, Ailment.Freeze.IsOn(Self) },
                {
                    PercentIncrease, Ailment.Shock.AddStat(Damage.Taken), MetaStats.IncreasedDamageTakenFromShocks.Value
                },
                { TotalOverride, MetaStats.IncreasedDamageTakenFromShocks.Maximum, 50 },
                { TotalOverride, MetaStats.IncreasedDamageTakenFromShocks.Minimum, 1 },
                {
                    PercentReduce, Ailment.Chill.AddStat(Stat.ActionSpeed),
                    MetaStats.ReducedActionSpeedFromChill.Value
                },
                { TotalOverride, MetaStats.ReducedActionSpeedFromChill.Maximum, 30 },
                { TotalOverride, MetaStats.ReducedActionSpeedFromChill.Minimum, 1 },
                { BaseSet, a => Ailment.From(a).TickRateModifier, a => ValueFactory.Create(1) },
                { PercentMore, a => Ailment.From(a).Duration, a => 100 / Ailment.From(a).TickRateModifier.Value },
                // stun (see https://pathofexile.gamepedia.com/Stun)
                {
                    TotalOverride, MetaStats.EffectiveStunThreshold,
                    Effect.Stun.Threshold, EffectiveStunThresholdValue
                },
                // other
                { PercentMore, Stat.Radius, Stat.AreaOfEffect.Value.Select(Math.Sqrt, v => $"Sqrt({v})") },
                { PercentMore, Stat.Cooldown, 100 - 100 * Stat.CooldownRecoverySpeed.Value.Invert },
            };

        private ValueBuilder ActionSpeedValueForPercentMore => (Stat.ActionSpeed.Value - 1) * 100;

        private IValueBuilder EffectiveStunThresholdValue(IStatBuilder stunThresholdStat)
        {
            // If stun threshold is less than 25%, it is scaled up.
            // See https://pathofexile.gamepedia.com/Stun#Stun_threshold
            var stunThreshold = stunThresholdStat.Value;
            return ValueFactory
                .If(stunThreshold >= 0.25).Then(stunThreshold)
                .Else(0.25 - 0.25 * (0.25 - stunThreshold) / (0.5 - stunThreshold));
        }
    }
}