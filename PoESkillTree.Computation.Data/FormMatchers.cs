﻿using System.Collections.Generic;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;
using PoESkillTree.Computation.Parsing.Builders;
using PoESkillTree.Computation.Parsing.Builders.Matching;
using PoESkillTree.Computation.Parsing.Data;
using PoESkillTree.Computation.Parsing.ModifierBuilding;

namespace PoESkillTree.Computation.Data
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="IStatMatchers"/> implementation matching stat parts specifying forms and values.
    /// </summary>
    public class FormMatchers : StatMatchersBase
    {
        private readonly IModifierBuilder _modifierBuilder;

        public FormMatchers(IBuilderFactories builderFactories,
            IMatchContexts matchContexts, IModifierBuilder modifierBuilder)
            : base(builderFactories, matchContexts)
        {
            _modifierBuilder = modifierBuilder;
        }

        protected override IEnumerable<MatcherData> CreateCollection() =>
            new FormMatcherCollection(_modifierBuilder, ValueFactory)
            {
                { "#% increased", PercentIncrease, Value },
                { "#% reduced", PercentReduce, Value },
                { "#% more", PercentMore, Value },
                { "#% less", PercentLess, Value },
                { @"\+#%? to", BaseAdd, Value },
                { @"\+?#%?(?= chance)", BaseAdd, Value },
                { @"\+?#% of", BaseAdd, Value },
                { "gain #% of", BaseAdd, Value },
                { "(?<!chance to )gain #", BaseAdd, Value },
                { "#% additional", BaseAdd, Value },
                { "an additional", BaseAdd, 1 },
                { "adds # to", BaseAdd, Value },
                { "can (have|summon) up to # additional", BaseAdd, Value },
                { @"-#% of", BaseSubtract, Value },
                { "-#%? to", BaseSubtract, Value },
            };

        // Add (that word appearing is required for ReSharper to highlight these regex patterns ...)
    }
}