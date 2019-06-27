using PoESkillTree.Engine.Computation.Common.Builders.Actions;
using PoESkillTree.Engine.Computation.Common.Builders.Buffs;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Common.Builders.Skills
{
    /// <summary>
    /// Represents a skill.
    /// </summary>
    public interface ISkillBuilder : IResolvable<ISkillBuilder>
    {
        /// <summary>
        /// Gets an action that occurs when Self casts this skill.
        /// </summary>
        IActionBuilder Cast { get; }

        /// <summary>
        /// Gets a stat representing the number of active instances of this skill (cast by Self).
        /// (e.g. the number of zombies cast by Raise Zombie)
        /// </summary>
        IStatBuilder Instances { get; }

        /// <summary>
        /// The amount or percentage of a pool this skill reserves.
        /// </summary>
        IStatBuilder Reservation { get; }

        /// <summary>
        /// The pool this skill's reservation uses.
        /// </summary>
        IStatBuilder ReservationPool { get; }

        /// <summary>
        /// This skill's identifier.
        /// </summary>
        ValueBuilder SkillId { get; }

        /// <summary>
        /// The buff provided by this skill. Returns a dummy buff if this skill does not provide a buff.
        /// </summary>
        IBuffBuilder Buff { get; }

        SkillDefinition Build(BuildParameters parameters);
    }
}