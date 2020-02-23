using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Builders.Conditions;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common;
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

        public IConditionBuilder IsRare =>
            new ValueConditionBuilder(ps =>
                StatBuilderUtils.FromIdentity(_statFactory, "IsRare", typeof(bool), ExplicitRegistrationTypes.UserSpecifiedValue(false))
                    .Value.Build(ps));

        public IConditionBuilder IsUnique =>
            new ValueConditionBuilder(ps =>
                StatBuilderUtils.FromIdentity(_statFactory, "IsUnique", typeof(bool), ExplicitRegistrationTypes.UserSpecifiedValue(false))
                    .Value.Build(ps));

        public IConditionBuilder IsRareOrUnique => IsRare.Or(IsUnique);
    }
}