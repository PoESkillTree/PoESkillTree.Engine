using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Common.Builders.Skills
{
    /// <summary>
    /// Represents a keyword describing skills, e.g. Attack or Projectile. A skill's keywords are retrieved from
    /// its ActiveSkillTypes and gem tags.
    /// </summary>
    public interface IKeywordBuilder : IResolvable<IKeywordBuilder>
    {
        Keyword Build(BuildParameters parameters);
    }
}