﻿using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Common.Utils;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Entities;
using PoESkillTree.Computation.Common.Builders.Resolving;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.Computation.Common.Parsing;

namespace PoESkillTree.Computation.Builders.Stats
{
    public class CompositeCoreStatBuilder : ICoreStatBuilder
    {
        private readonly IReadOnlyList<ICoreStatBuilder> _items;

        public CompositeCoreStatBuilder(params ICoreStatBuilder[] items) =>
            _items = items;

        private CompositeCoreStatBuilder Select(Func<ICoreStatBuilder, ICoreStatBuilder> selector) =>
            new CompositeCoreStatBuilder(_items.Select(selector).ToArray());

        public ICoreStatBuilder Resolve(ResolveContext context) =>
            Select(i => i.Resolve(context));

        public ICoreStatBuilder WithEntity(IEntityBuilder entityBuilder) =>
            Select(i => i.WithEntity(entityBuilder));

        public ICoreStatBuilder WithStatConverter(Func<IStat, IStat> statConverter) =>
            Select(i => i.WithStatConverter(statConverter));

        public IValue BuildValue(BuildParameters parameters) =>
            throw new ParseException("Can only access the value of stat builders that represent a single stat");

        public IEnumerable<StatBuilderResult> Build(BuildParameters parameters, ModifierSource originalModifierSource)
        {
            IEnumerable<StatBuilderResult> seed = new[]
            {
                new StatBuilderResult(new IStat[0], originalModifierSource, Funcs.Identity)
            };
            return _items.Aggregate(seed, Aggregate);

            IEnumerable<StatBuilderResult> Aggregate(IEnumerable<StatBuilderResult> previous, ICoreStatBuilder item) =>
                from p in previous
                from c in item.Build(parameters, p.ModifierSource).ToList()
                let stats = p.Stats.Concat(c.Stats).ToList()
                let source = c.ModifierSource
                let valueConverter = p.ValueConverter.AndThen(c.ValueConverter)
                select new StatBuilderResult(stats, source, valueConverter);
        }
    }
}