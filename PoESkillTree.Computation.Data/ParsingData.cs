﻿using System;
using System.Collections.Generic;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Modifiers;
using PoESkillTree.Computation.Common.Builders.Resolving;
using PoESkillTree.Computation.Common.Data;
using PoESkillTree.Computation.Data.Steps;
using PoESkillTree.GameModel.PassiveTree;
using PoESkillTree.GameModel.Skills;

namespace PoESkillTree.Computation.Data
{
    /// <summary>
    /// Implementation of <see cref="IParsingData{T}"/> using <see cref="Stepper"/> and the matcher implementations in
    /// this namespace.
    /// </summary>
    public class ParsingData : IParsingData<ParsingStep>
    {
        private readonly IBuilderFactories _builderFactories;
        private readonly IMatchContexts _matchContexts;

        private readonly Lazy<IReadOnlyList<IStatMatchers>> _statMatchers;

        private readonly Lazy<IReadOnlyList<IReferencedMatchers>> _referencedMatchers;

        private readonly Lazy<IReadOnlyList<StatReplacerData>> _statReplacers =
            new Lazy<IReadOnlyList<StatReplacerData>>(() => new StatReplacers().Replacers);

        private readonly Lazy<IStepper<ParsingStep>> _stepper =
            new Lazy<IStepper<ParsingStep>>(() => new Stepper());

        private readonly Lazy<StatMatchersSelector> _statMatchersSelector;

        public ParsingData(
            IBuilderFactories builderFactories, IMatchContexts matchContexts, IReadOnlyList<SkillDefinition> skills,
            IReadOnlyList<PassiveNodeDefinition> passives)
        {
            _builderFactories = builderFactories;
            _matchContexts = matchContexts;

            _statMatchers = new Lazy<IReadOnlyList<IStatMatchers>>(
                () => CreateStatMatchers(new ModifierBuilder(), passives));
            _referencedMatchers = new Lazy<IReadOnlyList<IReferencedMatchers>>(
                () => CreateReferencedMatchers(skills));
            _statMatchersSelector = new Lazy<StatMatchersSelector>(
                () => new StatMatchersSelector(StatMatchers));
        }

        public IReadOnlyList<IStatMatchers> StatMatchers => _statMatchers.Value;

        public IReadOnlyList<IReferencedMatchers> ReferencedMatchers => _referencedMatchers.Value;

        public IReadOnlyList<StatReplacerData> StatReplacers => _statReplacers.Value;

        public IStepper<ParsingStep> Stepper => _stepper.Value;

        public IStatMatchers SelectStatMatcher(ParsingStep step) => _statMatchersSelector.Value.Get(step);

        private IReadOnlyList<IStatMatchers> CreateStatMatchers(
            IModifierBuilder modifierBuilder, IReadOnlyList<PassiveNodeDefinition> passives)
            => new IStatMatchers[]
            {
                new SpecialMatchers(_builderFactories, _matchContexts, modifierBuilder),
                new StatManipulatorMatchers(_builderFactories, _matchContexts, modifierBuilder),
                new ValueConversionMatchers(_builderFactories, _matchContexts, modifierBuilder),
                new FormAndStatMatchers(_builderFactories, _matchContexts, modifierBuilder),
                new KeystoneStatMatchers(_builderFactories, _matchContexts, modifierBuilder, passives),
                new FormMatchers(_builderFactories, _matchContexts, modifierBuilder),
                new GeneralStatMatchers(_builderFactories, _matchContexts, modifierBuilder),
                new DamageStatMatchers(_builderFactories, _matchContexts, modifierBuilder),
                new PoolStatMatchers(_builderFactories, _matchContexts, modifierBuilder),
                new ConditionMatchers(_builderFactories, _matchContexts, modifierBuilder),
                new ActionConditionMatchers(_builderFactories, _matchContexts, modifierBuilder),
            };

        private IReadOnlyList<IReferencedMatchers> CreateReferencedMatchers(IReadOnlyList<SkillDefinition> skills) =>
            new IReferencedMatchers[]
            {
                new ActionMatchers(_builderFactories.ActionBuilders),
                new AilmentMatchers(_builderFactories.EffectBuilders.Ailment),
                new ChargeTypeMatchers(_builderFactories.ChargeTypeBuilders),
                new DamageTypeMatchers(_builderFactories.DamageTypeBuilders),
                new BuffMatchers(_builderFactories.BuffBuilders),
                new KeywordMatchers(_builderFactories.KeywordBuilders),
                new SkillMatchers(skills, _builderFactories.SkillBuilders.FromId),
            };
    }
}