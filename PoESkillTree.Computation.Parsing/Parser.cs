﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Data;
using PoESkillTree.Computation.Parsing.ItemParsers;
using PoESkillTree.Computation.Parsing.PassiveTreeParsers;
using PoESkillTree.Computation.Parsing.SkillParsers;
using PoESkillTree.GameModel;
using PoESkillTree.GameModel.Items;
using PoESkillTree.GameModel.PassiveTree;
using PoESkillTree.GameModel.Skills;
using PoESkillTree.GameModel.StatTranslation;
using PoESkillTree.Utils.Extensions;

namespace PoESkillTree.Computation.Parsing
{
    public class Parser<TStep> : IParser
    {
        private readonly ICoreParser _coreParser;
        private readonly IParser<ushort> _passiveNodeParser;
        private readonly IParser<ushort> _skilledPassiveNodeParser;
        private readonly IParser<ItemParserParameter> _itemParser;
        private readonly IParser<ItemSlot> _emptyItemSlotParser;
        private readonly IParser<Skill> _activeSkillParser;
        private readonly IParser<SupportSkillParserParameter> _supportSkillParser;

        private readonly StatTranslators _statTranslators;
        private readonly IEnumerable<IGivenStats> _givenStats;

        private readonly IDictionary<string, IParser<UntranslatedStatParserParameter>> _untranslatedStatParsers =
            new Dictionary<string, IParser<UntranslatedStatParserParameter>>();

        public static async Task<IParser> CreateAsync(
            GameData gameData, Task<IBuilderFactories> builderFactoriesTask, Task<IParsingData<TStep>> parsingDataTask)
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
                await parsingDataTask.ConfigureAwait(false));
        }

        private Parser(
            PassiveTreeDefinition passiveTree, BaseItemDefinitions baseItems, SkillDefinitions skills,
            StatTranslators statTranslators, IBuilderFactories builderFactories, IParsingData<TStep> parsingData)
        {
            _statTranslators = statTranslators;
            _coreParser = new CoreParser<TStep>(parsingData, builderFactories);
            _givenStats = parsingData.GivenStats;

            _passiveNodeParser = new PassiveNodeParser(passiveTree, builderFactories, _coreParser);
            _skilledPassiveNodeParser = new SkilledPassiveNodeParser(passiveTree, builderFactories);
            _itemParser =
                new ItemParser(baseItems, builderFactories, _coreParser, statTranslators[StatTranslationFileNames.Main]);
            _emptyItemSlotParser = new EmptyItemSlotParser(builderFactories);
            _activeSkillParser = new ActiveSkillParser(skills, builderFactories, GetOrAddUntranslatedStatParser);
            _supportSkillParser = new SupportSkillParser(skills, builderFactories, GetOrAddUntranslatedStatParser);
        }

        private IParser<UntranslatedStatParserParameter> GetOrAddUntranslatedStatParser(string translationFileName)
            => _untranslatedStatParsers.GetOrAdd(translationFileName, CreateUntranslatedStatParser);

        private IParser<UntranslatedStatParserParameter> CreateUntranslatedStatParser(string translationFileName)
        {
            var composite = new CompositeStatTranslator(
                _statTranslators[translationFileName],
                _statTranslators[StatTranslationFileNames.Custom]);
            return new UntranslatedStatParser(composite, _coreParser);
        }

        public ParseResult ParseRawModifier(
            string modifierLine, ModifierSource modifierSource, Entity modifierSourceEntity)
            => _coreParser.Parse(modifierLine, modifierSource, modifierSourceEntity);

        public ParseResult ParsePassiveNode(ushort nodeId)
            => _passiveNodeParser.Parse(nodeId);

        public ParseResult ParseSkilledPassiveNode(ushort nodeId)
            => _skilledPassiveNodeParser.Parse(nodeId);

        public ParseResult ParseItem(Item item, ItemSlot itemSlot)
            => _itemParser.Parse(new ItemParserParameter(item, itemSlot));

        public ParseResult ParseEmptyItemSlot(ItemSlot itemSlot)
            => _emptyItemSlotParser.Parse(itemSlot);

        public ParseResult ParseActiveSkill(Skill activeSkill)
            => _activeSkillParser.Parse(activeSkill);

        public ParseResult ParseSupportSkill(Skill activeSkill, Skill supportSkill)
            => _supportSkillParser.Parse(activeSkill, supportSkill);

        public IReadOnlyList<Modifier> ParseGivenModifiers()
            => GivenStatsParser.Parse(_coreParser, _givenStats);
    }
}