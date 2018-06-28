﻿using System;
using System.Collections.Generic;
using PoESkillTree.Computation.Builders.Conditions;
using PoESkillTree.Computation.Builders.Values;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Entities;
using PoESkillTree.Computation.Common.Builders.Resolving;
using PoESkillTree.Computation.Common.Builders.Skills;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.Computation.Common.Builders.Values;

namespace PoESkillTree.Computation.Builders.Stats
{
    public class StatBuilder : IFlagStatBuilder
    {
        protected IStatFactory StatFactory { get; }
        protected ICoreStatBuilder CoreStatBuilder { get; }

        public StatBuilder(IStatFactory statFactory, ICoreStatBuilder coreStatBuilder)
        {
            StatFactory = statFactory;
            CoreStatBuilder = coreStatBuilder;
        }

        protected IFlagStatBuilder FromIdentity(string identity, Type dataType, bool isExplicitlyRegistered = false) =>
            With(LeafCoreStatBuilder.FromIdentity(StatFactory, identity, dataType, isExplicitlyRegistered));

        protected virtual IFlagStatBuilder With(ICoreStatBuilder coreStatBuilder) =>
            new StatBuilder(StatFactory, coreStatBuilder);

        protected virtual IStatBuilder WithStatConverter(Func<IStat, IStat> statConverter) =>
            With(CoreStatBuilder.WithStatConverter(statConverter));

        public virtual IStatBuilder Resolve(ResolveContext context) => With(CoreStatBuilder.Resolve(context));

        public IStatBuilder Minimum => WithStatConverter(s => s.Minimum);
        public IStatBuilder Maximum => WithStatConverter(s => s.Maximum);

        public ValueBuilder Value =>
            new ValueBuilder(new ValueBuilderImpl(CoreStatBuilder.BuildValue, c => Resolve(c).Value));

        public IConditionBuilder IsSet =>
            ValueConditionBuilder.Create(Value, v => v.IsTrue());

        public IStatBuilder ConvertTo(IStatBuilder stat) =>
            new StatBuilder(StatFactory,
                new ConversionStatBuilder(StatFactory.ConvertTo, CoreStatBuilder, new StatBuilderAdapter(stat)));

        public IStatBuilder GainAs(IStatBuilder stat) =>
            new StatBuilder(StatFactory,
                new ConversionStatBuilder(StatFactory.GainAs, CoreStatBuilder, new StatBuilderAdapter(stat)));

        public IStatBuilder ChanceToDouble => WithStatConverter(StatFactory.ChanceToDouble);

        public IStatBuilder For(IEntityBuilder entity) => With(CoreStatBuilder.WithEntity(entity));

        public IStatBuilder With(IKeywordBuilder keyword) => WithCondition(KeywordCondition(keyword));

        public IStatBuilder NotWith(IKeywordBuilder keyword) => WithCondition(KeywordCondition(keyword).Not);

        private IConditionBuilder KeywordCondition(IKeywordBuilder keyword) =>
            ValueConditionBuilder.Create(
                (p, k) => StatFactory.ActiveSkillHasKeyword(p.ModifierSourceEntity, k.Build()),
                keyword);

        public IStatBuilder WithCondition(IConditionBuilder condition) =>
            With(new StatBuilderAdapter(this, condition));

        public IStatBuilder CombineWith(IStatBuilder other) =>
            With(new CompositeCoreStatBuilder(CoreStatBuilder, new StatBuilderAdapter(other)));

        public virtual IEnumerable<StatBuilderResult> Build(
            BuildParameters parameters, ModifierSource originalModifierSource) =>
            CoreStatBuilder.Build(parameters, originalModifierSource);
    }
}