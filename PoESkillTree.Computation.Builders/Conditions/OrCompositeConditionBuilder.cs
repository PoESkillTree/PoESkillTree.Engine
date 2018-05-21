﻿using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Common.Utils.Extensions;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Resolving;

namespace PoESkillTree.Computation.Builders.Conditions
{
    public class OrCompositeConditionBuilder : IConditionBuilder
    {
        public OrCompositeConditionBuilder(params IConditionBuilder[] conditions)
        {
            Conditions = conditions;
        }

        public OrCompositeConditionBuilder(IReadOnlyList<IConditionBuilder> conditions)
        {
            Conditions = conditions;
        }

        public IReadOnlyList<IConditionBuilder> Conditions { get; }

        public IConditionBuilder Resolve(ResolveContext context) =>
            new OrCompositeConditionBuilder(Conditions.Select(c => c.Resolve(context)).ToList());

        public IConditionBuilder And(IConditionBuilder condition) =>
            new AndCompositeConditionBuilder(this, condition);

        public IConditionBuilder Or(IConditionBuilder condition) =>
            new OrCompositeConditionBuilder(Conditions.Append(condition).ToList());

        public IConditionBuilder Not =>
            new AndCompositeConditionBuilder(Conditions.Select(c => c.Not).ToList());

        public (StatConverter statConverter, IValue value) Build()
        {
            var builtConditions = Conditions.Select(c => c.Build()).ToList();
            return (_ => throw new NotImplementedException(), new ConditionalValue(Calculate));

            bool Calculate(IValueCalculationContext context) =>
                builtConditions
                    .Select(t => t.value.Calculate(context))
                    .Any(ConditionalValue.IsTrue);
        }

        public override bool Equals(object obj) =>
            (obj == this) || (obj is OrCompositeConditionBuilder other && Equals(other));

        private bool Equals(OrCompositeConditionBuilder other) =>
            Conditions.SequenceEqual(other.Conditions);

        public override int GetHashCode() =>
            Conditions.SequenceHash();
    }
}