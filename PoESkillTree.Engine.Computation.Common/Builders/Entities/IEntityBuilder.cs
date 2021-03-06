using System.Collections.Generic;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Common.Builders.Entities
{
    /// <summary>
    /// Represents an entity that is source and target of modifier applications, can be affected by effect, 
    /// can be source and target of actions and similar.
    /// </summary>
    public interface IEntityBuilder
    {
        /// <summary>
        /// Builds to a potentially empty collection of entities.
        /// </summary>
        IReadOnlyCollection<Entity> Build(Entity modifierSourceEntity);
    }
}