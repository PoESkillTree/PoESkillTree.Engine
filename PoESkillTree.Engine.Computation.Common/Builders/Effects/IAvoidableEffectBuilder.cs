using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Common.Builders.Effects
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an avoidable effect, e.g. ailments or stun.
    /// </summary>
    public interface IAvoidableEffectBuilder : IEffectBuilder
    {
        /// <summary>
        /// Gets a stat representing the chance to avoid this effect when inflicted upon Self.
        /// </summary>
        IStatBuilder Avoidance { get; }
    }
}