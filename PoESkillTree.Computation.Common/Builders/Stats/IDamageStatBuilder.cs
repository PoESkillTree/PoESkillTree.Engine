using PoESkillTree.Common.Model.Items.Enums;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Damage;
using PoESkillTree.Computation.Common.Builders.Effects;
using PoESkillTree.Computation.Common.Builders.Resolving;

namespace PoESkillTree.Computation.Common.Builders.Stats
{
    /// <summary>
    /// Represents a stat for damage. Damage can be limited by damage type and source, and skill vs. ailment.
    /// </summary>
    public interface IDamageStatBuilder : IStatBuilder
    {
        /// <summary>
        /// Gets a stat representing the modifier to damage taken of this stat's damage types.
        /// </summary>
        IStatBuilder Taken { get; }

        /// <summary>
        /// Starts constructing a stat representing the percentage of damage of this stat's damage types that is taken
        /// from the given pool before being taken from another pool.
        /// </summary>
        IDamageTakenConversionBuilder TakenFrom(IPoolStatBuilder pool);

        /// <summary>
        /// Limits the damage by source.
        /// </summary>
        IDamageStatBuilder With(IDamageSourceBuilder source);

        /// <summary>
        /// Limits the damage to not apply to damage over time.
        /// </summary>
        IDamageStatBuilder WithHits { get; }

        /// <summary>
        /// Limits the damage to not apply to non-ailment damage over time.
        /// </summary>
        IDamageStatBuilder WithHitsAndAilments { get; }

        /// <summary>
        /// Limits the damage to only apply to ailments.
        /// </summary>
        IDamageStatBuilder WithAilments { get; }

        /// <summary>
        /// Limits the damage to only apply to the given ailment.
        /// </summary>
        IDamageStatBuilder With(IAilmentBuilder ailment);

        /// <summary>
        /// Returns a condition that is satisfied if damage dealt is of (any of) this stat's damage types and if
        /// the damage is dealt with a weapon having the given tags.
        /// </summary>
        IConditionBuilder With(Tags tags);

        /// <summary>
        /// Returns a condition that is satisfied if damage dealt is of (any of) this stat's damage types and if
        /// the damage is dealt with a weapon in the given slot (either MainHand or OffHand).
        /// </summary>
        IConditionBuilder With(ItemSlot slot);
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