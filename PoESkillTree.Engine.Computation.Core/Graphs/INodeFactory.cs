using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Core.Nodes;

namespace PoESkillTree.Engine.Computation.Core.Graphs
{
    /// <summary>
    /// Factory for creating nodes (<see cref="IDisposableNodeViewProvider"/>) that calculate their value using
    /// <see cref="IValue"/>.
    /// </summary>
    public interface INodeFactory
    {
        IDisposableNodeViewProvider Create(IValue value, PathDefinition path);
    }
}