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

namespace PoESkillTree.Computation.Builders.Actions
{
    public class ActionBuilder : IActionBuilder
    {
        private const int RecentlySeconds = 4;

        protected IStatFactory StatFactory { get; }
        protected string Identity { get; }
        private readonly IEntityBuilder _entity;

        public ActionBuilder(IStatFactory statFactory, string identity, IEntityBuilder entity)
        {
            StatFactory = statFactory;
            Identity = identity;
            _entity = entity;
        }

        public IActionBuilder Resolve(ResolveContext context) =>
            new ActionBuilder(StatFactory, Identity, _entity.Resolve(context));

        public IActionBuilder By(IEntityBuilder source) =>
            new ActionBuilder(StatFactory, Identity, source);

        public IConditionBuilder On =>
            new StatConvertingConditionBuilder(b => new StatBuilder(StatFactory,
                new ParametrisedCoreStatBuilder<IEntityBuilder>(new StatBuilderAdapter(b), _entity,
                    (e, s) => ConvertStat(e, s))));

        private IEnumerable<IStat> ConvertStat(IEntityBuilder entity, IStat stat) =>
            from e in entity.Build(stat.Entity)
            let identity = $"On.{Identity}.By.{e}"
            select StatFactory.CopyWithSuffix(stat, identity, stat.DataType, true);

        public IConditionBuilder InPastXSeconds(IValueBuilder seconds) =>
            new ValueConditionBuilder<IEntityBuilder, IValueBuilder>(BuildInPastXSecondsValue, _entity, seconds);

        private IValue BuildInPastXSecondsValue(BuildParameters parameters, IEntityBuilder entity,
            IValueBuilder seconds)
        {
            var builtEntity = BuildEntity(parameters, entity);
            var recentOccurencesStat = BuildRecentOccurencesStat(builtEntity);
            var lastOccurenceStat = BuildLastOccurenceStat(builtEntity);
            var secondsValue = seconds.Build(parameters);
            return new ConditionalValue(Calculate,
                $"({RecentlySeconds} <= {secondsValue} && {recentOccurencesStat} > 0) " +
                $"|| {lastOccurenceStat} <= {secondsValue}");

            bool Calculate(IValueCalculationContext context)
            {
                NodeValue? threshold = secondsValue.Calculate(context);
                if (RecentlySeconds <= threshold && context.GetValue(recentOccurencesStat) > 0)
                    return true;
                return context.GetValue(lastOccurenceStat) <= threshold;
            }
        }

        public IConditionBuilder Recently =>
            InPastXSeconds(new ValueBuilderImpl(RecentlySeconds));

        public ValueBuilder CountRecently =>
            new ValueBuilder(new ValueBuilderImpl(BuildCountRecentlyValue, c => Resolve(c).CountRecently));

        private IValue BuildCountRecentlyValue(BuildParameters parameters) =>
            new StatValue(BuildRecentOccurencesStat(BuildEntity(parameters, _entity)));

        private IStat BuildLastOccurenceStat(Entity entity) =>
            StatFactory.FromIdentity($"{Identity}.LastOccurence", entity, typeof(int), true);

        private IStat BuildRecentOccurencesStat(Entity entity) =>
            StatFactory.FromIdentity($"{Identity}.RecentOccurences", entity, typeof(int), true);

        private static Entity BuildEntity(BuildParameters parameters, IEntityBuilder entity) =>
            entity.Build(parameters.ModifierSourceEntity).Single();
    }
}