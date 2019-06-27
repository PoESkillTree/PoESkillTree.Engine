using System;
using JetBrains.Annotations;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;

namespace PoESkillTree.Engine.Computation.Data.Collections
{
    /// <inheritdoc />
    /// <summary>
    /// Collection of <see cref="Common.Data.MatcherData"/> for parsing properties that 
    /// allows collection initialization syntax for adding entries.
    /// </summary>
    /// <remarks>No property parsing happens yet, take this class with a grain of salt.</remarks>
    public class PropertyMatcherCollection : MatcherCollection
    {
        public PropertyMatcherCollection(IModifierBuilder modifierBuilder) : base(modifierBuilder)
        {
        }

        public void Add([RegexPattern] string regex)
        {
            Add(regex, ModifierBuilder);
        }

        public void Add([RegexPattern] string regex, IStatBuilder stat)
        {
            Add(regex, ModifierBuilder.WithStat(stat));
        }

        public void Add([RegexPattern] string regex, IStatBuilder stat, Func<ValueBuilder, ValueBuilder> converter)
        {
            var builder = ModifierBuilder
                .WithStat(stat)
                .WithValueConverter(converter.ToValueConverter());
            Add(regex, builder);
        }
    }
}