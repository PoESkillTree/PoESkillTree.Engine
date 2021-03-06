using PoESkillTree.Engine.Computation.Common.Builders.Actions;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Common.Builders.Skills
{
    /// <summary>
    /// Represents a collection of skills.
    /// </summary>
    /// <remarks>
    /// The stat properties that are the same as in <see cref="ISkillBuilder"/> only make sense as modifiers applied
    /// to the stats of skills in the collection.
    /// </remarks>
    public interface ISkillBuilderCollection : IResolvable<ISkillBuilderCollection>
    {
        /// <summary>
        /// Gets an action that occurs when Self casts any skill in this collection.
        /// </summary>
        IActionBuilder Cast { get; }

        /// <summary>
        /// Gets a stat representing the number of active instances of all skills in this collection combined 
        /// (cast by Self).
        /// </summary>
        IStatBuilder CombinedInstances { get; }

        /// <summary>
        /// The percentage of a pool skills in this collection reserve.
        /// </summary>
        IStatBuilder Reservation { get; }

        /// <summary>
        /// The pool skills in this collection reserve.
        /// </summary>
        IStatBuilder ReservationPool { get; }
    }
}