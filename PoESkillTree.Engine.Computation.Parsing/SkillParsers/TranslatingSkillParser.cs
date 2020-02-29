using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;
using PoESkillTree.Engine.GameModel.StatTranslation;
using PoESkillTree.Engine.Utils.Extensions;
using static MoreLinq.Extensions.IndexExtension;
using static MoreLinq.Extensions.PartitionExtension;
using static MoreLinq.Extensions.ToLookupExtension;
#if NETSTANDARD2_0
using static MoreLinq.Extensions.ToHashSetExtension;
#endif

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public delegate IParser<UntranslatedStatParserParameter> UntranslatedStatParserFactory(
        IReadOnlyList<string> statTranslationFileNames);

    /// <summary>
    /// Partial parser of <see cref="ActiveSkillParser"/> and <see cref="SupportSkillParser"/> that translates and
    /// parses all <see cref="UntranslatedStat"/>s that were not handled previously.
    /// </summary>
    public class TranslatingSkillParser
    {
        // This does not match every keystone stat, but it does match the two that are currently on skills.
        private static readonly Regex KeystoneStatRegex = new Regex("^keystone_");

        private readonly IBuilderFactories _builderFactories;
        private readonly UntranslatedStatParserFactory _statParserFactory;

        private SkillPreParseResult? _preParseResult;
        private IEnumerable<UntranslatedStat>? _parsedStats;

        public TranslatingSkillParser(
            IBuilderFactories builderFactories, UntranslatedStatParserFactory statParserFactory)
            => (_builderFactories, _statParserFactory) =
                (builderFactories, statParserFactory);

        public ParseResult Parse(
            Skill skill, SkillPreParseResult preParseResult, PartialSkillParseResult partialResult)
        {
            _preParseResult = preParseResult;
            _parsedStats = partialResult.ParsedStats.ToHashSet();

            var isMainSkill = preParseResult.IsMainSkill;
            var isActiveSkill = _builderFactories.MetaStatBuilders.IsActiveSkill(skill);
            var level = preParseResult.LevelDefinition;
            var qualityStats = level.QualityStats.Select(s => ApplyQuality(s, skill));
            var (keystoneStats, levelStats) = level.Stats.Partition(s => KeystoneStatRegex.IsMatch(s.StatId));
            var parseResults = new List<ParseResult>(4 + level.AdditionalStatsPerPart.Count + 4)
            {
                ParseResult.Success(partialResult.ParsedModifiers.ToList()),
                TranslateAndParse(qualityStats, isMainSkill),
                TranslateAndParse(levelStats, isMainSkill),
                // Keystones are translated into their names when using the main instead of skill translation files
                TranslateAndParse(keystoneStats, isMainSkill, StatTranslationFileNames.Main),
            };

            foreach (var (partIndex, stats) in level.AdditionalStatsPerPart.Index())
            {
                var condition = isMainSkill.And(_builderFactories.StatBuilders.MainSkillPart.Value.Eq(partIndex));
                var result = TranslateAndParse(stats, condition);
                parseResults.Add(result);
            }

            var qualityBuffStats = level.QualityBuffStats.Select(s => new BuffStat(ApplyQuality(s.Stat, skill), s.GetAffectedEntities));
            parseResults.Add(TranslateAndParseBuff(qualityBuffStats, isActiveSkill));
            parseResults.Add(TranslateAndParseBuff(level.BuffStats, isActiveSkill));

            var qualityPassiveStats = level.QualityPassiveStats.Select(s => ApplyQuality(s, skill));
            parseResults.Add(TranslateAndParse(qualityPassiveStats, isActiveSkill));
            parseResults.Add(TranslateAndParse(level.PassiveStats, isActiveSkill));

            _preParseResult = null;
            _parsedStats = null;
            return ParseResult.Aggregate(parseResults);
        }

        private ParseResult TranslateAndParse(IEnumerable<UntranslatedStat> stats, IConditionBuilder condition)
            => TranslateAndParse(stats, condition, _preParseResult!.SkillDefinition.StatTranslationFile, StatTranslationFileNames.Skill);

        private ParseResult TranslateAndParse(
            IEnumerable<UntranslatedStat> stats, IConditionBuilder condition, params string[] statTranslationFileNames)
        {
            var result = TranslateAndParse(stats, _preParseResult!.ModifierSourceEntity, statTranslationFileNames);
            return result.ApplyCondition(condition.Build, _preParseResult.ModifierSourceEntity);
        }

        private ParseResult TranslateAndParseBuff(IEnumerable<BuffStat> buffStats, IConditionBuilder condition)
        {
            var sourceEntity = _preParseResult!.ModifierSourceEntity;
            var results = new List<ParseResult>();
            var buffBuilder = _builderFactories.SkillBuilders.FromId(_preParseResult.SkillDefinition.Id).Buff;
            var statLookup = buffStats
                .SelectMany(t => t.GetAffectedEntities(sourceEntity).Select(e => (e, t.Stat)))
                .ToLookup();
            foreach (var (affectedEntity, stats) in statLookup)
            {
                var result = TranslateAndParse(stats, affectedEntity, StatTranslationFileNames.Main, StatTranslationFileNames.Skill);
                result = result.ApplyCondition(condition.Build, sourceEntity);

                var buildParameters = new BuildParameters(_preParseResult.GlobalSource, affectedEntity, default);
                var multiplier = buffBuilder.BuildAddStatMultiplier(buildParameters, new[] { sourceEntity });
                result = result.ApplyMultiplier(_ => multiplier, sourceEntity);
                results.Add(result);
            }

            return ParseResult.Aggregate(results);
        }

        private ParseResult TranslateAndParse(IEnumerable<UntranslatedStat> stats,
            Entity modifierSourceEntity,
            params string[] statTranslationFileNames)
        {
            var unparsedStats = stats.Except(_parsedStats).ToList();
            var (statsWithGemSource, statsWithLocalSource) =
                unparsedStats.Partition(s => SkillStatIds.ManipulatesSupportedGems.IsMatch(s.StatId));
            var statParser = _statParserFactory(statTranslationFileNames);
            var gemSourceResult = Parse(statParser, _preParseResult!.GemSource, modifierSourceEntity, statsWithGemSource.ToList());
            var localSourceResult = Parse(statParser, _preParseResult.LocalSource, modifierSourceEntity, statsWithLocalSource.ToList());
            return ParseResult.Aggregate(new[] {gemSourceResult, localSourceResult});
        }

        private static ParseResult Parse(IParser<UntranslatedStatParserParameter> parser,
            ModifierSource.Local modifierSource, Entity modifierSourceEntity, IReadOnlyList<UntranslatedStat> stats)
        {
            if (stats.IsEmpty())
                return ParseResult.Empty;
            return parser.Parse(modifierSource, modifierSourceEntity, stats);
        }

        private static UntranslatedStat ApplyQuality(UntranslatedStat qualityStat, Skill skill)
            => new UntranslatedStat(qualityStat.StatId, qualityStat.Value * skill.Quality / 1000);
    }
}