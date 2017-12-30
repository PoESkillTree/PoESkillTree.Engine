using JetBrains.Annotations;
using PoESkillTree.Computation.Parsing.Builders.Forms;
using PoESkillTree.Computation.Parsing.Builders.Values;
using PoESkillTree.Computation.Parsing.ModifierBuilding;

namespace PoESkillTree.Computation.Data.Collections
{
    /// <inheritdoc />
    /// <summary>
    /// Collection of <see cref="PoESkillTree.Computation.Parsing.Data.MatcherData"/>, with 
    /// <see cref="IIntermediateModifier"/>s consisting only of a form and a value, that allows collection 
    /// initialization syntax for adding entries.
    /// </summary>
    public class FormMatcherCollection : MatcherCollection
    {
        private readonly IValueBuilders _valueFactory;

        public FormMatcherCollection(IModifierBuilder modifierBuilder, IValueBuilders valueFactory)
            : base(modifierBuilder)
        {
            _valueFactory = valueFactory;
        }

        public void Add([RegexPattern] string regex, IFormBuilder form, double value)
        {
            var builder = ModifierBuilder
                .WithForm(form)
                .WithValue(_valueFactory.Create(value));
            Add(regex, builder);
        }

        public void Add([RegexPattern] string regex, IFormBuilder form, IValueBuilder value)
        {
            var builder = ModifierBuilder
                .WithForm(form)
                .WithValue(value);
            Add(regex, builder);
        }
    }
}