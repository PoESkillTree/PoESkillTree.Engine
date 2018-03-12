﻿using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Core.Events;
using PoESkillTree.Computation.Core.Graphs;
using PoESkillTree.Computation.Core.NodeCollections;
using PoESkillTree.Computation.Core.Nodes;

namespace PoESkillTree.Computation.Core
{
    public class Calculator : ICalculator
    {
        private readonly ISuspendableEvents _suspender;
        private readonly IModifierCollection _modifierCollection;
        private readonly ICalculationGraphPruner _graphPruner;

        public Calculator(
            ISuspendableEvents suspender, IModifierCollection modifierCollection,
            ICalculationGraphPruner graphPruner, INodeRepository nodeRepository,
            INodeCollection<IStat> explicitlyRegisteredStats)
        {
            _suspender = suspender;
            _modifierCollection = modifierCollection;
            _graphPruner = graphPruner;
            NodeRepository = nodeRepository;
            ExplicitlyRegisteredStats = explicitlyRegisteredStats;
        }

        public INodeRepository NodeRepository { get; }

        public INodeCollection<IStat> ExplicitlyRegisteredStats { get; }

        public void Update(CalculatorUpdate update)
        {
            _suspender.SuspendEvents();
            // If the remove/add order matters for performance, ordering logic could be added.
            foreach (var modifier in update.RemovedModifiers)
            {
                _modifierCollection.RemoveModifier(modifier);
            }
            foreach (var modifier in update.AddedModifiers)
            {
                _modifierCollection.AddModifier(modifier);
            }
            _graphPruner.RemoveUnusedNodes();
            _suspender.ResumeEvents();
        }

        // Passing an empty SuspendableEventsComposite instead
        // might be a significant performance improvement for "preview" calculations, where events are not used.
        // (or add a property to ICalculator to disable/enable suspender usage)
        public static Calculator CreateCalculator()
        {
            var innerNodeFactory = new NodeFactory();
            var nodeFactory = new TransformableNodeFactory(innerNodeFactory, v => new TransformableValue(v));
            var statRegistryCollection = new SuspendableNodeCollection<IStat>();
            var statRegistry = new StatRegistry(statRegistryCollection);
            var coreGraph = new CoreCalculationGraph(
                s => new CoreStatGraph(new StatNodeFactory(nodeFactory, s)), nodeFactory);
            var eventGraph = new CalculationGraphWithEvents(coreGraph);
            eventGraph.StatAdded += (_, args) => statRegistry.Add(args.Stat);
            eventGraph.StatRemoved += (_, args) => statRegistry.Remove(args.Stat);
            var prunableGraph = new PrunableCalculationGraph(eventGraph, statRegistry);

            var defaultView = new DefaultViewNodeRepository(prunableGraph);
            var suspendableView = new SuspendableViewNodeRepository(prunableGraph);
            innerNodeFactory.NodeRepository = defaultView;
            statRegistry.NodeRepository = suspendableView;

            var suspender = new SuspendableEventsComposite();
            suspender.Add(new StatGraphCollectionSuspender(prunableGraph));
            suspender.Add(statRegistryCollection);

            return new Calculator(suspender, prunableGraph, prunableGraph, suspendableView, statRegistryCollection);
        }
    }
}