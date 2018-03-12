﻿using System.Collections.Generic;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Core.Events;
using PoESkillTree.Computation.Core.NodeCollections;
using PoESkillTree.Computation.Core.Nodes;

namespace PoESkillTree.Computation.Core.Graphs
{
    public class StatRegistry : IDeterminesNodeRemoval
    {
        private readonly NodeCollection<IStat> _nodeCollection;

        private readonly Dictionary<IStat, WrappingNode> _registeredWrappedNodes =
            new Dictionary<IStat, WrappingNode>();

        private readonly Dictionary<IStat, ICalculationNode> _registeredNodes =
            new Dictionary<IStat, ICalculationNode>();

        public StatRegistry(NodeCollection<IStat> nodeCollection)
        {
            _nodeCollection = nodeCollection;
        }

        public INodeRepository NodeRepository { private get; set; }

        public void Add(IStat stat)
        {
            if (!stat.IsRegisteredExplicitly)
                return;
            var node = NodeRepository.GetNode(stat);
            _registeredNodes[stat] = node;
            var wrappedNode = new WrappingNode(node);
            _registeredWrappedNodes[stat] = wrappedNode;
            _nodeCollection.Add(wrappedNode, stat);
        }

        public void Remove(IStat stat)
        {
            if (!_registeredWrappedNodes.TryGetValue(stat, out var wrappedNode))
                return;
            wrappedNode.Dispose();
            _registeredNodes.Remove(stat);
            _registeredWrappedNodes.Remove(stat);
            _nodeCollection.Remove(wrappedNode);
        }

        public bool CanBeRemoved(ISuspendableEventViewProvider<ICalculationNode> node)
        {
            if (_registeredNodes.ContainsValue(node.SuspendableView))
            {
                return node.SubscriberCount <= 1;
            }
            return node.SubscriberCount == 0;
        }

        public bool CanBeRemoved(ICountsSubsribers node) => node.SubscriberCount == 0;
    }
}