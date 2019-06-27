using System;
using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Builders.Actions;
using PoESkillTree.Engine.Computation.Builders.Buffs;
using PoESkillTree.Engine.Computation.Builders.Charges;
using PoESkillTree.Engine.Computation.Builders.Damage;
using PoESkillTree.Engine.Computation.Builders.Effects;
using PoESkillTree.Engine.Computation.Builders.Entities;
using PoESkillTree.Engine.Computation.Builders.Skills;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Actions;
using PoESkillTree.Engine.Computation.Common.Builders.Buffs;
using PoESkillTree.Engine.Computation.Common.Builders.Charges;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Effects;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Skills;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Builders.Resolving
{
    public class UnresolvedReferenceConverter : IReferenceConverter
    {
        private readonly IStatFactory _statFactory;
        private readonly string _description;
        private readonly Func<ResolveContext, IReferenceConverter> _resolver;

        public UnresolvedReferenceConverter(IStatFactory statFactory, string description,
            Func<ResolveContext, IReferenceConverter> resolver) =>
            (_statFactory, _description, _resolver) = (statFactory, description, resolver);

        public IDamageTypeBuilder AsDamageType
        {
            get
            {
                var core = new UnresolvedCoreBuilder<IEnumerable<DamageType>>($"{this}.AsDamageType",
                    context => new ProxyDamageTypeBuilder(_resolver(context).AsDamageType));
                return new DamageTypeBuilder(_statFactory, core);
            }
        }

        public IChargeTypeBuilder AsChargeType
        {
            get
            {
                var core = new UnresolvedCoreBuilder<ChargeType>($"{this}.AsChargeType",
                    context => CoreBuilder.Proxy(_resolver(context).AsChargeType, (ps, b) => b.Build(ps)));
                return new ChargeTypeBuilder(_statFactory, core);
            }
        }

        public IAilmentBuilder AsAilment
        {
            get
            {
                var core = new UnresolvedCoreBuilder<Ailment>($"{this}.AsAilment",
                    context => CoreBuilder.Proxy<IAilmentBuilder, IEffectBuilder, Ailment>(
                        _resolver(context).AsAilment, (ps, b) => b.Build(ps)));
                return new AilmentBuilder(_statFactory, core);
            }
        }

        public IKeywordBuilder AsKeyword =>
            new UnresolvedKeywordBuilder($"{this}.AsKeyword", context => _resolver(context).AsKeyword);

        public IActionBuilder AsAction
        {
            get
            {
                var core = new UnresolvedCoreBuilder<string>($"{this}.AsAction",
                    context => CoreBuilder.Proxy(_resolver(context).AsAction, (ps, b) => b.Build(ps)));
                return new ActionBuilder(_statFactory, core, new ModifierSourceEntityBuilder());
            }
        }

        public IStatBuilder AsStat
        {
            get
            {
                var core = new UnresolvedCoreStatBuilder($"{this}.AsStat",
                    context => new StatBuilderAdapter(_resolver(context).AsStat));
                return new StatBuilder(_statFactory, core);
            }
        }

        public IPoolStatBuilder AsPoolStat
        {
            get
            {
                var core = new UnresolvedCoreBuilder<Pool>($"{this}.AsPoolStat",
                    context => CoreBuilder.Proxy<IPoolStatBuilder, IStatBuilder, Pool>(
                        _resolver(context).AsPoolStat, (ps, b) => b.BuildPool(ps)));
                return new PoolStatBuilder(_statFactory, core);
            }
        }

        public IBuffBuilder AsBuff
        {
            get
            {
                var core = new UnresolvedCoreBuilder<string>($"{this}.AsBuff",
                    context => CoreBuilder.Proxy((IEffectBuilder) _resolver(context).AsBuff, (ps, b) => b.Build(ps)));
                return new BuffBuilder(_statFactory, core);
            }
        }

        public ISkillBuilder AsSkill
        {
            get
            {
                var core = new UnresolvedCoreBuilder<SkillDefinition>($"{this}.AsSkill",
                    context => CoreBuilder.Proxy(_resolver(context).AsSkill, (ps, b) => b.Build(ps)));
                return new SkillBuilder(_statFactory, core);
            }
        }

        public override string ToString() => _description;
    }
}