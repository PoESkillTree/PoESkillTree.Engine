using System.Collections.Generic;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;
using PoESkillTree.Computation.Providers;
using PoESkillTree.Computation.Providers.Matching;
using PoESkillTree.Computation.Providers.Stats;

namespace PoESkillTree.Computation.Data
{
    public class PoolStatMatchers : UsesMatchContext, IStatMatchers
    {
        public PoolStatMatchers(IProviderFactories providerFactories, 
            IMatchContextFactory matchContextFactory) 
            : base(providerFactories, matchContextFactory)
        {
            StatMatchers = CreateCollection();
        }

        public IEnumerable<object> StatMatchers { get; }

        private StatMatcherCollection<IPoolStatProvider> CreateCollection() =>
            new StatMatcherCollection<IPoolStatProvider>
            {
                { "(maximum )?life", Life },
                { "(maximum )?mana", Mana },
                { "(maximum )?energy shield", EnergyShield },
            };
    }
}