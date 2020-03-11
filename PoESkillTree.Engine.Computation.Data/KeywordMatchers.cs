using PoESkillTree.Engine.Computation.Data.Base;
using PoESkillTree.Engine.Computation.Data.Collections;
using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common.Builders.Skills;
using PoESkillTree.Engine.Computation.Common.Data;

namespace PoESkillTree.Engine.Computation.Data
{
    /// <summary>
    /// <see cref="IReferencedMatchers"/> implementation for <see cref="IKeywordBuilder"/>s.
    /// </summary>
    public class KeywordMatchers : ReferencedMatchersBase<IKeywordBuilder>
    {
        private IKeywordBuilders Keyword { get; }

        public KeywordMatchers(IKeywordBuilders keywordBuilders)
        {
            Keyword = keywordBuilders;
        }

        protected override IReadOnlyList<ReferencedMatcherData> CreateCollection() =>
            new ReferencedMatcherCollection<IKeywordBuilder>
            {
                { "melee", Keyword.Melee },
                { "projectiles?", Keyword.Projectile },
                { "golems?", Keyword.Golem },
                { "traps?", Keyword.Trap },
                { "mines?", Keyword.Mine },
                { "totems?", Keyword.Totem },
                { "curses?", Keyword.Curse },
                { "auras?", Keyword.Aura },
                { "area", Keyword.AreaOfEffect },
                { "warcry", Keyword.Warcry },
                { "herald", Keyword.Herald },
                { "brand", Keyword.Brand },
                { "movement", Keyword.Movement },
                { "banner", Keyword.Banner },
                { "channelling", Keyword.From(GameModel.Skills.Keyword.Channelling) },
                { "guard", Keyword.From(GameModel.Skills.Keyword.Guard) },
                { "triggered", Keyword.Triggered },
                { "travel", Keyword.From(GameModel.Skills.Keyword.Travel) },
            };
    }
}