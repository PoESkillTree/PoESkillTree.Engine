using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Parsing.ItemParsers;
using PoESkillTree.Engine.Computation.Parsing.JewelParsers;
using PoESkillTree.Engine.Computation.Parsing.PassiveTreeParsers;
using PoESkillTree.Engine.Computation.Parsing.SkillParsers;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.GameModel.PassiveTree;
using PoESkillTree.Engine.GameModel.Skills;
using PoESkillTree.Engine.GameModel.StatTranslation;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Parsing
{
    public class Parser<TStep> : IParser
    {
        private readonly ICoreParser _coreParser;
        private readonly IParser<ushort> _passiveNodeParser;
        private readonly IParser<ushort> _skilledPassiveNodeParser;
        private readonly IParser<ItemParserParameter> _itemParser;
        private readonly IParser<ItemParserParameter> _itemJewelParser;
        private readonly IParser<JewelInSkillTreeParserParameter> _treeJewelParser;
        private readonly IParser<SkillsParserParameter> _skillsParser;
        private readonly IParser<ActiveSkillParserParameter> _activeSkillParser;
        private readonly IParser<SupportSkillParserParameter> _supportSkillParser;
        private readonly IParser<GemParserParameter> _gemParser;

        private readonly StatTranslators _statTranslators;
        private readonly IEnumerable<IGivenStats> _givenStats;

        private readonly ConcurrentDictionary<IReadOnlyList<string>, IParser<UntranslatedStatParserParameter>>
            _untranslatedStatParsers =
                new ConcurrentDictionary<IReadOnlyList<string>, IParser<UntranslatedStatParserParameter>>();

        public static async Task<Parser<TStep>> CreateAsync(
            GameData gameData, Task<IBuilderFactories> builderFactoriesTask, Task<IParsingData<TStep>> parsingDataTask,
            IValueCalculationContext valueCalculationContext)
        {
            var passiveTreeTask = gameData.PassiveTree;
            var baseItemsTask = gameData.BaseItems;
            var skillsTask = gameData.Skills;
            var statTranslatorsTask = gameData.StatTranslators;
            return new Parser<TStep>(
                await passiveTreeTask.ConfigureAwait(false),
                await baseItemsTask.ConfigureAwait(false),
                await skillsTask.ConfigureAwait(false),
                await statTranslatorsTask.ConfigureAwait(false),
                await builderFactoriesTask.ConfigureAwait(false),
                await parsingDataTask.ConfigureAwait(false),
                valueCalculationContext);
        }

        private Parser(
            PassiveTreeDefinition passiveTree, BaseItemDefinitions baseItems, SkillDefinitions skills,
            StatTranslators statTranslators, IBuilderFactories builderFactories, IParsingData<TStep> parsingData,
            IValueCalculationContext valueCalculationContext)
        {
            _statTranslators = statTranslators;
            _coreParser = new CoreParser<TStep>(parsingData, builderFactories);
            _givenStats = parsingData.GivenStats;

            _passiveNodeParser = Caching(new PassiveNodeParser(passiveTree, builderFactories, _coreParser));
            _skilledPassiveNodeParser = Caching(new SkilledPassiveNodeParser(passiveTree, builderFactories));
            _itemParser = Caching(new ItemParser(baseItems, builderFactories, _coreParser,
                statTranslators[StatTranslationFileNames.Main]));
            _itemJewelParser = Caching(new JewelInItemParser(_coreParser));
            _treeJewelParser = Caching(new JewelInSkillTreeParser(passiveTree, builderFactories, _coreParser));
            _activeSkillParser =
                Caching(new ActiveSkillParser(skills, builderFactories, GetOrAddUntranslatedStatParser));
            _supportSkillParser =
                Caching(new SupportSkillParser(skills, builderFactories, GetOrAddUntranslatedStatParser));
            var skillModificationParser = new SkillModificationParser(skills, builderFactories, valueCalculationContext);
            _skillsParser = new SkillsParser(skills, _activeSkillParser, _supportSkillParser, skillModificationParser);
            _gemParser = new GemParser(skills, builderFactories);
        }

        private IParser<UntranslatedStatParserParameter> GetOrAddUntranslatedStatParser(
            IReadOnlyList<string> translationFileNames)
            => _untranslatedStatParsers.GetOrAdd(translationFileNames, CreateUntranslatedStatParser);

        private IParser<UntranslatedStatParserParameter> CreateUntranslatedStatParser(
            IReadOnlyList<string> translationFileNames)
        {
            var translators = translationFileNames
                .Append(StatTranslationFileNames.Custom)
                .Select(s => _statTranslators[s])
                .ToList();
            var composite = new CompositeStatTranslator(translators);
            return Caching(new UntranslatedStatParser(composite, _coreParser));
        }

        private static IParser<T> Caching<T>(IParser<T> parser)
            => new CachingParser<T>(parser);

        public ParseResult ParseRawModifier(
            string modifierLine, ModifierSource modifierSource, Entity modifierSourceEntity)
            => _coreParser.Parse(modifierLine, modifierSource, modifierSourceEntity);

        public ParseResult ParsePassiveNode(ushort nodeId)
            => _passiveNodeParser.Parse(nodeId);

        public ParseResult ParseSkilledPassiveNode(ushort nodeId)
            => _skilledPassiveNodeParser.Parse(nodeId);

        public ParseResult ParseItem(Item item, ItemSlot itemSlot, Entity entity = Entity.Character)
            => _itemParser.Parse(new ItemParserParameter(item, itemSlot, entity));

        public ParseResult ParseJewelSocketedInItem(Item item, ItemSlot itemSlot, Entity entity = Entity.Character)
            => _itemJewelParser.Parse(new ItemParserParameter(item, itemSlot, entity));

        public ParseResult ParseJewelSocketedInSkillTree(Item item, JewelRadius jewelRadius, ushort nodeId)
            => _treeJewelParser.Parse(new JewelInSkillTreeParserParameter(item, jewelRadius, nodeId));

        public ParseResult ParseSkills(IReadOnlyList<Skill> skills, Entity entity = Entity.Character)
            => _skillsParser.Parse(new SequenceEquatableListView<Skill>(skills), entity);

        public ParseResult ParseGem(Gem gem, out IReadOnlyList<Skill> skills, Entity entity = Entity.Character) =>
            _gemParser.Parse(gem, entity, out skills);

        public ParseResult ParseActiveSkill(ActiveSkillParserParameter parameter) =>
            _activeSkillParser.Parse(parameter);

        public ParseResult ParseSupportSkill(SupportSkillParserParameter parameter) =>
            _supportSkillParser.Parse(parameter);

        public IReadOnlyList<Modifier> ParseGivenModifiers()
            => GivenStatsParser.Parse(_coreParser, _givenStats);

        public IEnumerable<Func<IReadOnlyList<Modifier>>> CreateGivenModifierParseDelegates()
            => _givenStats.Select<IGivenStats, Func<IReadOnlyList<Modifier>>>(
                g => () => GivenStatsParser.Parse(_coreParser, g));
    }
}