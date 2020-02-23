using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Base;
using PoESkillTree.Engine.Computation.Data.Collections;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Data.GivenStats
{
    /// <summary>
    /// Additional modifiers that are required for skills to work as intended and can't be added through
    /// <see cref="GameModel.Skills.SkillDefinitionExtensions"/>.
    /// </summary>
    public class AdditionalSkillStats : UsesConditionBuilders, IGivenStats
    {
        private readonly IModifierBuilder _modifierBuilder;
        private readonly Lazy<IReadOnlyList<IIntermediateModifier>> _lazyGivenStats;

        public AdditionalSkillStats(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories)
        {
            _modifierBuilder = modifierBuilder;
            _lazyGivenStats = new Lazy<IReadOnlyList<IIntermediateModifier>>(() => CreateCollection().ToList());
        }

        private IMetaStatBuilders MetaStats => BuilderFactories.MetaStatBuilders;

        public IReadOnlyList<Entity> AffectedEntities { get; } = new[] { GameModel.Entity.Character };

        public IReadOnlyList<string> GivenStatLines { get; } = new string[0];

        public IReadOnlyList<IIntermediateModifier> GivenModifiers => _lazyGivenStats.Value;

        private GivenStatCollection CreateCollection() => new GivenStatCollection(_modifierBuilder, ValueFactory)
        {
            { TotalOverride, Skills.FromId("ArcticArmour").Buff.EffectOn(Self), 0, Flag.AlwaysStationary.Not },

            {
                TotalOverride, Stat.SkillNumberOfHitsPerCast, Projectile.Count.Value,
                IsMainSkill("BlastRain", 1)
            },

            {
                TotalOverride, Buff.Blind.On(OpponentOfSelf), true,
                And(Skills.FromId("BloodSandArmour").Buff.IsOn(OpponentOfSelf), Flag.InSandStance)
            },
            {
                TotalOverride, Buff.Maim.On(OpponentOfSelf), true,
                And(Skills.FromId("BloodSandArmour").Buff.IsOn(OpponentOfSelf), Flag.InBloodStance)
            },

            { TotalOverride, Skills.FromId("BurningArrow").Buff.StackCount.For(OpponentOfSelf).Maximum, 5 },

            { TotalOverride, Skills[Keyword.Banner].Reservation, 0, Flag.IsBannerPlanted },

            { TotalOverride, Fire.Invert.Damage, 0, IsMainSkill("ElementalHit", 0) },
            { TotalOverride, Cold.Invert.Damage, 0, IsMainSkill("ElementalHit", 1) },
            { TotalOverride, Lightning.Invert.Damage, 0, IsMainSkill("ElementalHit", 2) },

            { TotalOverride, Skills.FromId("FireBeam").Buff.On(OpponentOfSelf), 1, SkillIsActive("FireBeam") },
            {
                BaseSet, Fire.Exposure.For(OpponentOfSelf), -25,
                SkillIsActive("FireBeam").And(Skills.FromId("FireBeam").Buff.StackCount.For(OpponentOfSelf).Value
                                              >= Skills.FromId("FireBeam").Buff.StackCount.For(OpponentOfSelf).Maximum.Value)
            },

            {
                // Freezing Pulse's damage dissipates while traveling
                // 60 * Projectile.Speed is the range, Projectile.TravelDistance / range is the percentage traveled
                PercentLess, Damage,
                ValueFactory.LinearScale(Projectile.TravelDistance / (60 * Projectile.Speed.Value),
                    (0, 0), (1, 50)),
                IsMainSkill("FreezingPulse")
            },
            {
                // Freezing Pulse's additional chance to freeze dissipates while traveling
                BaseAdd, Ailment.Freeze.Chance,
                ValueFactory.LinearScale(Projectile.TravelDistance / (60 * Projectile.Speed.Value),
                    (0, 25), (0.25, 0)),
                IsMainSkill("FreezingPulse")
            },

            {
                TotalOverride, Stat.SkillNumberOfHitsPerCast, Projectile.Count.Value,
                IsMainSkill("IceSpear", 1)
            },
            {
                TotalOverride, Stat.SkillNumberOfHitsPerCast, Projectile.Count.Value,
                IsMainSkill("IceSpear", 3)
            },

            {
                // Reduce cast rate proportional to the time spent channeling
                PercentLess, Stat.CastRate,
                100 * (Stat.SkillStage.Maximum.Value - Stat.SkillStage.Value + 1) / Stat.SkillStage.Maximum.Value,
                IsMainSkill("ScourgeArrow").And(Stat.SkillStage.Value > 0)
            },

            {
                TotalOverride, Stat.SkillNumberOfHitsPerCast, Projectile.Count.Value,
                IsMainSkill("ShatteringSteel", 2)
            },

            { TotalOverride, Skills.FromId("Slither").Buff.AddStatForSource(Buff.Elusive.On(Self), Self), true },

            { TotalOverride, Buff.ArcaneSurge.On(Self), 1, SkillIsActive("SupportArcaneSurge") },

            { TotalOverride, Buff.Innervation.On(Self), 1, SkillIsActive("SupportOnslaughtOnSlayingShockedEnemy") },

            {
                // The reduction to ExpirationModifier is already built into Temporal Chains' duration.
                // Calculate the duration without it so it can be applied again, but while being affected by Curse Effect modifiers.
                // Without Curse Effect, ExpirationModifier will be 0.6, which is a 1 / 0.6 multiplier to Duration.
                // 40 PercentLess is a 0.6 modifier, negating that.
                PercentLess, Skills.FromId("TemporalChains").Buff.Duration, 40
            },

            { TotalOverride, Buff.Withered.On(OpponentOfSelf), 1, SkillIsActive("Wither") },
        };

        private IConditionBuilder IsMainSkill(string skillId, int skillPart)
            => IsMainSkill(skillId).And(Stat.MainSkillPart.Value.Eq(skillPart));

        private IConditionBuilder IsMainSkill(string skillId)
            => MetaStats.MainSkillId.Value.Eq(Skills.FromId(skillId).SkillId);

        private IConditionBuilder SkillIsActive(string skillId)
            => MetaStats.ActiveSkillItemSlot(skillId).IsSet;
    }
}