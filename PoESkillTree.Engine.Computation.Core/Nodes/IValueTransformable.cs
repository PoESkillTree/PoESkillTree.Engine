using PoESkillTree.Engine.Computation.Common;

namespace PoESkillTree.Engine.Computation.Core.Nodes
{
    /// <summary>
    /// A value that can be transformed using <see cref="IValueTransformation"/>s.
    /// </summary>
    public interface IValueTransformable
    {
        void Add(IValueTransformation transformation);

        void Remove(IValueTransformation transformation);

        void RemoveAll();
    }
}