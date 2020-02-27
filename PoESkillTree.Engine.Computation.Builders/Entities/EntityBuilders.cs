using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Builders.Entities
{
    public class EntityBuilders : IEntityBuilders
    {
        private readonly IStatFactory _statFactory;

        public EntityBuilders(IStatFactory statFactory) => _statFactory = statFactory;

        public IEntityWithRarityBuilder Self => new ModifierSourceEntityWithRarityBuilder(_statFactory);
        public IHostileEntityBuilder OpponentsOfSelf => new ModifierSourceOpponentEntityBuilder(_statFactory);
        public IEntityBuilder MainOpponentOfSelf => new MainOpponentOfModifierSourceEntityBuilder();
        public IEntityBuilder Enemy => new EntityBuilder(Entity.Enemy);
        public IEntityBuilder Character => new EntityBuilder(Entity.Character);
        public ICountableEntityBuilder Ally => new ModifierSourceAllyEntityBuilder(_statFactory);
        public IEntityBuilder Totem => new ModifierSourceTotemEntityBuilder();
        public IEntityBuilder Minion => new ModifierSourceMinionEntityBuilder();
        public IEntityBuilder Any => EntityBuilder.AllEntities;
        public IEntityBuilder From(IEnumerable<Entity> entities) => new EntityBuilder(entities.ToArray());
    }
}