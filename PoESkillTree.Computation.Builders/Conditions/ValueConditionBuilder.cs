﻿using System;
using System.Linq.Expressions;
using PoESkillTree.Common.Utils;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Resolving;
using PoESkillTree.Computation.Common.Builders.Values;

namespace PoESkillTree.Computation.Builders.Conditions
{
    public class ValueConditionBuilder : IConditionBuilder
    {
        private readonly Func<Entity, IValue> _buildValue;
        private readonly Func<ResolveContext, IConditionBuilder> _resolve;

        public ValueConditionBuilder(bool value) : this(new ConditionalValue(value))
        {
        }

        private ValueConditionBuilder(IValue value) : this(_ => value)
        {
        }

        private ValueConditionBuilder(Func<Entity, IValue> buildValue)
        {
            _buildValue = buildValue;
            _resolve = _ => this;
        }

        private ValueConditionBuilder(
            Func<Entity, IValue> buildValue, Func<ResolveContext, Func<Entity, IValue>> resolve)
        {
            _buildValue = buildValue;
            _resolve = c => new ValueConditionBuilder(resolve(c));
        }

        public IConditionBuilder Resolve(ResolveContext context) => _resolve(context);

        public IConditionBuilder And(IConditionBuilder condition) =>
            new AndCompositeConditionBuilder(this, condition);

        public IConditionBuilder Or(IConditionBuilder condition) =>
            new OrCompositeConditionBuilder(this, condition);

        public IConditionBuilder Not => Create(this, b => !b);

        public (StatConverter statConverter, IValue value) Build(Entity modifierSourceEntity) =>
            (Funcs.Identity, _buildValue(modifierSourceEntity));
        
        // TODO Only here for compatibility with stubs in Console. Remove once those are removed.
        public override string ToString() => Build(Entity.Character).value.ToString();


        private static IConditionBuilder Create(
            ValueConditionBuilder operand,
            Expression<Func<bool, bool>> calculate) =>
            new ValueConditionBuilder(
                entity => Build(entity, operand, calculate),
                c => (entity => Build(entity, operand.Resolve(c), calculate)));

        public static IConditionBuilder Create(
            IValueBuilder operand,
            Expression<Func<NodeValue?, bool>> calculate) =>
            new ValueConditionBuilder(
                entity => Build(entity, operand, calculate),
                c => (entity => Build(entity, operand.Resolve(c), calculate)));

        public static IConditionBuilder Create(
            IValueBuilder operand1, IValueBuilder operand2,
            Expression<Func<NodeValue?, NodeValue?, bool>> calculate) =>
            new ValueConditionBuilder(
                entity => Build(entity, operand1, operand2, calculate),
                c => (entity => Build(entity, operand1.Resolve(c), operand2.Resolve(c), calculate)));

        private static IValue Build(
            Entity modifierSourceEntity,
            IConditionBuilder operand,
            Expression<Func<bool, bool>> calculate)
        {
            var builtOperand = operand.Build(modifierSourceEntity).value;
            var func = calculate.Compile();
            var identity = calculate.ToString(builtOperand);
            return new ConditionalValue(c => func(builtOperand.Calculate(c).IsTrue()), identity);
        }

        private static IValue Build(
            Entity modifierSourceEntity,
            IValueBuilder operand,
            Expression<Func<NodeValue?, bool>> calculate)
        {
            var builtOperand = operand.Build(modifierSourceEntity);
            var func = calculate.Compile();
            var identity = calculate.ToString(builtOperand);
            return new ConditionalValue(c => func(builtOperand.Calculate(c)), identity);
        }

        private static IValue Build(
            Entity modifierSourceEntity,
            IValueBuilder operand1, IValueBuilder operand2,
            Expression<Func<NodeValue?, NodeValue?, bool>> calculate)
        {
            var o1 = operand1.Build(modifierSourceEntity);
            var o2 = operand2.Build(modifierSourceEntity);
            var func = calculate.Compile();
            var identity = calculate.ToString(o1, o2);
            return new ConditionalValue(c => func(o1.Calculate(c), o2.Calculate(c)), identity);
        }
    }
}