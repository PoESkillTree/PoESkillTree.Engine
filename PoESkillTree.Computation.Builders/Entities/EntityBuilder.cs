﻿using System;
using System.Collections.Generic;
using PoESkillTree.Common.Utils.Extensions;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders.Entities;
using PoESkillTree.Computation.Common.Builders.Resolving;

namespace PoESkillTree.Computation.Builders.Entities
{
    public class EntityBuilder : IEntityBuilder
    {
        private readonly IReadOnlyCollection<Entity> _entities;

        public EntityBuilder(params Entity[] entities)
        {
            if (entities.IsEmpty())
                throw new ArgumentException("must not be empty", nameof(entities));
            _entities = entities;
        }

        public IEntityBuilder Resolve(ResolveContext context) => this;

        public IReadOnlyCollection<Entity> Build(Entity modifierSourceEntity) => _entities;
    }

    public class ModifierSourceEntityBuilder : IEntityBuilder
    {
        public IEntityBuilder Resolve(ResolveContext context) => this;

        public IReadOnlyCollection<Entity> Build(Entity modifierSourceEntity) => new[] { modifierSourceEntity };
    }
}