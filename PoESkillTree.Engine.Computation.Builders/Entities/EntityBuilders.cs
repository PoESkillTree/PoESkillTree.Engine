using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Builders.Entities
{
    public class EntityBuilders : IEntityBuilders
    {
        private readonly IStatFactory _statFactory;

        public EntityBuilders(IStatFactory statFactory) => _statFactory = statFactory;

        public IEntityBuilder Self => new ModifierSourceEntityBuilder();
        public IEntityBuilder OpponentOfSelf => new ModifierSourceOpponentEntityBuilder();
        public IEnemyBuilder Enemy => new EnemyBuilder(_statFactory);
        public IEntityBuilder Character => new EntityBuilder(Entity.Character);
        public ICountableEntityBuilder Ally => new AllyBuilder(_statFactory);
        public IEntityBuilder Totem => new EntityBuilder(Entity.Totem);
        public IEntityBuilder Minion => new EntityBuilder(Entity.Minion);
        public IEntityBuilder Any => EntityBuilder.AllEntities;
        public IEntityBuilder From(IEnumerable<Entity> entities) => new EntityBuilder(entities.ToArray());

        private class EnemyBuilder : EntityBuilder, IEnemyBuilder
        {
            private readonly IStatFactory _statFactory;

            public EnemyBuilder(IStatFactory statFactory) : base(Entity.Enemy) =>
                _statFactory = statFactory;

            public IConditionBuilder IsNearby => StatBuilderUtils.ConditionFromIdentity(_statFactory, "Enemy.IsNearby",
                ExplicitRegistrationTypes.UserSpecifiedValue(false));

            public ValueBuilder CountNearby
                => StatBuilderUtils.FromIdentity(_statFactory, "Enemy.CountNearby", typeof(uint),
                    ExplicitRegistrationTypes.UserSpecifiedValue(0)).Value;

            public ValueBuilder CountRareOrUniqueNearby
                => StatBuilderUtils.FromIdentity(_statFactory, "Enemy.CountRareOrUniqueNearby", typeof(uint),
                    ExplicitRegistrationTypes.UserSpecifiedValue(0)).Value;

            public ValueBuilder Distance
                => StatBuilderUtils.FromIdentity(_statFactory, "Enemy.Distance", typeof(uint),
                    ExplicitRegistrationTypes.UserSpecifiedValue(40)).Value;

            public IConditionBuilder IsRare => StatBuilderUtils.ConditionFromIdentity(_statFactory, "Enemy.IsRare",
                ExplicitRegistrationTypes.UserSpecifiedValue(false));

            public IConditionBuilder IsUnique => StatBuilderUtils.ConditionFromIdentity(_statFactory, "Enemy.IsUnique",
                ExplicitRegistrationTypes.UserSpecifiedValue(false));

            public IConditionBuilder IsRareOrUnique => IsRare.Or(IsUnique);

            public IConditionBuilder IsMoving => StatBuilderUtils.ConditionFromIdentity(_statFactory, "Enemy.IsMoving",
                ExplicitRegistrationTypes.UserSpecifiedValue(false));
        }

        private class AllyBuilder : EntityBuilder, ICountableEntityBuilder
        {
            private readonly IStatFactory _statFactory;

            public AllyBuilder(IStatFactory statFactory) : base(Entity.Minion, Entity.Totem) =>
                _statFactory = statFactory;

            public ValueBuilder CountNearby
                => StatBuilderUtils.FromIdentity(_statFactory, "Ally.CountNearby", typeof(uint),
                    ExplicitRegistrationTypes.UserSpecifiedValue(0)).Value;
        }
    }
}