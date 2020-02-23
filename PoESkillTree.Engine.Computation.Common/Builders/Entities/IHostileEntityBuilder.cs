using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Values;

namespace PoESkillTree.Engine.Computation.Common.Builders.Entities
{
    /// <summary>
    /// Represents hostile entities.
    /// </summary>
    public interface IHostileEntityBuilder : ICountableEntityBuilder, IEntityWithRarityBuilder
    {
        /// <summary>
        /// Gets a condition that is satisfied if this entity is near Self.
        /// </summary>
        IConditionBuilder IsNearby { get; }

        ValueBuilder CountRareOrUniqueNearby { get; }

        ValueBuilder Distance { get; }

        IConditionBuilder IsMoving { get; }
    }
}