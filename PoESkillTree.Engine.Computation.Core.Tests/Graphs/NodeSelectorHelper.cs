using PoESkillTree.Engine.Computation.Common;

namespace PoESkillTree.Engine.Computation.Core.Graphs
{
    internal static class NodeSelectorHelper
    {
        public static NodeSelector Selector(NodeType nodeType) => new NodeSelector(nodeType, PathDefinition.MainPath);

        public static FormNodeSelector Selector(Form form) => new FormNodeSelector(form, PathDefinition.MainPath);
    }
}