﻿using System.Collections.Generic;
using PoESkillTree.Computation.Common.Builders.Buffs;
using PoESkillTree.Computation.Common.Builders.Resolving;
using PoESkillTree.Computation.Common.Builders.Skills;

namespace PoESkillTree.Computation.Builders.Buffs
{
    public class BuffBuilderWithKeywords : IResolvable<BuffBuilderWithKeywords>
    {
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