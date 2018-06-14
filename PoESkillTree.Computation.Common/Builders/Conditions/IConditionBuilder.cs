﻿using PoESkillTree.Computation.Common.Builders.Resolving;

namespace PoESkillTree.Computation.Common.Builders.Conditions
{
    /// <summary>
    /// Represents a condition.
    /// </summary>
    public interface IConditionBuilder : IResolvable<IConditionBuilder>
    {
        /// <summary>
        /// Returns a new condition that is satisfied if this condition and <paramref name="condition"/> are satisfied.
        /// </summary>
        IConditionBuilder And(IConditionBuilder condition);

        /// <summary>
        /// Returns a new condition that is satisfied if this condition or <paramref name="condition"/> is satisfied.
        /// </summary>
        IConditionBuilder Or(IConditionBuilder condition);

        /// <summary>
        /// Returns a new condition that is satisfied if this condition is not satisfied.
        /// </summary>
        IConditionBuilder Not { get; }

        /// <summary>
        /// Builds this condition into a stat converter and a value.
        /// <para>
        /// If the condition doesn't convert stats, the stat converter is the identity
        /// (must always be <see cref="PoESkillTree.Common.Utils.Funcs.Identity{T}"/>).
        /// </para>
        /// <para>
        /// If the condition doesn't apply to values, the value is always 1
        /// so it doesn't change the actual value when being multiplied with it. Converting boolean conditions
        /// from and to <c>NodeValue?</c> follows the rules of <see cref="ConditionalValue"/>.
        /// </para>
        /// </summary>
        (StatConverter statConverter, IValue value) Build(BuildParameters parameters);
    }
}