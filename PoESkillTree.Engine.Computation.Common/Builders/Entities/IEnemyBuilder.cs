using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Values;

namespace PoESkillTree.Engine.Computation.Common.Builders.Entities
{
    /// <summary>
    /// Represents enemy entities.
    /// </summary>
    public interface IEnemyBuilder : ICountableEntityBuilder
    {
        /// <summary>
        /// Gets a condition that is satisfied if this enemy is near Self.
        /// </summary>
        IConditionBuilder IsNearby { get; }

        ValueBuilder CountRareOrUniqueNearby { get; }

        ValueBuilder Distance { get; }

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

        IConditionBuilder IsMoving { get; }
    }
}