using System;
using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.GameModel.PassiveTree;

namespace PoESkillTree.Engine.Computation.Builders.Stats
{
    public class MultipliedAttributeForEachNodeInRadiusStatBuilder : ICoreStatBuilder
    {
        private readonly Func<BuildParameters, IEnumerable<PassiveNodeDefinition>> _getNodesInRadius;
        private readonly IStatBuilder _source;
        private readonly IStatBuilder _target;

        public MultipliedAttributeForEachNodeInRadiusStatBuilder(
            Func<BuildParameters, IEnumerable<PassiveNodeDefinition>> getNodesInRadius,
            IStatBuilder source, IStatBuilder target)
        {
            _getNodesInRadius = getNodesInRadius;
            _source = source;
            _target = target;
        }

        public ICoreStatBuilder Resolve(ResolveContext context)
            => new MultipliedAttributeForEachNodeInRadiusStatBuilder(
                _getNodesInRadius, _source.Resolve(context), _target.Resolve(context));

        public ICoreStatBuilder WithEntity(IEntityBuilder entityBuilder)
            => new MultipliedAttributeForEachNodeInRadiusStatBuilder(
                _getNodesInRadius, _source.For(entityBuilder), _target.For(entityBuilder));

        public IEnumerable<StatBuilderResult> Build(BuildParameters parameters)
        {
            foreach (var node in _getNodesInRadius(parameters))
            {
                var value = _source.AsPassiveNodePropertyFor(node.Id).ValueFor(NodeType.BaseSet);
                var stat = _target.AsPassiveNodePropertyFor(node.Id);
                foreach (var (stats, source, valueConverter) in stat.Build(parameters))
                {
                    yield return new StatBuilderResult(stats, source, valueConverter.AndThen(v => v.Multiply(value)));
                }
            }
        }
    }
}