using JetBrains.Annotations;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;

namespace PoESkillTree.Engine.Computation.Data.Collections
{
    /// <inheritdoc />
    /// <summary>
    /// Collection of <see cref="Common.Data.MatcherData"/>, with 
    /// <see cref="IIntermediateModifier"/>s consisting only of a stat converter, that allows collection 
    /// initialization syntax for adding entries.
    /// </summary>
    public class StatManipulatorMatcherCollection : MatcherCollection
    {
        public StatManipulatorMatcherCollection(IModifierBuilder modifierBuilder) : base(modifierBuilder)
        {
        }

        /// <summary>
        /// Adds a matcher applying a stat converter.
        /// </summary>
        public void Add(
            [RegexPattern] string regex,
            StatConverter manipulateStat,
            string substitution = "")
        {
            var builder = ModifierBuilder
                .WithStatConverter(manipulateStat);
            Add(regex, builder, substitution);
        }
    }
}