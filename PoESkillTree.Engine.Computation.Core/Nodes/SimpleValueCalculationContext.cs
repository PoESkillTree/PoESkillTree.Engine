using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common;

namespace PoESkillTree.Engine.Computation.Core.Nodes
{
    public class SimpleValueCalculationContext : IValueCalculationContext
    {
        private readonly INodeRepository _nodeRepository;

        public SimpleValueCalculationContext(INodeRepository nodeRepository)
        {
            _nodeRepository = nodeRepository;
        }

        public PathDefinition CurrentPath { get; } = PathDefinition.MainPath;

        public IReadOnlyCollection<PathDefinition> GetPaths(IStat stat) =>
            _nodeRepository.GetPaths(stat);

        public NodeValue? GetValue(IStat stat, NodeType nodeType, PathDefinition path) =>
            _nodeRepository.GetNode(stat, nodeType, path).Value;

        public List<NodeValue?> GetValues(Form form, IEnumerable<(IStat stat, PathDefinition path)> paths) =>
            paths
                .SelectMany(t => _nodeRepository.GetFormNodeCollection(t.stat, form, t.path))
                .Select(t => t.node)
                .Distinct()
                .Select(n => n.Value)
                .ToList();
    }
}