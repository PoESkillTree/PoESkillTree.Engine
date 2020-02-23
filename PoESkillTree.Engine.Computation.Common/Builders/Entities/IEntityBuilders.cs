using System.Collections.Generic;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Common.Builders.Entities
{
    /// <summary>
    /// Factory interface for entities.
    /// </summary>
    public interface IEntityBuilders
    {
        /// <summary>
        /// Gets the <see cref="BuildParameters.ModifierSourceEntity"/>.
        /// </summary>
        IEntityWithRarityBuilder Self { get; }

        /// <summary>
        /// Gets the entity/entities opposing <see cref="BuildParameters.ModifierSourceEntity"/>
        /// (Enemy opposes Character, Minion and Totem).
        /// </summary>
        IHostileEntityBuilder OpponentOfSelf { get; }

        /// <summary>
        /// Gets an entity representing enemies.
        /// </summary>
        IEntityBuilder Enemy { get; }

        /// <summary>
        /// Gets an entity representing the player character.
        /// </summary>
        IEntityBuilder Character { get; }

        /// <summary>
        /// Gets an entity representing the <see cref="BuildParameters.ModifierSourceEntity"/>'s allies.
        /// </summary>
        ICountableEntityBuilder Ally { get; }

        /// <summary>
        /// Gets an entity representing the <see cref="BuildParameters.ModifierSourceEntity"/>'s totems.
        /// </summary>
        IEntityBuilder Totem { get; }

        /// <summary>
        /// Gets an entity representing the <see cref="BuildParameters.ModifierSourceEntity"/>'s minions.
        /// </summary>
        IEntityBuilder Minion { get; }

        IEntityBuilder Any { get; }

        IEntityBuilder From(IEnumerable<Entity> entities);
    }
}