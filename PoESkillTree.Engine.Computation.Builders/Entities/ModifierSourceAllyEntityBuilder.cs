using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Builders.Entities
{
    public class ModifierSourceAllyEntityBuilder : ICountableEntityBuilder
    {
        private readonly IStatFactory _statFactory;

        public ModifierSourceAllyEntityBuilder(IStatFactory statFactory) =>
            _statFactory = statFactory;

        public ValueBuilder CountNearby
            => StatBuilderUtils.FromIdentity(_statFactory, "Ally.CountNearby", typeof(uint),
                ExplicitRegistrationTypes.UserSpecifiedValue(0)).Value;

        public IReadOnlyCollection<Entity> Build(Entity modifierSourceEntity) => modifierSourceEntity.Allies().ToList();
    }
}