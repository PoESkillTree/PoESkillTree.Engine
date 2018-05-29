using PoESkillTree.Computation.Common.Builders.Resolving;

namespace PoESkillTree.Computation.Common.Builders.Stats
{
    /// <summary>
    /// Represents a stat for damage. Damage can be limited by damage type and everything from
    /// <see cref="IDamageRelatedStatBuilder"/>.
    /// </summary>
    public interface IDamageStatBuilder : IDamageRelatedStatBuilder
    {
        /// <summary>
        /// Gets a stat representing the modifier to damage taken of this stat's damage types.
        /// </summary>
        IDamageRelatedStatBuilder Taken { get; }

        /// <summary>
        /// Starts constructing a stat representing the percentage of damage of this stat's damage types that is taken
        /// from the given pool before being taken from another pool.
        /// </summary>
        IDamageTakenConversionBuilder TakenFrom(IPoolStatBuilder pool);
    }


    public interface IDamageTakenConversionBuilder : IResolvable<IDamageTakenConversionBuilder>
    {
        /// <summary>
        /// Returns a stat representing the percentage of damage of specific types that is taken from a specific pool
        /// before being taken from the given pool.
        /// </summary>
        IStatBuilder Before(IPoolStatBuilder pool);
    }
}