using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EnumsNET;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Skills;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Parsing;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;
using PoESkillTree.Engine.Utils.Extensions;

namespace PoESkillTree.Engine.Computation.Builders.Damage
{
    public class DamageTypeBuilder : IDamageTypeBuilder
    {
        private static readonly IReadOnlyList<DamageType> NonRandomDamageTypes =
            Enums.GetValues<DamageType>().Except(DamageType.RandomElement).ToList();

        private readonly IStatFactory _statFactory;
        private readonly ICoreBuilder<IEnumerable<DamageType>> _coreDamageType;

        public DamageTypeBuilder(IStatFactory statFactory, DamageType damageType)
            : this(statFactory, CoreBuilder.Create(new[] { damageType }))
        {
        }

        public DamageTypeBuilder(IStatFactory statFactory, ICoreBuilder<IEnumerable<DamageType>> coreDamageType)
        {
            _coreDamageType = coreDamageType;
            _statFactory = statFactory;
        }

        private IDamageTypeBuilder With(ICoreBuilder<IEnumerable<DamageType>> coreDamageType) =>
            new DamageTypeBuilder(_statFactory, coreDamageType);

        public IKeywordBuilder Resolve(ResolveContext context) =>
            With(_coreDamageType.Resolve(context));

        public Keyword Build(BuildParameters parameters)
        {
            var types = _coreDamageType.Build(parameters).ToList();
            if (types.IsEmpty())
                throw new ParseException("Can't built zero damage types");
            if (types.Count > 1)
                throw new ParseException("Can't built multiple damage types");
            if (Enum.TryParse(types.Single().ToString(), out Keyword keyword))
                return keyword;
            throw new ParseException($"{_coreDamageType} can't be used as a keyword");
        }

        public IReadOnlyList<DamageType> BuildDamageTypes(BuildParameters parameters)
            => _coreDamageType.Build(parameters).ToList();

        public IDamageTypeBuilder And(IDamageTypeBuilder type) =>
            With(CoreBuilder.BinaryOperation(_coreDamageType, new ProxyDamageTypeBuilder(type), Enumerable.Union));

        public IDamageTypeBuilder Invert =>
            With(CoreBuilder.UnaryOperation(_coreDamageType, NonRandomDamageTypes.Except));

        public IDamageTypeBuilder Except(IDamageTypeBuilder type) =>
            With(CoreBuilder.BinaryOperation(_coreDamageType, new ProxyDamageTypeBuilder(type), Enumerable.Except));

        public IStatBuilder Resistance =>
            new StatBuilder(_statFactory, CoreStat(typeof(int)));

        public IStatBuilder ResistanceAgainstHits =>
            new StatBuilder(_statFactory, CoreStat(typeof(double)));

        public IStatBuilder ResistanceAgainstDoTs =>
            new StatBuilder(_statFactory, CoreStat(typeof(double)));

        public IDamageStatBuilder Damage =>
            new DamageStatBuilder(_statFactory, CoreStat(_statFactory.Damage));

        public IDamageRelatedStatBuilder DamageMultiplier =>
            DamageRelatedStatBuilder.Create(_statFactory, new CompositeCoreStatBuilder(
                CoreStat(typeof(int), nameof(DamageMultiplierWithCrits)),
                CoreStat(typeof(int), nameof(DamageMultiplierWithNonCrits))));

        public IDamageRelatedStatBuilder DamageMultiplierWithCrits =>
            DamageRelatedStatBuilder.Create(_statFactory, CoreStat(typeof(int)));

        public IDamageRelatedStatBuilder DamageMultiplierWithNonCrits =>
            DamageRelatedStatBuilder.Create(_statFactory, CoreStat(typeof(int)));

        public IDamageTakenConversionBuilder DamageTakenFrom(IPoolStatBuilder pool)
        {
            var damage = CoreStat(_statFactory.Damage);
            var takenFrom = new ParametrisedCoreStatBuilder<IStatBuilder>(damage, pool,
                (ps, p, s) => _statFactory.CopyWithSuffix(s, $"TakenFrom({((IPoolStatBuilder) p).BuildPool(ps)})",
                    typeof(uint)));
            return new DamageTakenConversionBuilder(_statFactory, takenFrom);
        }

        public IStatBuilder HitDamageTakenAs(DamageType type) =>
            new StatBuilder(_statFactory,
                CoreStat((e, t) => _statFactory.FromIdentity($"{t}.HitDamageTakenAs({type})", e, typeof(uint))));


        public IDamageRelatedStatBuilder Penetration =>
            DamageRelatedStatBuilder.Create(_statFactory, new CompositeCoreStatBuilder(
                CoreStat(typeof(int), nameof(PenetrationWithCrits)),
                CoreStat(typeof(int), nameof(PenetrationWithNonCrits)))).WithHits;

        public IDamageRelatedStatBuilder PenetrationWithCrits =>
            DamageRelatedStatBuilder.Create(_statFactory, CoreStat(typeof(int))).WithHits;

        public IDamageRelatedStatBuilder PenetrationWithNonCrits =>
            DamageRelatedStatBuilder.Create(_statFactory, CoreStat(typeof(int))).WithHits;

        public IDamageRelatedStatBuilder IgnoreResistance =>
            DamageRelatedStatBuilder.Create(_statFactory, new CompositeCoreStatBuilder(
                CoreStat(typeof(bool), nameof(IgnoreResistanceWithCrits)),
                CoreStat(typeof(bool), nameof(IgnoreResistanceWithNonCrits)))).WithHits;

        public IDamageRelatedStatBuilder IgnoreResistanceWithCrits =>
            DamageRelatedStatBuilder.Create(_statFactory, CoreStat(typeof(bool))).WithHits;

        public IDamageRelatedStatBuilder IgnoreResistanceWithNonCrits =>
            DamageRelatedStatBuilder.Create(_statFactory, CoreStat(typeof(bool))).WithHits;

        public IStatBuilder Exposure
            => new StatBuilder(_statFactory, CoreStat(_statFactory.Exposure));

        public IStatBuilder ReflectedDamageTaken =>
            new StatBuilder(_statFactory, CoreStat(typeof(int)));

        private ICoreStatBuilder CoreStat(Type dataType, [CallerMemberName] string identitySuffix = "") =>
            CoreStat((e, t) => _statFactory.FromIdentity(t.GetName() + "." + identitySuffix, e, dataType));

        private ICoreStatBuilder CoreStat(Func<Entity, DamageType, IStat> statFactory) =>
            new CoreStatBuilderFromCoreBuilder<IEnumerable<DamageType>>(_coreDamageType,
                (_, e, ts) => ts.Select(t => statFactory(e, t)));

        private class DamageTakenConversionBuilder : IDamageTakenConversionBuilder
        {
            private readonly IStatFactory _statFactory;
            private readonly ICoreStatBuilder _coreStat;

            public DamageTakenConversionBuilder(IStatFactory statFactory, ICoreStatBuilder coreStat)
            {
                _statFactory = statFactory;
                _coreStat = coreStat;
            }

            public IStatBuilder Before(IPoolStatBuilder pool)
            {
                var coreStat = new ParametrisedCoreStatBuilder<IStatBuilder>(_coreStat, pool,
                    (ps, p, s) => _statFactory.CopyWithSuffix(s, $"Before({((IPoolStatBuilder) p).BuildPool(ps)})",
                        typeof(uint)));
                return new StatBuilder(_statFactory, coreStat);
            }
        }
    }
}