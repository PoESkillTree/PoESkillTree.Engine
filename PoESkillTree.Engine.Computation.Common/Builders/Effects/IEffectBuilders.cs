using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Common.Builders.Effects
{
    /// <summary>
    /// Factory interface for effects.
    /// </summary>
    public interface IEffectBuilders
    {
        /// <summary>
        /// Gets an effect representing stuns.
        /// </summary>
        IStunEffectBuilder Stun { get; }

        /// <summary>
        /// Gets an effect representing knockbacks.
        /// </summary>
        IKnockbackEffectBuilder Knockback { get; }

        /// <summary>
        /// Gets a factory for ailment effects.
        /// </summary>
        IAilmentBuilders Ailment { get; }

        /// <summary>
        /// Gets a factory for ground effects.
        /// </summary>
        IGroundEffectBuilders Ground { get; }

        /// <summary>
        /// Modifier to the expiration of all effects on Self.
        /// actual duration = duration / ExpirationModifier
        /// </summary>
        IStatBuilder ExpirationModifier { get; }
    }
}