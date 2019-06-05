using System.Collections.Generic;
using PoESkillTree.Computation.Common.Builders.Effects;
using PoESkillTree.Computation.Common.Data;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;

namespace PoESkillTree.Computation.Data
{
    /// <summary>
    /// <see cref="IReferencedMatchers"/> implementation for <see cref="IAilmentBuilder"/>s.
    /// </summary>
    public class AilmentMatchers : ReferencedMatchersBase<IAilmentBuilder>
    {
        private IAilmentBuilders Ailment { get; }

        public AilmentMatchers(IAilmentBuilders ailmentBuilders)
        {
            Ailment = ailmentBuilders;
        }

        protected override IReadOnlyList<ReferencedMatcherData> CreateCollection() =>
            new ReferencedMatcherCollection<IAilmentBuilder>
            {
                // chance to x/x duration/always x
                { "ignites?", Ailment.Ignite },
                { "shocks?", Ailment.Shock },
                { "chills?", Ailment.Chill },
                { "freezes?", Ailment.Freeze },
                { "bleed", Ailment.Bleed },
                { "cause bleeding", Ailment.Bleed },
                { "poisons?", Ailment.Poison },
                // being/while/against x
                { "ignited", Ailment.Ignite },
                { "shocked", Ailment.Shock },
                { "chilled", Ailment.Chill },
                { "frozen", Ailment.Freeze },
                { "bleeding", Ailment.Bleed },
                { "poisoned", Ailment.Poison },
            };
    }
}