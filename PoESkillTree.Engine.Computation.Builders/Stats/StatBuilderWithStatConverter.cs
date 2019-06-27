using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Builders.Stats
{
    internal class StatBuilderWithStatConverter : ICoreStatBuilder
    {
        private readonly ICoreStatBuilder _inner;
        private readonly Func<ModifierSource, IStat, IStat> _statConverter;

        public StatBuilderWithStatConverter(ICoreStatBuilder inner, Func<ModifierSource, IStat, IStat> statConverter)
            => (_inner, _statConverter) = (inner, statConverter);

        public ICoreStatBuilder Resolve(ResolveContext context) =>
            new StatBuilderWithStatConverter(_inner.Resolve(context), _statConverter);

        public ICoreStatBuilder WithEntity(IEntityBuilder entityBuilder) =>
            new StatBuilderWithStatConverter(_inner.WithEntity(entityBuilder), _statConverter);

        public IEnumerable<StatBuilderResult> Build(BuildParameters parameters) =>
            from r in _inner.Build(parameters)
            let stats = r.Stats.Select(s => _statConverter(r.ModifierSource, s)).ToList()
            select new StatBuilderResult(stats, r.ModifierSource, r.ValueConverter);
    }
}