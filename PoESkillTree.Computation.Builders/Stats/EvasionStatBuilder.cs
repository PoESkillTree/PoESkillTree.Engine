﻿using System;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders.Stats;

namespace PoESkillTree.Computation.Builders.Stats
{
    internal class EvasionStatBuilder : StatBuilder, IEvasionStatBuilder
    {
        private const string Prefix = "Evasion";

        public EvasionStatBuilder(IStatFactory statFactory)
            : base(statFactory, LeafCoreStatBuilder.FromIdentity(statFactory, Prefix, typeof(uint)))
        {
        }

        public IStatBuilder Chance => ChanceAgainstProjectileAttacks.CombineWith(ChanceAgainstMeleeAttacks);

        public IStatBuilder ChanceAgainstProjectileAttacks =>
            FromIdentity($"{Prefix} chance against projectile attacks", typeof(uint));

        public IStatBuilder ChanceAgainstMeleeAttacks =>
            FromIdentity($"{Prefix} chance against melee attacks", typeof(uint));

        private IStatBuilder FromIdentity(
            string identity, Type dataType, ExplicitRegistrationType explicitRegistrationType = null) =>
            With(LeafCoreStatBuilder.FromIdentity(StatFactory, identity, dataType, explicitRegistrationType));
    }
}