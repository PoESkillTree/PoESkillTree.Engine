﻿using System;
using PoESkillTree.Computation.Builders.Values;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Resolving;
using PoESkillTree.Computation.Common.Builders.Values;
using static PoESkillTree.Computation.Console.Builders.BuilderFactory;

namespace PoESkillTree.Computation.Console.Builders
{
    public class ValueBuilderStub : BuilderStub, IValueBuilder
    {
        private readonly Resolver<IValueBuilder> _resolver;

        public ValueBuilderStub(string stringRepresentation, Resolver<IValueBuilder> resolver)
            : base(stringRepresentation)
        {
            _resolver = resolver;
        }

        private IValueBuilder This => this;

        public IValueBuilder MaximumOnly =>
            CreateValue(This, o => $"{o} (maximum value only)");

        public IConditionBuilder Eq(IValueBuilder other) =>
            CreateCondition(This, other, (l, r) => $"({l} == {r})");

        public IConditionBuilder GreaterThan(IValueBuilder other) =>
            CreateCondition(This, other, (l, r) => $"({l} > {r})");

        public IValueBuilder Add(IValueBuilder other) =>
            CreateValue(This, other, (l, r) => $"({l} + {r})");

        public IValueBuilder Multiply(IValueBuilder other) =>
            CreateValue(This, other, (l, r) => $"({l} * {r})");

        public IValueBuilder DivideBy(IValueBuilder divisor) =>
            CreateValue(This, divisor, (l, r) => $"({l} / {r})");

        public IValueBuilder If(IValue condition) =>
            CreateValue(This, (IValueBuilder) new ValueBuilderImpl(condition), (l, r) => $"{l} if {r} else null");

        public IValueBuilder Select(Func<double, double> selector, Func<IValue, string> identity) =>
            CreateValue(This, o => identity(o.Build(default)));

        public IValueBuilder Create(double value) =>
            new ValueBuilderImpl(value);

        public IValueBuilder Resolve(ResolveContext context) =>
            _resolver(this, context);

        public IValue Build(BuildParameters parameters) => new ValueStub(this);
    }


    public class ValueStub : BuilderStub, IValue
    {
        public ValueStub(BuilderStub builderStub) : base(builderStub)
        {
        }

        public NodeValue? Calculate(IValueCalculationContext valueCalculationContext) => 
            throw new NotSupportedException();
    }
}