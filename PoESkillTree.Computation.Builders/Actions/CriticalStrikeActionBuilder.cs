﻿using PoESkillTree.Computation.Builders.Stats;
using PoESkillTree.Computation.Common.Builders.Actions;
using PoESkillTree.Computation.Common.Builders.Entities;
using PoESkillTree.Computation.Common.Builders.Stats;

namespace PoESkillTree.Computation.Builders.Actions
{
    internal class CriticalStrikeActionBuilder : ActionBuilder, ICriticalStrikeActionBuilder
    {
        public CriticalStrikeActionBuilder(IStatFactory statFactory, IEntityBuilder entity)
            : base(statFactory, CoreBuilder.Create("CriticalStrike"), entity)
        {
        }

        public IDamageRelatedStatBuilder Chance =>
            StatBuilderUtils.DamageRelatedFromIdentity(StatFactory, "CriticalStrike.Chance", typeof(double))
                .WithHits;

        public IDamageRelatedStatBuilder Multiplier =>
            StatBuilderUtils.DamageRelatedFromIdentity(StatFactory, "CriticalStrike.Multiplier", typeof(double),
                    canApplyToAilmentDamage: true);

        public IStatBuilder ExtraDamageTaken =>
            StatBuilderUtils.FromIdentity(StatFactory, "CriticalStrike.ExtraDamageTaken", typeof(int));
    }
}