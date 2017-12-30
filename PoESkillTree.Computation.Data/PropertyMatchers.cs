﻿using System.Collections.Generic;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;
using PoESkillTree.Computation.Parsing.Builders;
using PoESkillTree.Computation.Parsing.Builders.Matching;
using PoESkillTree.Computation.Parsing.Data;
using PoESkillTree.Computation.Parsing.ModifierBuilding;

namespace PoESkillTree.Computation.Data
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="IStatMatchers"/> implementation used to match properties of items and skills.
    /// </summary>
    /// <remarks>
    /// This is not complete at all, just some example properties. It isn't used yet anyway and how it will be used
    /// might be different from how it is implemented right now.
    /// </remarks>
    public class PropertyMatchers : StatMatchersBase
    {
        // "Elemental Damage: ..." needs to be replaced by up to three properties (one for each element)
        // before it gets here.

        private readonly IModifierBuilder _modifierBuilder;

        public PropertyMatchers(IBuilderFactories builderFactories,
            IMatchContexts matchContexts, IModifierBuilder modifierBuilder)
            : base(builderFactories, matchContexts)
        {
            _modifierBuilder = modifierBuilder;
        }

        protected override IEnumerable<MatcherData> CreateCollection() =>
            new PropertyMatcherCollection(_modifierBuilder, ValueFactory)
            {
                { "quality" }, // do nothing with it
                { "attacks per second", Skills.Speed },
                { "cast time", Skills.Speed, v => v.Invert },
                { "fire damage", Fire.Damage },
                { "damage effectiveness", Skills.DamageEffectiveness }
            };
    }
}