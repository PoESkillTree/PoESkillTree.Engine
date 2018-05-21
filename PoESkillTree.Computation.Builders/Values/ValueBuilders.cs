﻿using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Common.Utils;
using PoESkillTree.Common.Utils.Extensions;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Values;
using PoESkillTree.Computation.Common.Parsing;

namespace PoESkillTree.Computation.Builders.Values
{
    public class ValueBuilders : IValueBuilders
    {
        public IThenBuilder If(IConditionBuilder condition) => new ThenBuilder(condition);

        public IValueBuilder Create(double value) => new ValueBuilderImpl(value);

        public IValueBuilder FromMinAndMax(IValueBuilder minimumValue, IValueBuilder maximumValue)
        {
            return ValueBuilderImpl.Create(minimumValue, maximumValue, Calculate);

            NodeValue? Calculate(NodeValue? min, NodeValue? max, IValueCalculationContext context) =>
                min.HasValue && max.HasValue
                    ? new NodeValue(min.Value.Minimum, max.Value.Maximum)
                    : (NodeValue?) null;
        }


        private class ThenBuilder : IThenBuilder
        {
            private readonly IReadOnlyList<(IConditionBuilder condition, IValueBuilder value)> _conditionValuePairs;
            private readonly IConditionBuilder _branchCondition;

            public ThenBuilder(IConditionBuilder branchCondition)
            {
                _conditionValuePairs = new (IConditionBuilder condition, IValueBuilder value)[0];
                _branchCondition = branchCondition;
            }

            public ThenBuilder(
                IReadOnlyList<(IConditionBuilder condition, IValueBuilder value)> conditionValuePairs,
                IConditionBuilder branchCondition)
            {
                _conditionValuePairs = conditionValuePairs;
                _branchCondition = branchCondition;
            }

            public IConditionalValueBuilder Then(IValueBuilder value) =>
                new ConditionalValueBuilder(_conditionValuePairs.Append((_branchCondition, value)).ToList());

            public IConditionalValueBuilder Then(double value) => Then(new ValueBuilderImpl(value));
        }


        private class ConditionalValueBuilder : IConditionalValueBuilder
        {
            private readonly IReadOnlyList<(IConditionBuilder condition, IValueBuilder value)> _conditionValuePairs;

            public ConditionalValueBuilder(
                IReadOnlyList<(IConditionBuilder condition, IValueBuilder value)> conditionValuePairs)
            {
                _conditionValuePairs = conditionValuePairs;
            }

            public IThenBuilder ElseIf(IConditionBuilder condition) =>
                new ThenBuilder(_conditionValuePairs, condition);

            public ValueBuilder Else(IValueBuilder value)
            {
                return new ValueBuilder(new ValueBuilderImpl(Build));

                IValue Build()
                {
                    var pairs = new List<(IValue condition, IValue value)>();
                    foreach (var (c, v) in _conditionValuePairs)
                    {
                        var condition = c.Build();
                        if (condition.statConverter != Funcs.Identity)
                        {
                            throw new ParseException(
                                $"Conditions for building conditional values must be value conditions. {c}");
                        }
                        pairs.Add((condition.value, v.Build()));
                    }
                    return new BranchingValue(pairs, value.Build());
                }
            }

            public ValueBuilder Else(double value) => Else(new ValueBuilderImpl(0));
        }


        private class BranchingValue : IValue
        {
            private readonly IReadOnlyList<(IValue condition, IValue value)> _conditionValuePairs;
            private readonly IValue _elseValue;

            public BranchingValue(
                IReadOnlyList<(IValue condition, IValue value)> conditionValuePairs, IValue elseValue)
            {
                _conditionValuePairs = conditionValuePairs;
                _elseValue = elseValue;
            }

            public NodeValue? Calculate(IValueCalculationContext context)
            {
                foreach (var (c, v) in _conditionValuePairs)
                {
                    if (c.Calculate(context).IsTrue())
                    {
                        return v.Calculate(context);
                    }
                }
                return _elseValue.Calculate(context);
            }
        }
    }
}