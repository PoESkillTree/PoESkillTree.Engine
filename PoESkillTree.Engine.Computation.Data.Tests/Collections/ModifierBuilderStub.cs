using System;
using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Forms;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;

namespace PoESkillTree.Engine.Computation.Data.Collections
{
    internal class ModifierBuilderStub : IModifierBuilder, IIntermediateModifier
    {
        internal IEnumerable<IConditionBuilder?>? Conditions { get; private set; }
        internal IEnumerable<IFormBuilder?>? Forms { get; private set; }
        internal IEnumerable<IStatBuilder?>? Stats { get; private set; }
        internal IEnumerable<IValueBuilder?>? Values { get; private set; }

        public IReadOnlyList<IntermediateModifierEntry> Entries => throw new InvalidOperationException();

#pragma warning disable 8613, 8766 // This class changes the interface's contract to simplify testing
        public StatConverter? StatConverter { get; private set; }
        public ValueConverter? ValueConverter { get; private set; }
#pragma warning restore

        public IModifierBuilder WithCondition(IConditionBuilder condition)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            if (Conditions != null)
                throw new InvalidOperationException();
            var ret = (ModifierBuilderStub) MemberwiseClone();
            ret.Conditions = new[] { condition };
            return ret;
        }

        public IModifierBuilder WithConditions(IReadOnlyList<IConditionBuilder?> conditions)
        {
            if (conditions == null)
                throw new ArgumentNullException(nameof(conditions));
            if (Conditions != null)
                throw new InvalidOperationException();
            var ret = (ModifierBuilderStub) MemberwiseClone();
            ret.Conditions = conditions;
            return ret;
        }

        public IModifierBuilder WithForm(IFormBuilder form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));
            if (Forms != null)
                throw new InvalidOperationException();
            var ret = (ModifierBuilderStub) MemberwiseClone();
            ret.Forms = new[] { form };
            return ret;
        }

        public IModifierBuilder WithForms(IReadOnlyList<IFormBuilder?> forms)
        {
            if (forms == null)
                throw new ArgumentNullException(nameof(forms));
            if (Forms != null)
                throw new InvalidOperationException();
            var ret = (ModifierBuilderStub) MemberwiseClone();
            ret.Forms = forms;
            return ret;
        }

        public IModifierBuilder WithStat(IStatBuilder stat)
        {
            if (stat == null)
                throw new ArgumentNullException(nameof(stat));
            if (Stats != null)
                throw new InvalidOperationException();
            var ret = (ModifierBuilderStub) MemberwiseClone();
            ret.Stats = new[] { stat };
            return ret;
        }

        public IModifierBuilder WithStatConverter(StatConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
            if (StatConverter != null)
                throw new InvalidOperationException();
            var ret = (ModifierBuilderStub) MemberwiseClone();
            ret.StatConverter = converter;
            return ret;
        }

        public IModifierBuilder WithStats(IReadOnlyList<IStatBuilder?> stats)
        {
            if (stats == null)
                throw new ArgumentNullException(nameof(stats));
            if (Stats != null)
                throw new InvalidOperationException();
            var ret = (ModifierBuilderStub) MemberwiseClone();
            ret.Stats = stats;
            return ret;
        }

        public IModifierBuilder WithValue(IValueBuilder value)
        {
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException(nameof(value));
            if (Values != null)
                throw new InvalidOperationException();
            var ret = (ModifierBuilderStub) MemberwiseClone();
            ret.Values = new[] { value };
            return ret;
        }

        public IModifierBuilder WithValueConverter(ValueConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
            if (ValueConverter != null)
                throw new InvalidOperationException();
            var ret = (ModifierBuilderStub) MemberwiseClone();
            ret.ValueConverter = converter;
            return ret;
        }

        public IModifierBuilder WithValues(IReadOnlyList<IValueBuilder?> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (Values != null)
                throw new InvalidOperationException();
            var ret = (ModifierBuilderStub) MemberwiseClone();
            ret.Values = values;
            return ret;
        }

        public IIntermediateModifier Build()
        {
            return this;
        }
    }
}