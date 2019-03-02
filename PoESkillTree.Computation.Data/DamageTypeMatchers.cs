﻿using System.Collections.Generic;
using PoESkillTree.Computation.Common.Builders.Damage;
using PoESkillTree.Computation.Common.Data;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;

namespace PoESkillTree.Computation.Data
{
    /// <summary>
    /// <see cref="IReferencedMatchers"/> implementation for <see cref="IDamageTypeBuilder"/>s.
    /// </summary>
    public class DamageTypeMatchers : ReferencedMatchersBase<IDamageTypeBuilder>
    {
        private readonly IDamageTypeBuilders _damageTypeBuilders;

        public DamageTypeMatchers(IDamageTypeBuilders damageTypeBuilders)
        {
            _damageTypeBuilders = damageTypeBuilders;
        }

        private IDamageTypeBuilder Physical => _damageTypeBuilders.Physical;
        private IDamageTypeBuilder Fire => _damageTypeBuilders.Fire;
        private IDamageTypeBuilder Lightning => _damageTypeBuilders.Lightning;
        private IDamageTypeBuilder Cold => _damageTypeBuilders.Cold;
        private IDamageTypeBuilder Chaos => _damageTypeBuilders.Chaos;

        protected override IReadOnlyList<ReferencedMatcherData> CreateCollection() =>
            new ReferencedMatcherCollection<IDamageTypeBuilder>
            {
                { "physical", Physical },
                { "fire", Fire },
                { "lightning", Lightning },
                { "cold", Cold },
                { "chaos", Chaos },
                // combinations
                { "elemental", Fire.And(Lightning).And(Cold) },
                { "physical, cold and lightning", Physical.And(Cold).And(Lightning) },
                { "physical and fire", Physical.And(Fire) },
                // inverse
                { "non-fire", Fire.Invert },
                { "non-chaos", Chaos.Invert },
            };
    }
}