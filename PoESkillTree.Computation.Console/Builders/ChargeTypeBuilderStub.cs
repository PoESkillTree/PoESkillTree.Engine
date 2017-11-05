﻿using PoESkillTree.Computation.Parsing.Builders.Charges;
using PoESkillTree.Computation.Parsing.Builders.Matching;
using PoESkillTree.Computation.Parsing.Builders.Stats;
using static PoESkillTree.Computation.Console.Builders.BuilderFactory;

namespace PoESkillTree.Computation.Console.Builders
{
    public class ChargeTypeBuilderStub : BuilderStub, IChargeTypeBuilder
    {
        private readonly Resolver<IChargeTypeBuilder> _resolver;

        public ChargeTypeBuilderStub(string stringRepresentation, 
            Resolver<IChargeTypeBuilder> resolver) 
            : base(stringRepresentation)
        {
            _resolver = resolver;
        }

        private IChargeTypeBuilder This => this;

        public IStatBuilder Amount => CreateStat(This, o => $"{o} amount");

        public IStatBuilder Duration => CreateStat(This, o => $"{o} duration");

        public IStatBuilder ChanceToGain => CreateStat(This, o => $"{o} chance to gain");

        public IChargeTypeBuilder Resolve(ResolveContext context) =>
            _resolver(this, context);
    }


    public class ChargeTypeBuildersStub : IChargeTypeBuilders
    {
        private static IChargeTypeBuilder Create(string stringRepresentation) =>
            new ChargeTypeBuilderStub(stringRepresentation, (current, _) => current);

        public IChargeTypeBuilder Endurance => Create("Endurance Charge");

        public IChargeTypeBuilder Frenzy => Create("Frenzy Charge");

        public IChargeTypeBuilder Power => Create("Power Charge");
    }
}