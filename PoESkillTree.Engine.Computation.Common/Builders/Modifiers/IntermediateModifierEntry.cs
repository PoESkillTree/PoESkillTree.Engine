using System;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Forms;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Common.Builders.Modifiers
{
    /// <summary>
    /// Entries of <see cref="IIntermediateModifier"/>. Implemented as an immutable class that allows changes
    /// through a fluent interface creating new instances on each method call. Since <see cref="IIntermediateModifier"/>
    /// is for partial modifiers, every property can be null.
    /// </summary>
    public class IntermediateModifierEntry : ValueObject
    {
        public IFormBuilder? Form { get; }

        public IStatBuilder? Stat { get; }

        public IValueBuilder? Value { get; }

        public IConditionBuilder? Condition { get; }

        public IntermediateModifierEntry()
        {
        }

        public IntermediateModifierEntry(
            IFormBuilder? form, IStatBuilder? stat, IValueBuilder? value, IConditionBuilder? condition)
        {
            Form = form;
            Stat = stat;
            Value = value;
            Condition = condition;
        }

        public IntermediateModifierEntry WithForm(IFormBuilder? form)
        {
            if (Form != null && form != null)
                throw new InvalidOperationException(nameof(WithForm) + " must not be called multiple times");
            return new IntermediateModifierEntry(form, Stat, Value, Condition);
        }

        public IntermediateModifierEntry WithStat(IStatBuilder? stat)
        {
            if (Stat != null && stat != null)
                throw new InvalidOperationException(nameof(WithStat) + " must not be called multiple times");
            return new IntermediateModifierEntry(Form, stat, Value, Condition);
        }

        public IntermediateModifierEntry WithValue(IValueBuilder? value)
        {
            if (Value != null && value != null)
                throw new InvalidOperationException(nameof(WithValue) + " must not be called multiple times");
            return new IntermediateModifierEntry(Form, Stat, value, Condition);
        }

        public IntermediateModifierEntry WithCondition(IConditionBuilder? condition)
        {
            if (Condition != null && condition != null)
                throw new InvalidOperationException(nameof(WithCondition) + " must not be called multiple times");
            return new IntermediateModifierEntry(Form, Stat, Value, condition);
        }

        protected override object ToTuple() => (Form, Stat, Value, Condition);
    }
}