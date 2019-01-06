﻿using System;
using System.Collections.Generic;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Modifiers;
using PoESkillTree.Computation.Common.Builders.Values;
using PoESkillTree.Computation.Common.Data;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;
using PoESkillTree.GameModel.Items;
using static PoESkillTree.Computation.Common.Builders.Values.ValueBuilderUtils;

namespace PoESkillTree.Computation.Data
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="IStatMatchers"/> implementation matching stat parts specifying converters to the modifier's
    /// main value (at the moment, these are all multipliers).
    /// </summary>
    public class ValueConversionMatchers : StatMatchersBase
    {
        private readonly IModifierBuilder _modifierBuilder;

        public ValueConversionMatchers(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories)
        {
            _modifierBuilder = modifierBuilder;
        }

        protected override IEnumerable<MatcherData> CreateCollection() =>
            new ValueConversionMatcherCollection(_modifierBuilder)
            {
                // action
                { "for each enemy you've killed recently", Kill.CountRecently },
                {
                    "per enemy killed by you or your totems recently",
                    Kill.CountRecently + Kill.By(Entity.Totem).CountRecently
                },
                {
                    "for each enemy you or your minions have killed recently, up to #%",
                    CappedMultiplier(Kill.CountRecently + Kill.By(Entity.Minion).CountRecently, Value)
                },
                { "for each hit you've blocked recently", Block.CountRecently },
                { "for each corpse consumed recently", Action.ConsumeCorpse.CountRecently },
                // equipment
                { "for each magic item you have equipped", Equipment.Count(e => e.Has(FrameType.Magic)) },
                // stats
                { "per # ({StatMatchers})", PerStat(stat: Reference.AsStat, divideBy: Value) },
                { "per # ({StatMatchers}) ceiled", PerStatCeiled(stat: Reference.AsStat, divideBy: Value) },
                { "per ({StatMatchers})", PerStat(stat: Reference.AsStat) },
                {
                    "per # ({StatMatchers}) on helmet",
                    PerStat(Reference.AsStat.ValueFor(NodeType.Base, new ModifierSource.Local.Item(ItemSlot.Helm)),
                        divideBy: Value)
                },
                {
                    "per # ({StatMatchers}) on body armour",
                    PerStat(
                        Reference.AsStat.ValueFor(NodeType.Base, new ModifierSource.Local.Item(ItemSlot.BodyArmour)),
                        divideBy: Value)
                },
                { "per grand spectrum", PerStat(stat: Stat.GrandSpectrumJewelsSocketed) },
                { "per level", PerStat(Stat.Level) },
                { "per (stage|fuse charge)", PerStat(Stat.SkillStage) },
                { "for each (stage|blade)", PerStat(Stat.SkillStage) },
                { @"per stage, up to \+#", CappedMultiplier(Stat.SkillStage.Value, Value) },
                {
                    "per ({ChargeTypeMatchers}) removed",
                    v => v * (Reference.AsChargeType.Amount.Value - Reference.AsChargeType.Amount.Minimum.Value)
                },
                // buffs
                { "per buff on you", Buffs(targets: Self).Count() },
                { "per curse on you", Buffs(targets: Self).With(Keyword.Curse).Count() },
                { "for each curse on that enemy,", Buffs(targets: Enemy).With(Keyword.Curse).Count() },
                { "for each impale on enemy", Buff.Impale.StackCount.For(Enemy).Value },
                // ailments
                { "for each poison on the enemy", Ailment.Poison.InstancesOn(Enemy).Value },
                { "per poison on enemy", Ailment.Poison.InstancesOn(Enemy).Value },
                {
                    @"per poison affecting enemy, up to \+#%",
                    CappedMultiplier(Ailment.Poison.InstancesOn(Enemy).Value, Value)
                },
                {
                    "for each poison on the enemy, up to #",
                    CappedMultiplier(Ailment.Poison.InstancesOn(Enemy).Value, Value)
                },
                { "per elemental ailment on the enemy", Ailment.Elemental.Count(b => b.IsOn(Enemy)) },
                // skills
                { "for each zombie you own", Skills.RaiseZombie.Instances.Value },
                { "for each summoned golem", Golems.CombinedInstances.Value },
                { "for each golem you have summoned", Golems.CombinedInstances.Value },
                { "for each type of golem you have summoned", Golems.CombinedInstances.Value },
                {
                    "for each skill you've used Recently, up to #%",
                    CappedMultiplier(AllSkills.Cast.CountRecently, Value)
                },
                // traps, mines, totems
                { "for each trap", Traps.CombinedInstances.Value },
                { "for each mine", Mines.CombinedInstances.Value },
                { "for each trap and mine you have", Traps.CombinedInstances.Value + Mines.CombinedInstances.Value },
                { "per totem", Totems.CombinedInstances.Value },
                // unique
                {
                    "for each poison you have inflicted recently",
                    Stat.UniqueInt("# of Poisons inflicted Recently", 0)
                },
                {
                    "for each remaining chain",
                    Projectile.ChainCount.Value -
                    Stat.UniqueInt("# of times the Active Skill has Chained", 0)
                },
                {
                    "for each of your mines detonated recently, up to #%",
                    CappedMultiplier(Stat.UniqueInt("# of Mines Detonated Recently", 0), Value)
                },
                {
                    "for each of your traps triggered recently, up to #%",
                    CappedMultiplier(Stat.UniqueInt("# of Traps Triggered Recently", 0), Value)
                },
                {
                    "for each time you've blocked in the past 10 seconds",
                    Stat.UniqueInt("# of times blocked in the past 10 seconds", 0)
                },
                {
                    "per one hundred nearby enemies",
                    Stat.UniqueInt("# of nearby enemies", 0) / 100
                },
            }; // add

        private Func<ValueBuilder, ValueBuilder> CappedMultiplier(ValueBuilder multiplier, ValueBuilder maximum)
        {
            return v => ValueFactory.Minimum(v * multiplier, maximum);
        }
    }
}