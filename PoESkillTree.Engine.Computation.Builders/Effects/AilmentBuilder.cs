using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Effects;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Skills;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Builders.Effects
{
    public class AilmentBuilder : AvoidableEffectBuilder, IAilmentBuilder
    {
        private readonly ICoreBuilder<Ailment> _ailment;

        public AilmentBuilder(IStatFactory statFactory, ICoreBuilder<Ailment> ailment)
            : base(statFactory, CoreBuilder.UnaryOperation(ailment, a => a.GetName()!))
        {
            _ailment = ailment;
        }

        public override IEffectBuilder Resolve(ResolveContext context) =>
            new AilmentBuilder(StatFactory, _ailment.Resolve(context));

        protected override bool OnIsUserSpecified => true;

        public IStatBuilder InstancesOn(IEntityBuilder target) =>
            FromIdentity("InstanceCount", typeof(uint)).For(target);

        public IStatBuilder Source(IDamageTypeBuilder type)
        {
            var inner = CoreStatBuilderFromIdentity("HasSource", typeof(bool));
            var coreStat = new ParametrisedCoreStatBuilder<IKeywordBuilder>(inner, type,
                (ps, k, s) => ConcretizeSourceStat(((IDamageTypeBuilder) k).BuildDamageTypes(ps), s));
            return new StatBuilder(StatFactory, coreStat);
        }

        public IStatBuilder CriticalStrikesAlwaysInflict
            => FromIdentity("CriticalStrikesAlwaysInflict", typeof(bool));
        public IStatBuilder ChanceToRemove => FromIdentity("ChanceToRemove", typeof(uint));

        public IStatBuilder TickRateModifier => FromIdentity("TickRateModifier", typeof(double));

        private IEnumerable<IStat> ConcretizeSourceStat(IReadOnlyList<DamageType> types, IStat stat) =>
            types.Select(t => StatFactory.CopyWithSuffix(stat, t.ToString(), typeof(bool)));

        public new Ailment Build(BuildParameters parameters) => _ailment.Build(parameters);
    }
}