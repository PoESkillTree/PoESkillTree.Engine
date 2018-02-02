﻿using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using PoESkillTree.Common.Utils.Extensions;
using PoESkillTree.Computation.Common;

namespace PoESkillTree.Computation.Core.Nodes
{
    public class ValueCalculationContext : IValueCalculationContext
    {
        private readonly INodeRepository _nodeRepository;
        private readonly ISet<ICalculationNode> _usedNodes = new HashSet<ICalculationNode>();
        private readonly ISet<INodeCollection> _usedNodeCollections = new HashSet<INodeCollection>();

        public ValueCalculationContext(INodeRepository nodeRepository)
        {
            _nodeRepository = nodeRepository;
        }

        public NodeValue? GetValue(IStat stat, NodeType nodeType = NodeType.Total)
        {
            var node = _nodeRepository.GetNode(stat, nodeType);
            _usedNodes.Add(node);
            return node.Value;
        }

        public IEnumerable<NodeValue?> GetValues(Form form, params IStat[] stats)
        {
            var nodeCollections = stats.Select(s => _nodeRepository.GetFormNodeCollection(s, form)).ToList();
            _usedNodeCollections.UnionWith(nodeCollections);
            var nodes = nodeCollections.Flatten().ToHashSet();
            _usedNodes.UnionWith(nodes);
            return nodes.Select(n => n.Value);
        }

        public IEnumerable<ICalculationNode> UsedNodes => _usedNodes;
        public IEnumerable<INodeCollection> UsedNodeCollections => _usedNodeCollections;

        public void Clear()
        {
            _usedNodes.Clear();
            _usedNodeCollections.Clear();
        }
    }
}