﻿using System.Collections.Generic;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Modifiers;
using PoESkillTree.Computation.Common.Builders.Resolving;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.Computation.Common.Data;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;

namespace PoESkillTree.Computation.Data
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="IStatMatchers"/> implementation matching stat parts specifying converters to the modifier's stats.
    /// </summary>
    public class StatManipulatorMatchers : StatMatchersBase
    {
        private readonly IModifierBuilder _modifierBuilder;

        public StatManipulatorMatchers(
            IBuilderFactories builderFactories, IMatchContexts matchContexts, IModifierBuilder modifierBuilder)
            : base(builderFactories, matchContexts)
        {
            _modifierBuilder = modifierBuilder;
        }

        protected override IEnumerable<MatcherData> CreateCollection() =>
            new StatManipulatorMatcherCollection(_modifierBuilder)
            {
                { "you and nearby allies( deal| have)?", s => Buff.Aura(s, Self, Ally) },
                {
                    "auras you cast grant (?<inner>.*) to you and allies",
                    s => s.AddTo(Skills[Keyword.Aura]), "${inner}"
                },
                {
                    "consecrated ground you create grants (?<inner>.*) to you and allies",
                    s => s.AddTo(Ground.Consecrated), "${inner}"
                },
                {
                    "every # seconds, gain (?<inner>.*) for # seconds",
                    s => Buff.Temporary(Values.First, Values.Last, s), "${inner}"
                },
                { "nearby enemies (have|deal)", s => Buff.Aura(s, Enemy) },
                { "nearby enemies take", (IDamageStatBuilder s) => Buff.Aura(s.Taken, Enemy) },
                { "enemies near your totems (have|deal)", s => Entity.Totem.Stat(Buff.Aura(s, Enemy)) },
                {
                    "enemies near your totems take",
                    (IDamageStatBuilder s) => Entity.Totem.Stat(Buff.Aura(s.Taken, Enemy))
                },
                // Keep whole mod line, take is part of the condition matcher
                { "enemies .+ take", (IDamageStatBuilder s) => s.Taken, "$0" },
                { "for # seconds", s => s.WithCondition(Action.InPastXSeconds(Value)) },
            };
    }
}