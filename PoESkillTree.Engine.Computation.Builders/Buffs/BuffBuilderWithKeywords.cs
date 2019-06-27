using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common.Builders.Buffs;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Builders.Buffs
{
    public class BuffBuilderWithKeywords : IResolvable<BuffBuilderWithKeywords>
    {
        public BuffBuilderWithKeywords(IBuffBuilder buff, params Keyword[] keywords)
            : this(buff, (IReadOnlyList<Keyword>) keywords)
        {
        }

        public BuffBuilderWithKeywords(IBuffBuilder buff, IReadOnlyList<Keyword> keywords)
        {
            Buff = buff;
            Keywords = keywords;
        }

        public IBuffBuilder Buff { get; }
        public IReadOnlyList<Keyword> Keywords { get; }

        public BuffBuilderWithKeywords Resolve(ResolveContext context) =>
            new BuffBuilderWithKeywords((IBuffBuilder) Buff.Resolve(context), Keywords);
    }
}