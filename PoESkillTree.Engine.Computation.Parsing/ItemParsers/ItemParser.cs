using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.GameModel.StatTranslation;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Parsing.ItemParsers
{
    /// <summary>
    /// Parser for items in ItemSlots
    /// </summary>
    public class ItemParser : IParser<ItemParserParameter>
    {
        private readonly BaseItemDefinitions _baseItemDefinitions;
        private readonly IBuilderFactories _builderFactories;
        private readonly ICoreParser _coreParser;
        private readonly IStatTranslator _statTranslator;

        public ItemParser(
            BaseItemDefinitions baseItemDefinitions, IBuilderFactories builderFactories, ICoreParser coreParser,
            IStatTranslator statTranslator)
        {
            _baseItemDefinitions = baseItemDefinitions;
            _builderFactories = builderFactories;
            _coreParser = coreParser;
            _statTranslator = statTranslator;
        }

        public ParseResult Parse(ItemParserParameter parameter)
        {
            var (item, slot, entity) = parameter;

            if (!item.IsEnabled)
                return ParseResult.Empty;

            var localSource = new ModifierSource.Local.Item(slot, item.Name);
            var globalSource = new ModifierSource.Global(localSource);
            var baseItemDefinition = _baseItemDefinitions.GetBaseItemById(item.BaseMetadataId);
            var partialParserParameter =
                new PartialItemParserParameter(item, slot, entity, baseItemDefinition, localSource, globalSource);

            var partialParsers = CreatePartialParsers();
            var parseResults = new List<ParseResult>(partialParsers.Length);
            foreach (var partialParser in partialParsers)
            {
                parseResults.Add(partialParser.Parse(partialParserParameter));
            }
            return ParseResult.Aggregate(parseResults);
        }

        private IParser<PartialItemParserParameter>[] CreatePartialParsers()
            => new IParser<PartialItemParserParameter>[]
            {
                new ItemEquipmentParser(_builderFactories),
                new ItemPropertyParser(_builderFactories),
                new ItemModifierParser(_builderFactories, _coreParser, _statTranslator),
            };
    }

    public class ItemParserParameter : ValueObject
    {
        public ItemParserParameter(Item item, ItemSlot itemSlot, Entity entity)
            => (Item, ItemSlot, Entity) = (item, itemSlot, entity);

        public void Deconstruct(out Item item, out ItemSlot itemSlot, out Entity entity)
            => (item, itemSlot, entity) = (Item, ItemSlot, Entity);

        public Item Item { get; }
        public ItemSlot ItemSlot { get; }
        public Entity Entity { get; }

        protected override object ToTuple() => (Item, ItemSlot, Entity);
    }
}