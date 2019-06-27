using PoESkillTree.Engine.Computation.Core.NodeCollections;
using PoESkillTree.Engine.Computation.Core.Nodes;

namespace PoESkillTree.Engine.Computation.Core.Graphs
{
    /// <summary>
    /// Factory for creating the nodes and modifier node collections for stat subgraphs.
    /// </summary>
    public interface IStatNodeFactory
    {
        IDisposableNodeViewProvider Create(NodeSelector selector);
        ModifierNodeCollection Create(FormNodeSelector selector);
    }
}