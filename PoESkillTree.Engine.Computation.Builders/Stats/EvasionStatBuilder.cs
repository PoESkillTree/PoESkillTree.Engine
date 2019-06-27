using System;
using System.Runtime.CompilerServices;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Builders.Stats
{
    internal class EvasionStatBuilder : StatBuilder, IEvasionStatBuilder
    {
        private const string Prefix = "Evasion";

        public EvasionStatBuilder(IStatFactory statFactory)
            : base(statFactory, LeafCoreStatBuilder.FromIdentity(statFactory, Prefix, typeof(uint)))
        {
        }

        public IStatBuilder Chance => ChanceAgainstProjectileAttacks.CombineWith(ChanceAgainstMeleeAttacks);

        public IStatBuilder ChanceAgainstProjectileAttacks => FromIdentity(typeof(uint));

        public IStatBuilder ChanceAgainstMeleeAttacks => FromIdentity(typeof(uint));

        private IStatBuilder FromIdentity(Type dataType, [CallerMemberName] string identity = null)
            => With(LeafCoreStatBuilder.FromIdentity(StatFactory, Prefix + "." + identity, dataType));
    }
}