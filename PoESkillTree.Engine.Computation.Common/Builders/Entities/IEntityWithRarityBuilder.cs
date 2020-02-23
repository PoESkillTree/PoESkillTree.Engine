using PoESkillTree.Engine.Computation.Common.Builders.Conditions;

namespace PoESkillTree.Engine.Computation.Common.Builders.Entities
{
    public interface IEntityWithRarityBuilder : IEntityBuilder
    {
        /// <summary>
        /// Gets a condition that is satisfied if this enemy is Rare. 
        /// </summary>
        IConditionBuilder IsRare { get; }

        /// <summary>
        /// Gets a condition that is satisfied if this enemy is Unique. 
        /// </summary>
        IConditionBuilder IsUnique { get; }

        /// <summary>
        /// Gets a condition that is satisfied if this enemy is Rare or Unique.
        /// </summary>
        IConditionBuilder IsRareOrUnique { get; }
    }
}