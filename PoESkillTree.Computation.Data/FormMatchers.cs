﻿using System.Collections.Generic;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;
using PoESkillTree.Computation.Providers;

namespace PoESkillTree.Computation.Data
{
    public class FormMatchers : UsesFormProviders, IStatMatchers
    {
        public FormMatchers(IProviderFactories providerFactories) : base(providerFactories)
        {
            StatMatchers = CreateCollection();
        }

        public IEnumerable<object> StatMatchers { get; }

        private FormMatcherCollection CreateCollection() => new FormMatcherCollection
        {
            { "#% increased", PercentIncrease },
            { "#% reduced", PercentReduce },
            { "#% more", PercentMore },
            { "#% less", PercentLess},
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