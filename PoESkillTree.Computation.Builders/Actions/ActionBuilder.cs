﻿using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Computation.Builders.Conditions;
using PoESkillTree.Computation.Builders.Stats;
using PoESkillTree.Computation.Builders.Values;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Actions;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Entities;
using PoESkillTree.Computation.Common.Builders.Resolving;
using PoESkillTree.Computation.Common.Builders.Values;
using PoESkillTree.GameModel;
using static PoESkillTree.Computation.Common.ExplicitRegistrationTypes;

namespace PoESkillTree.Computation.Builders.Actions
{
    public class ActionBuilder : IActionBuilder
    {
        private const int RecentlySeconds = 4;

        protected IStatFactory StatFactory { get; }
        private readonly ICoreBuilder<string> _identity;
        private readonly IEntityBuilder _entity;

        public ActionBuilder(IStatFactory statFactory, ICoreBuilder<string> identity, IEntityBuilder entity)
        {
            StatFactory = statFactory;
            _identity = identity;
            _entity = entity;
        }

        public IActionBuilder Resolve(ResolveContext context) =>
            new ActionBuilder(StatFactory, _identity.Resolve(context), _entity);

        public IActionBuilder By(IEntityBuilder source) =>
            new ActionBuilder(StatFactory, _identity, source);

        public IConditionBuilder On =>
            new StatConvertingConditionBuilder(
                b => new StatBuilder(StatFactory,
                    new ParametrisedCoreStatBuilder<ICoreBuilder<string>>(
                        new StatBuilderAdapter(b), _identity, ConvertStat)),
                c => Resolve(c).On);

        private IEnumerable<IStat> ConvertStat(BuildParameters parameters, ICoreBuilder<string> identity, IStat stat)
        {
            var builtIdentity = identity.Build(parameters);
            return from e in _entity.Build(stat.Entity)
                   let i = $"On({builtIdentity}).By({e})"
                   let registrationType = GainOnAction(stat, builtIdentity, e)
                   select StatFactory.CopyWithSuffix(stat, i, stat.DataType, registrationType);
        }

        public IConditionBuilder InPastXSeconds(IValueBuilder seconds) =>
            new ValueConditionBuilder(ps => BuildInPastXSecondsValue(ps, seconds),
                c => Resolve(c).InPastXSeconds(seconds.Resolve(c)));

        private IValue BuildInPastXSecondsValue(BuildParameters parameters, IValueBuilder seconds)
        {
            var builtEntity = BuildEntity(parameters, _entity);
            var recentOccurrencesStat = BuildRecentOccurrencesStat(parameters, builtEntity);
            var lastOccurenceStat = BuildLastOccurrenceStat(parameters, builtEntity);
            var secondsValue = seconds.Build(parameters);
            return new ConditionalValue(Calculate,
                $"({RecentlySeconds} <= {secondsValue} && {recentOccurrencesStat} > 0) " +
                $"|| {lastOccurenceStat} <= {secondsValue}");

            bool Calculate(IValueCalculationContext context)
            {
                NodeValue? threshold = secondsValue.Calculate(context);
                if (RecentlySeconds <= threshold && context.GetValue(recentOccurrencesStat) > 0)
                    return true;
                return context.GetValue(lastOccurenceStat) <= threshold;
            }
        }

        public IConditionBuilder Recently =>
            InPastXSeconds(new ValueBuilderImpl(RecentlySeconds));

        public ValueBuilder CountRecently =>
            new ValueBuilder(new ValueBuilderImpl(BuildCountRecentlyValue, c => Resolve(c).CountRecently));

        private IValue BuildCountRecentlyValue(BuildParameters parameters)
            => new StatValue(BuildRecentOccurrencesStat(parameters, BuildEntity(parameters, _entity)));

        private IStat BuildLastOccurrenceStat(BuildParameters parameters, Entity entity)
            => StatFactory.FromIdentity($"{Build(parameters)}.LastOccurrence", entity, typeof(uint),
                UserSpecifiedValue(null));

        private IStat BuildRecentOccurrencesStat(BuildParameters parameters, Entity entity)
            => StatFactory.FromIdentity($"{Build(parameters)}.RecentOccurrences", entity, typeof(uint),
                UserSpecifiedValue(0));

        private static Entity BuildEntity(BuildParameters parameters, IEntityBuilder entity) =>
            entity.Build(parameters.ModifierSourceEntity).Single();

        public string Build(BuildParameters parameters) => _identity.Build(parameters);
    }
}