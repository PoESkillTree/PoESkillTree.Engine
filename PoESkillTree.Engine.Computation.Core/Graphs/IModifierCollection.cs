using PoESkillTree.Engine.Computation.Common;

namespace PoESkillTree.Engine.Computation.Core.Graphs
{
    /// <summary>
    /// Writing interface for a collection of <see cref="Modifier"/>s.
    /// </summary>
    public interface IModifierCollection
    {
        void AddModifier(Modifier modifier);
        void RemoveModifier(Modifier modifier);
    }
}