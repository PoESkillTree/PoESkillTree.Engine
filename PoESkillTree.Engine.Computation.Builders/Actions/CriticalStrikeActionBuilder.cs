using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Actions;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Builders.Actions
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

        public IDamageRelatedStatBuilder BaseChance =>
            StatBuilderUtils.DamageRelatedFromIdentity(StatFactory, "CriticalStrike.BaseChance", typeof(double))
                .WithHits;

        public IDamageRelatedStatBuilder Multiplier =>
            StatBuilderUtils.DamageRelatedFromIdentity(StatFactory, "CriticalStrike.Multiplier", typeof(double),
                    canApplyToAilmentDamage: true);

        public IStatBuilder ExtraDamageTaken =>
            StatBuilderUtils.FromIdentity(StatFactory, "CriticalStrike.ExtraDamageTaken", typeof(int));
    }
}