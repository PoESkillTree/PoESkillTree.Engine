using PoESkillTree.Engine.Computation.Common.Builders.Resolving;

namespace PoESkillTree.Engine.Computation.Common.Builders.Skills
{
    /// <summary>
    /// Represents a gem tag.
    /// </summary>
    public interface IGemTagBuilder : IResolvable<IGemTagBuilder>
    {
        /// <summary>
        /// Builds to the internal id of the gem tag.
        /// </summary>
        string Build(BuildParameters parameters);
    }
}