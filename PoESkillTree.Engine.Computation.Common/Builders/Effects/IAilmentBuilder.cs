using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Common.Builders.Effects
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an ailment.
    /// </summary>
    public interface IAilmentBuilder : IAvoidableEffectBuilder
    {
        /// <summary>
        /// Returns a stat representing the number of instances of this ailment currently affecting
        /// <paramref name="target"/>.
        /// </summary>
        IStatBuilder InstancesOn(IEntityBuilder target);

        /// <summary>
        /// Returns a stat representing whether all of the damage types in <paramref name="type"/> can inflict
        /// this ailment.
        /// </summary>
        IStatBuilder Source(IDamageTypeBuilder type);

        IStatBuilder CriticalStrikesAlwaysInflict { get; }

        IStatBuilder ChanceToRemove { get; }

        /// <summary>
        /// Modifies the tick rate of the ailment's damage
        /// </summary>
        IStatBuilder TickRateModifier { get; }

        new Ailment Build(BuildParameters parameters);
    }
}