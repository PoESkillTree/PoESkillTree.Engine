using System;
using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Builders.Entities
{
    public class ModifierSourceOpponentEntityBuilder : IEntityBuilder
    {
        public IReadOnlyCollection<Entity> Build(Entity modifierSourceEntity)
        {
            switch (modifierSourceEntity)
            {
                case Entity.Character:
                case Entity.Totem:
                case Entity.Minion:
                    return new[] { Entity.Enemy };
                case Entity.Enemy:
                    return new[] { Entity.Character, Entity.Totem, Entity.Minion };
                default:
                    throw new ArgumentOutOfRangeException(nameof(modifierSourceEntity), modifierSourceEntity, null);
            }
        }
    }
}