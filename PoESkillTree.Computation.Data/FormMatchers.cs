﻿using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;
using PoESkillTree.Computation.Providers;

namespace PoESkillTree.Computation.Data
{
    public class FormMatchers : UsesFormProviders, IStatMatchers
    {
        private readonly IMatchBuilder _matchBuilder;
        private readonly Lazy<IReadOnlyList<MatcherData>> _lazyMatchers;

        public FormMatchers(IProviderFactories providerFactories, IMatchBuilder matchBuilder) 
            : base(providerFactories)
        {
            _matchBuilder = matchBuilder;
            _lazyMatchers = new Lazy<IReadOnlyList<MatcherData>>(() => CreateCollection().ToList());
        }

        public IReadOnlyList<MatcherData> Matchers => _lazyMatchers.Value;

        private FormMatcherCollection CreateCollection() => new FormMatcherCollection(_matchBuilder,
            ValueFactory)
        {
            { "#% increased", PercentIncrease },
            { "#% reduced", PercentReduce },
            { "#% more", PercentMore },
            { "#% less", PercentLess },
            { @"\+#%? to", BaseAdd },
            { @"\+?#%?(?= chance)", BaseAdd },
            { @"\+?#% of", BaseAdd },
            { "gain #% of", BaseAdd },
            { "gain #", BaseAdd },
            { "#% additional", BaseAdd },
            { "an additional", BaseAdd, 1 },
            { @"-#% of", BaseSubtract },
            { "-#%? to", BaseSubtract },
            { "can (have|summon) up to # additional", MaximumAdd },
        };
    }
}