using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common.Builders.Buffs;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Base;
using PoESkillTree.Engine.Computation.Data.Collections;

namespace PoESkillTree.Engine.Computation.Data
{
    /// <summary>
    /// <see cref="IReferencedMatchers"/> implementation for <see cref="IBuffBuilder"/>s.
    /// </summary>
    public class BuffMatchers : ReferencedMatchersBase<IBuffBuilder>
    {
        private IBuffBuilders Buff { get; }

        public BuffMatchers(IBuffBuilders buffBuilders)
        {
            Buff = buffBuilders;
        }

        protected override IReadOnlyList<ReferencedMatcherData> CreateCollection() =>
            new ReferencedMatcherCollection<IBuffBuilder>
            {
                { "fortify", Buff.Fortify },
                { "maim(ed)?", Buff.Maim },
                { "hinder(ed)?", Buff.Hinder },
                { "intimidated?", Buff.Intimidate },
                { "taunt(ed)?", Buff.Taunt },
                { "blind(ed)?", Buff.Blind },
                { "onslaught", Buff.Onslaught },
                { "unholy might", Buff.UnholyMight },
                { "phasing", Buff.Phasing },
                { "arcane surge", Buff.ArcaneSurge },
                { "tailwind", Buff.Tailwind },
                { "innervation", Buff.Innervation },
                { "impaled?", Buff.Impale },
                { "infusion", Buff.Infusion },
                { "snares?", Buff.Snare },
                { "ensnared", Buff.Snare },
                { "wither(ed)?", Buff.Withered },
                { "elusive", Buff.Elusive },
            }; // Add
    }
}