using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
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
    /// <see cref="IGivenStats"/> implementation that provides the stats applied when effects are active.
    /// </summary>
    public class EffectStats : UsesStatBuilders, IGivenStats
    {
        private readonly IModifierBuilder _modifierBuilder;
        private readonly Lazy<IReadOnlyList<IIntermediateModifier>> _lazyGivenStats;

        public EffectStats(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories)
        {
            _modifierBuilder = modifierBuilder;
            _lazyGivenStats = new Lazy<IReadOnlyList<IIntermediateModifier>>(() => CreateCollection().ToList());
        }

        public IReadOnlyList<Entity> AffectedEntities { get; } = Enums.GetValues<Entity>().ToList();

        public IReadOnlyList<string> GivenStatLines { get; } = new string[0];

        public IReadOnlyList<IIntermediateModifier> GivenModifiers => _lazyGivenStats.Value;

        private EffectStatCollection CreateCollection() => new EffectStatCollection(_modifierBuilder, ValueFactory)
        {
            // ailments
            { Ailment.Freeze, PercentLess, Stat.ActionSpeed, 100 },
            // buffs
            { Buff.Fortify, PercentLess, Damage.Taken.WithHits, 20 },
            { Buff.Maim, PercentReduce, Stat.MovementSpeed, 30 },
            { Buff.Intimidate, PercentIncrease, Damage.Taken, 10 },
            { Buff.Onslaught, PercentIncrease, Stat.CastRate, 20 },
            { Buff.Onslaught, PercentIncrease, Stat.MovementSpeed, 20 },
            {
                Buff.UnholyMight,
                BaseAdd, Physical.Damage.WithHitsAndAilments.GainAs(Chaos.Damage.WithHitsAndAilments), 30
            },
            { Buff.ArcaneSurge, PercentMore, Damage.WithSkills(DamageSource.Spell), 10 },
            { Buff.ArcaneSurge, PercentIncrease, Stat.CastRate.With(DamageSource.Spell), 10 },
            { Buff.ArcaneSurge, BaseAdd, Mana.Regen.Percent, 0.5 },
            { Buff.Tailwind, PercentIncrease, Stat.ActionSpeed, 10 },
            { Buff.CoveredInAsh, PercentLess, Stat.MovementSpeed, 20 },
            { Buff.CoveredInAsh, PercentIncrease, Fire.Damage.Taken, 20 },
            { Buff.Conflux.Igniting, BaseSet, Ailment.Ignite.Source(AnyDamageType), 1 },
            { Buff.Conflux.Shocking, BaseSet, Ailment.Shock.Source(AnyDamageType), 1 },
            { Buff.Conflux.Chilling, BaseSet, Ailment.Chill.Source(AnyDamageType), 1 },
            { Buff.Conflux.Elemental, BaseSet, Ailment.Ignite.Source(AnyDamageType), 1 },
            { Buff.Conflux.Elemental, BaseSet, Ailment.Shock.Source(AnyDamageType), 1 },
            { Buff.Conflux.Elemental, BaseSet, Ailment.Chill.Source(AnyDamageType), 1 },
            { Buff.Rampage, PercentIncrease, Stat.MovementSpeed, (Buff.Rampage.StackCount.Value / 10).Floor() },
            { Buff.Rampage, PercentIncrease, Damage, (2 * Buff.Rampage.StackCount.Value / 10).Floor() },
            { Buff.Rampage, PercentIncrease, Stat.MovementSpeed.For(Entity.Minion), (Buff.Rampage.StackCount.Value / 10).Floor() },
            { Buff.Rampage, PercentIncrease, Damage.For(Entity.Minion), (2 * Buff.Rampage.StackCount.Value / 10).Floor() },
            { Buff.Withered, PercentIncrease, Chaos.Damage.Taken, 6 * Buff.Withered.StackCount.Value },
            { Buff.Elusive, BaseAdd, Stat.Dodge.AttackChance, 15 },
            { Buff.Elusive, BaseAdd, Stat.Dodge.SpellChance, 15 },
            { Buff.Elusive, PercentIncrease, Stat.MovementSpeed, 30 },
        };
    }
}