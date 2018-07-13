﻿using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Computation.Builders.Entities;
using PoESkillTree.Computation.Builders.Stats;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders.Damage;
using PoESkillTree.Computation.Common.Builders.Effects;
using PoESkillTree.Computation.Common.Builders.Entities;
using PoESkillTree.Computation.Common.Builders.Resolving;
using PoESkillTree.Computation.Common.Builders.Skills;
using PoESkillTree.Computation.Common.Builders.Stats;

namespace PoESkillTree.Computation.Builders.Effects
{
    public class AilmentBuilder : AvoidableEffectBuilder, IAilmentBuilder
    {
        private readonly Ailment _ailment;

        public AilmentBuilder(IStatFactory statFactory, Ailment ailment)
            : base(statFactory, CoreBuilder.Create(ailment.ToString()))
        {
            _ailment = ailment;
        }

        public override IEffectBuilder Resolve(ResolveContext context) => this;

        public IStatBuilder Chance => ChanceOn(new EntityBuilder(Entity.Enemy));

        public IStatBuilder InstancesOn(IEntityBuilder target) =>
            FromIdentity("InstanceCount", typeof(int)).For(target);

        public IFlagStatBuilder Source(IDamageTypeBuilder type)
        {
            var inner = CoreStatBuilderFromIdentity("HasSource", typeof(bool));
            var coreStat = new ParametrisedCoreStatBuilder<IKeywordBuilder>(inner, type,
                (k, s) => ConcretizeSourceStat((IDamageTypeBuilder) k, s));
            return new StatBuilder(StatFactory, coreStat);
        }

        private IEnumerable<IStat> ConcretizeSourceStat(IDamageTypeBuilder type, IStat stat) =>
            type.BuildDamageTypes().Select(t => StatFactory.CopyWithSuffix(stat, t.ToString(), typeof(bool)));

        public new Ailment Build() => _ailment;
    }
}