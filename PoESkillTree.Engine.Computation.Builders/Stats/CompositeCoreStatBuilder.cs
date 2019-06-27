using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Parsing;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Builders.Stats
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

        public IEnumerable<StatBuilderResult> Build(BuildParameters parameters)
        {
            IEnumerable<StatBuilderResult> seed = new[]
            {
                new StatBuilderResult(new IStat[0], parameters.ModifierSource, Funcs.Identity)
            };
            return _items.Aggregate(seed, Aggregate);

            IEnumerable<StatBuilderResult> Aggregate(IEnumerable<StatBuilderResult> previous, ICoreStatBuilder item)
            {
                var previousResults = previous.ToList();
                foreach (var previousResult in previousResults)
                {
                    var currentParameters = parameters.With(previousResult.ModifierSource);
                    var currentResults = item.Build(currentParameters).ToList();
                    if (previousResults.Count > 1 && currentResults.Count > 1)
                        throw new ParseException("At most one composite item may build to multiple StatBuilderResults");
                    foreach (var currentResult in currentResults)
                    {
                        yield return new StatBuilderResult(
                            previousResult.Stats.Concat(currentResult.Stats).ToList(),
                            currentResult.ModifierSource,
                            previousResult.ValueConverter.AndThen(currentResult.ValueConverter));
                    }
                }
            }
        }
    }
}