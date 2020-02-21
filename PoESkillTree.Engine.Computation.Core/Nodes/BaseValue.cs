using PoESkillTree.Engine.Computation.Common;

namespace PoESkillTree.Engine.Computation.Core.Nodes
{
    /// <summary>
    /// <see cref="IValue"/> for <see cref="NodeType.Base"/> on non-conversion paths.
    /// </summary>
    public class BaseValue : IValue
    {
        private readonly IStat _stat;
        private readonly PathDefinition _path;

        public BaseValue(IStat stat, PathDefinition path)
        {
            _stat = stat;
            _path = path;
        }

        public NodeValue? Calculate(IValueCalculationContext context)
        {
            var baseSet = context.GetValue(_stat, NodeType.BaseSet, _path);
            var baseAdd = context.GetValue(_stat, NodeType.BaseAdd, _path);
            if (baseSet is null && baseAdd is null)
                return null;
            return _stat.Round((baseSet ?? new NodeValue(0)) + (baseAdd ?? new NodeValue(0)));
        }
    }
}