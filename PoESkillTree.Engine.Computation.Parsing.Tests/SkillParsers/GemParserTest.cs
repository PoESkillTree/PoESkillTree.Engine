using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using PoESkillTree.Engine.Computation.Builders;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.GameModel.PassiveTree;
using PoESkillTree.Engine.GameModel.Skills;
using static PoESkillTree.Engine.Computation.Parsing.ParserTestUtils;
using static PoESkillTree.Engine.Computation.Parsing.SkillParsers.SkillParserTestUtils;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    [TestFixture]
    public class GemParserTest
    {
        [Test]
        public void FlameTotemHasCorrectRequirements()
        {
            var (definition, gem) = CreateFlameTotemDefinition();
            var sut = CreateSut(definition);

            var result = sut.Parse(gem, Entity.Character, out _);

            var modifiers = result.Modifiers;
            GetValueForIdentity(modifiers, "Level.Required").Calculate(null!).Should().Be(new NodeValue(74));
            AnyModifierHasIdentity(modifiers, "Dexterity.Required").Should().BeFalse();
            GetValueForIdentity(modifiers, "Intelligence.Required").Calculate(null!).Should().Be(new NodeValue(68));
            GetValueForIdentity(modifiers, "Strength.Required").Calculate(null!).Should().Be(new NodeValue(98));
        }

        private static (SkillDefinition, Gem) CreateFlameTotemDefinition()
        {
            var activeSkill = CreateActiveSkillDefinition("Flame Totem");
            var level = CreateLevelDefinition(requiredLevel: 74, requiredIntelligence: 68, requiredStrength: 98);
            var levels = new Dictionary<int, SkillLevelDefinition> { { 1, level } };
            return (CreateActive("FlameTotem", activeSkill, levels),
                new Gem("FlameTotem", 1, 10, ItemSlot.Belt, 0, 0, true));
        }

        [Test]
        public void CastOnCriticalStrikeHasCorrectDexterityRequirement()
        {
            var (definition, gem) = CreateCastOnCriticalStrikeDefinition();
            var sut = CreateSut(definition);

            var result = sut.Parse(gem, Entity.Character, out _);

            var modifiers = result.Modifiers;
            GetValueForIdentity(modifiers, "Dexterity.Required").Calculate(null!).Should().Be(new NodeValue(40));
        }

        private static (SkillDefinition, Gem) CreateCastOnCriticalStrikeDefinition()
        {
            var activeSkill = CreateActiveSkillDefinition("Cast On Critical Strike Support");
            var level = CreateLevelDefinition(requiredLevel: 38, requiredDexterity: 40, requiredIntelligence: 27);
            var levels = new Dictionary<int, SkillLevelDefinition> { { 1, level } };
            return (CreateActive("SupportCastOnCrit", activeSkill, levels),
                new Gem("SupportCastOnCrit", 1, 10, ItemSlot.Belt, 0, 0, true));
        }

        // TODO test skills output

        private static GemParser CreateSut(SkillDefinition definition)
        {
            var skillDefinitions = new SkillDefinitions(new[] {definition});
            var builderFactories = new BuilderFactories(new PassiveTreeDefinition(Array.Empty<PassiveNodeDefinition>()), skillDefinitions);
            return new GemParser(skillDefinitions, builderFactories);
        }
    }
}