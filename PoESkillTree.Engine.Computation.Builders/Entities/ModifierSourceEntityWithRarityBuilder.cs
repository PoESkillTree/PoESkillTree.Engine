using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PoESkillTree.Engine.Computation.Builders.Conditions;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Builders.Entities
{
    public class ModifierSourceEntityWithRarityBuilder : IEntityWithRarityBuilder
    {
        private readonly IStatFactory _statFactory;

        public ModifierSourceEntityWithRarityBuilder(IStatFactory statFactory)
        {
            _statFactory = statFactory;
        }

        public IReadOnlyCollection<Entity> Build(Entity modifierSourceEntity) => new[] {modifierSourceEntity};

        public IConditionBuilder IsRareOrUnique => IsRare.Or(IsUnique);

        public IConditionBuilder IsRare => new ValueConditionBuilder(ps => BuildFalseExceptForEnemyValue(ps));

        public IConditionBuilder IsUnique => new ValueConditionBuilder(ps => BuildFalseExceptForEnemyValue(ps));

        private IValue BuildFalseExceptForEnemyValue(BuildParameters ps, [CallerMemberName] string identity = "") =>
            ps.ModifierSourceEntity == Entity.Enemy ? CreateValue(ps, identity) : new Constant(false);

        private IValue CreateValue(BuildParameters ps, string identity) =>
            StatBuilderUtils.FromIdentity(_statFactory, identity, typeof(bool), ExplicitRegistrationTypes.UserSpecifiedValue(false))
                .Value.Build(ps);
    }
}