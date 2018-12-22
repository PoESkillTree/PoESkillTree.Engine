﻿using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PoESkillTree.Computation.Builders;
using PoESkillTree.Computation.Builders.Stats;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Parsing.ItemParsers;
using PoESkillTree.GameModel;
using PoESkillTree.GameModel.Items;
using PoESkillTree.GameModel.Modifiers;
using PoESkillTree.GameModel.Skills;

namespace PoESkillTree.Computation.Parsing.Tests
{
    [TestFixture]
    public class ItemParserTest
    {
        [Test]
        public void ParseReturnsCorrectResultForGlobalModifier()
        {
            var parserParam = CreateItem(ItemSlot.BodyArmour, "+42 to maximum Life");
            var source = CreateGlobalSource(parserParam);
            var baseItemDefinition = CreateBaseItemDefinition(parserParam.Item, default, default);
            var expected = CreateModifier("", Form.BaseAdd, 2, source);
            var coreParser = Mock.Of<ICoreParser>(p =>
                p.Parse(new CoreParserParameter("+42 to maximum Life", source, Entity.Character))
                == ParseResult.Success(new[] { expected }));
            var sut = CreateSut(baseItemDefinition, coreParser);

            var result = sut.Parse(parserParam);

            Assert.That(result.Modifiers, Has.Member(expected));
        }

        [Test]
        public void ParseReturnsCorrectItemTagsModifier()
        {
            var parserParam = CreateItem(ItemSlot.BodyArmour);
            var baseItemDefinition =
                CreateBaseItemDefinition(parserParam.Item, ItemClass.BodyArmour, Tags.BodyArmour | Tags.Armour);
            var expected = CreateModifier($"{parserParam.ItemSlot}.ItemTags", Form.BaseSet,
                baseItemDefinition.Tags.EncodeAsDouble());
            var sut = CreateSut(baseItemDefinition);

            var result = sut.Parse(parserParam);

            Assert.That(result.Modifiers, Has.Member(expected));
        }

        private static ItemParser CreateSut(BaseItemDefinition baseItemDefinition)
            => CreateSut(baseItemDefinition, Mock.Of<ICoreParser>());

        private static ItemParser CreateSut(BaseItemDefinition baseItemDefinition, ICoreParser coreParser)
        {
            var baseItemDefinitions = new BaseItemDefinitions(new[] { baseItemDefinition });
            var builderFactories =
                new BuilderFactories(new StatFactory(), new SkillDefinitions(new SkillDefinition[0]));
            return new ItemParser(baseItemDefinitions, builderFactories, coreParser);
        }

        private static ItemParserParameter CreateItem(ItemSlot itemSlot, params string[] mods)
            => CreateItem(itemSlot, 0, 0, mods);

        private static ItemParserParameter CreateItem(ItemSlot itemSlot, int quality, int requiredLevel, params string[] mods)
        {
            var modDict = new Dictionary<ModLocation, IReadOnlyList<string>>
            {
                { ModLocation.Explicit, mods }
            };
            var item = new Item("metadataId", "itemName", quality, requiredLevel, modDict);
            return new ItemParserParameter(item, itemSlot);
        }

        private static BaseItemDefinition CreateBaseItemDefinition(Item item, ItemClass itemClass, Tags tags)
            => new BaseItemDefinition(item.BaseMetadataId, "", itemClass,
                new string[0], tags, null, null, null,
                null, 0, 0, 0, default, "");

        private static ModifierSource.Global CreateGlobalSource(ItemParserParameter parserParam)
            => new ModifierSource.Global(CreateLocalSource(parserParam));

        private static ModifierSource.Local.Item CreateLocalSource(ItemParserParameter parserParam)
            => new ModifierSource.Local.Item(parserParam.ItemSlot, parserParam.Item.Name);

        private static Modifier CreateModifier(string stat, Form form, double? value, ModifierSource source = null)
            => new Modifier(new[] { new Stat(stat), }, form, new Constant(value),
                source ?? new ModifierSource.Global());
    }
}