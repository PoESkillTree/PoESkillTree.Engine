using System.Collections;
using System.Collections.Generic;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Forms;
using PoESkillTree.Computation.Common.Builders.Modifiers;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.Computation.Common.Builders.Values;

namespace PoESkillTree.Computation.Data.Collections
{
    /// <summary>
    /// Collection of <see cref="IIntermediateModifier"/> that allows collection initialization syntax for adding
    /// entries.
    /// </summary>
    public class GivenStatCollection : IEnumerable<IIntermediateModifier>
    {
        private readonly IModifierBuilder _modifierBuilder;
        private readonly IValueBuilders _valueFactory;

        private readonly List<IIntermediateModifier> _data = new List<IIntermediateModifier>();

        public GivenStatCollection(IModifierBuilder modifierBuilder, IValueBuilders valueFactory)
        {
            _modifierBuilder = modifierBuilder;
            _valueFactory = valueFactory;
        }

        public IEnumerator<IIntermediateModifier> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(IFormBuilder form, IStatBuilder stat, double value, IConditionBuilder condition = null)
            => Add(form, stat, _valueFactory.Create(value), condition);

        public void Add(IFormBuilder form, IStatBuilder stat, IValueBuilder value, IConditionBuilder condition = null)
        {
            var builder = _modifierBuilder
                .WithForm(form)
                .WithStat(stat)
                .WithValue(value);
            if (condition != null)
                builder = builder.WithCondition(condition);
            _data.Add(builder.Build());
        }
    }
}