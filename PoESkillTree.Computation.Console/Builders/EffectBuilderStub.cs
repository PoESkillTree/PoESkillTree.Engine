﻿using PoESkillTree.Computation.Parsing.Builders.Conditions;
using PoESkillTree.Computation.Parsing.Builders.Effects;
using PoESkillTree.Computation.Parsing.Builders.Entities;
using PoESkillTree.Computation.Parsing.Builders.Matching;
using PoESkillTree.Computation.Parsing.Builders.Stats;
using static PoESkillTree.Computation.Console.Builders.BuilderFactory;

namespace PoESkillTree.Computation.Console.Builders
{
    public abstract class EffectBuilderStub : BuilderStub, IEffectBuilder
    {
        private readonly Resolver<IEffectBuilder> _resolver;

        protected EffectBuilderStub(string stringRepresentation, Resolver<IEffectBuilder> resolver) 
            : base(stringRepresentation)
        {
            _resolver = resolver;
        }

        protected IEffectBuilder This => this;

        public IFlagStatBuilder On(IEntityBuilder target) =>
            CreateFlagStat(This, target, (o1, o2) => $"Apply {o1} to {o2}");

        public IStatBuilder ChanceOn(IEntityBuilder target) =>
            CreateStat(This, target, (o1, o2) => $"Chance to apply {o1} to {o2}");

        public IConditionBuilder IsOn(IEntityBuilder target) =>
            CreateCondition(This, target, (l, r) => $"{l} is applied to {r}");

        public IStatBuilder Duration =>
            CreateStat(This, o => $"{o} duration");

        public IEffectBuilder Resolve(ResolveContext context) =>
            _resolver(this, context);
    }


    public class EffectBuildersStub : IEffectBuilders
    {
        public IStunEffectBuilder Stun => new StunEffectBuilderStub();

        public IKnockbackEffectBuilder Knockback => new KnockbackEffectBuilderStub();

        public IAilmentBuilders Ailment => new AilmentBuildersStub();

        public IGroundEffectBuilders Ground => new GroundEffectBuildersStub();
    }


    public abstract class AvoidableEffectBuilderStub : EffectBuilderStub, IAvoidableEffectBuilder
    {
        protected AvoidableEffectBuilderStub(string stringRepresentation, 
            Resolver<IEffectBuilder> resolver) 
            : base(stringRepresentation, resolver)
        {
        }

        public IStatBuilder Avoidance => CreateStat(This, o => $"{o} avoidance");
    }


    public class KnockbackEffectBuilderStub : EffectBuilderStub, IKnockbackEffectBuilder
    {
        public KnockbackEffectBuilderStub() 
            : base("Knockback", (current, _) => current)
        {
        }

        public IStatBuilder Distance => CreateStat(This, o => $"{o} distance");
    }
}