using System;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Builders.Stats
{
    public static class StatBuilderUtils
    {
        public static IConditionBuilder ConditionFromIdentity(
            IStatFactory statFactory, string identity,
            ExplicitRegistrationType? explicitRegistrationType = null) =>
            FromIdentity(statFactory, identity, typeof(bool), explicitRegistrationType).IsSet;

        public static IStatBuilder FromIdentity(
            IStatFactory statFactory, string identity, Type dataType,
            ExplicitRegistrationType? explicitRegistrationType = null) =>
            new StatBuilder(statFactory,
                LeafCoreStatBuilder.FromIdentity(statFactory, identity, dataType, explicitRegistrationType));

        public static IDamageRelatedStatBuilder DamageRelatedFromIdentity(
            IStatFactory statFactory, string identity, Type dataType,
            bool canApplyToSkillDamage = false, bool canApplyToAilmentDamage = false) =>
            DamageRelatedStatBuilder.Create(statFactory,
                LeafCoreStatBuilder.FromIdentity(statFactory, identity, dataType),
                canApplyToSkillDamage, canApplyToAilmentDamage);
    }
}