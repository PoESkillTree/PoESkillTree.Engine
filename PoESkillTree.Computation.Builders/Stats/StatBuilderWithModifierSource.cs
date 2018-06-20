﻿using System;
using System.Collections.Generic;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Entities;
using PoESkillTree.Computation.Common.Builders.Resolving;
using PoESkillTree.Computation.Common.Builders.Stats;

namespace PoESkillTree.Computation.Builders.Stats
{
    public class StatBuilderWithModifierSource : ICoreStatBuilder
    {
        private readonly ICoreStatBuilder _statBuilder;
        private readonly ModifierSource _modifierSource;

        public StatBuilderWithModifierSource(ICoreStatBuilder statBuilder, ModifierSource modifierSource)
        {
            _statBuilder = statBuilder;
            _modifierSource = modifierSource;
        }

        private ICoreStatBuilder Select(Func<ICoreStatBuilder, ICoreStatBuilder> selector) =>
            new StatBuilderWithModifierSource(selector(_statBuilder), _modifierSource);

        public ICoreStatBuilder Resolve(ResolveContext context) =>
            Select(b => b.Resolve(context));

        public ICoreStatBuilder WithEntity(IEntityBuilder entityBuilder) =>
            Select(b => b.WithEntity(entityBuilder));

        public ICoreStatBuilder WithStatConverter(Func<IStat, IStat> statConverter) =>
            Select(b => b.WithStatConverter(statConverter));

        public IValue BuildValue(BuildParameters parameters) =>
            _statBuilder.BuildValue(parameters);

        public IEnumerable<StatBuilderResult>
            Build(BuildParameters parameters, ModifierSource originalModifierSource) =>
            _statBuilder.Build(parameters, _modifierSource);
    }
}