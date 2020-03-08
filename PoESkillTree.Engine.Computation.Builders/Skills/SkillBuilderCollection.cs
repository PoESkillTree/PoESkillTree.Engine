using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MoreLinq;
using PoESkillTree.Engine.Computation.Builders.Actions;
using PoESkillTree.Engine.Computation.Builders.Entities;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Actions;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Skills;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;
using PoESkillTree.Engine.Utils.Extensions;

namespace PoESkillTree.Engine.Computation.Builders.Skills
{
    public class SkillBuilderCollection : ISkillBuilderCollection
    {
        private readonly IStatFactory _statFactory;
        private readonly ICoreBuilder<IEnumerable<Keyword>> _coreBuilder;
        private readonly IEnumerable<SkillDefinition> _skills;

        public SkillBuilderCollection(
            IStatFactory statFactory, IEnumerable<IKeywordBuilder> keywords,
            IEnumerable<SkillDefinition> skills)
            : this(statFactory, new KeywordsCoreBuilder(keywords), skills)
        {
        }

        private SkillBuilderCollection(
            IStatFactory statFactory, ICoreBuilder<IEnumerable<Keyword>> coreBuilder,
            IEnumerable<SkillDefinition> skills)
        {
            _statFactory = statFactory;
            _coreBuilder = coreBuilder;
            _skills = skills;
        }

        public ISkillBuilderCollection Resolve(ResolveContext context) =>
            new SkillBuilderCollection(_statFactory, _coreBuilder.Resolve(context), _skills);

        public IActionBuilder Cast =>
            new ActionBuilder(_statFactory, CoreBuilder.UnaryOperation(_coreBuilder,
                ks => $"{KeywordsToString(ks)}.Cast"), new ModifierSourceEntityBuilder());

        public IStatBuilder CombinedInstances =>
            new StatBuilder(_statFactory, new CoreStatBuilderFromCoreBuilder<IEnumerable<Keyword>>(_coreBuilder,
                (e, ks) => _statFactory.FromIdentity($"{KeywordsToString(ks)}.Instances", e,
                    typeof(uint))));

        public IStatBuilder Reservation =>
            new StatBuilder(_statFactory, new CoreStatBuilderFromCoreBuilder<IEnumerable<Keyword>>(_coreBuilder,
                (_, e, ks) => SelectSkillStats(ks, s => _statFactory.SkillReservation(e, s.Id))));

        public IStatBuilder ReservationPool =>
            new StatBuilder(_statFactory, new CoreStatBuilderFromCoreBuilder<IEnumerable<Keyword>>(_coreBuilder,
                (_, e, ks) => SelectSkillStats(ks, e, typeof(Pool))));

        private IEnumerable<IStat> SelectSkillStats(
            IEnumerable<Keyword> keywords, Entity entity, Type dataType,
            [CallerMemberName] string identitySuffix = "") =>
            SelectSkillStats(keywords, s => _statFactory.FromIdentity(s.Id + " . " + identitySuffix, entity, dataType));

        private IEnumerable<IStat> SelectSkillStats(
            IEnumerable<Keyword> keywords, Func<SkillDefinition, IStat> statFactory)
        {
            var keywordList = keywords.ToList();
            return _skills
                .Where(s => !s.IsSupport && s.ActiveSkill.Keywords.ContainsAll(keywordList))
                .Select(statFactory);
        }

        private static string KeywordsToString(IEnumerable<Keyword> keywords) =>
            $"Skills[{keywords.ToDelimitedString(", ")}]";

        private class KeywordsCoreBuilder : ICoreBuilder<IEnumerable<Keyword>>
        {
            private readonly IEnumerable<IKeywordBuilder> _keywords;

            public KeywordsCoreBuilder(IEnumerable<IKeywordBuilder> keywords) =>
                _keywords = keywords;

            public ICoreBuilder<IEnumerable<Keyword>> Resolve(ResolveContext context) =>
                new KeywordsCoreBuilder(_keywords.Select(b => b.Resolve(context)));

            public IEnumerable<Keyword> Build(BuildParameters parameters) =>
                _keywords.Select(b => b.Build(parameters));
        }
    }
}