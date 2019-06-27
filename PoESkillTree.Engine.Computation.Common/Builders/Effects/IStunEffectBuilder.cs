using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Common.Builders.Effects
{
    /// <summary>
    /// Represents the stun effect and action.
    /// </summary>
    public interface IStunEffectBuilder : IAvoidableEffectBuilder
    {
        /// <summary>
        /// Gets a stat representing the stun threshold modifier.
        /// </summary>
        IDamageRelatedStatBuilder Threshold { get; }

        /// <summary>
        /// Gets a stat representing the stun recovery modifier.
        /// </summary>
        IStatBuilder Recovery { get; }

        IStatBuilder ChanceToAvoidInterruptionWhileCasting { get; }
    }
}