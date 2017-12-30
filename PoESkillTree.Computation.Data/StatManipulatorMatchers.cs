﻿using System.Collections.Generic;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;
using PoESkillTree.Computation.Parsing.Builders;
using PoESkillTree.Computation.Parsing.Builders.Matching;
using PoESkillTree.Computation.Parsing.Builders.Stats;
using PoESkillTree.Computation.Parsing.Data;
using PoESkillTree.Computation.Parsing.ModifierBuilding;

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
                { "you and nearby allies( deal| have)?", s => s.AsAura(Self, Ally) },
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
                    s => Buff.Rotation(Values.First).Step(Values.Last, s.AsBuff), "${inner}"
                },
                { "nearby enemies (have|deal)", s => s.AsAura(Enemy) },
                { "nearby enemies take", (IDamageStatBuilder s) => s.Taken.AsAura(Enemy) },
                { "enemies near your totems (have|deal)", s => Entity.Totem.Stat(s.AsAura(Enemy)) },
                {
                    "enemies near your totems take",
                    (IDamageStatBuilder s) => Entity.Totem.Stat(s.Taken.AsAura(Enemy))
                },
                // Keep whole mod line, take is part of the condition matcher
                { "enemies .+ take", (IDamageStatBuilder s) => s.Taken, "$0" },
                {
                    "(?<inner>chance to .*) for # seconds",
                    s => s.ForXSeconds(Value).ChanceOn(Self), "${inner}"
                },
                { "for # seconds", s => s.ForXSeconds(Value).On(Self) },
            };
    }
}