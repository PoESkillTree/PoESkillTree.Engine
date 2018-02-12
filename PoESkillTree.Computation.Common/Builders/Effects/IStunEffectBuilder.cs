using PoESkillTree.Computation.Common.Builders.Actions;
using PoESkillTree.Computation.Common.Builders.Stats;

namespace PoESkillTree.Computation.Common.Builders.Effects
{
    /// <summary>
    /// Represents the stun effect and action.
    /// </summary>
    public interface IStunEffectBuilder : IAvoidableEffectBuilder, IActionBuilder
    {
        /// <summary>
        /// Gets a stat representing the stun threshold.
        /// </summary>
        IStatBuilder Threshold { get; }

        /// <summary>
        /// Gets a stat representing the stun recovery.
        /// </summary>
        IStatBuilder Recovery { get; }

        /// <summary>
        /// Gets a stat representing the chance to avoid interruption from stuns while casting.
        /// </summary>
        IStatBuilder ChanceToAvoidInterruptionWhileCasting { get; }
    }
}