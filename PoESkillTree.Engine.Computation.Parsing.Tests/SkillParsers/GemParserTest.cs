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

        [Test]
        public void FlameTotemHasASingleSkill()
        {
            var (definition, gem) = CreateFlameTotemDefinition();
            var expected = Skill.FromGem(gem, true);
            var sut = CreateSut(definition);

            sut.Parse(gem, Entity.Character, out var actual);

            actual.Should().BeEquivalentTo(expected);
        }

        private static (SkillDefinition, Gem) CreateFlameTotemDefinition() =>
            CreateDefinition("FlameTotem", requiredLevel: 74, requiredIntelligence: 68, requiredStrength: 98);

        [Test]
        public void CastOnCriticalStrikeHasCorrectDexterityRequirement()
        {
            var (definition, gem) = CreateCastOnCriticalStrikeDefinition();
            var sut = CreateSut(definition);

            var result = sut.Parse(gem, Entity.Character, out _);

            var modifiers = result.Modifiers;
            GetValueForIdentity(modifiers, "Dexterity.Required").Calculate(null!).Should().Be(new NodeValue(40));
        }

        [Test]
        public void CastOnCriticalStrikeHasTwoEnabledSkills()
        {
            var (definition, gem) = CreateCastOnCriticalStrikeDefinition();
            IReadOnlyList<Skill> expected = new[]
            {
                Skill.FromGem(gem, true),
                Skill.SecondaryFromGem("SupportCastOnCritTriggered", gem, true),
            };
            var sut = CreateSut(definition);

            sut.Parse(gem, Entity.Character, out var actual);

            actual.Should().BeEquivalentTo(expected);
        }

        private static (SkillDefinition, Gem) CreateCastOnCriticalStrikeDefinition() =>
            CreateDefinition("SupportCastOnCrit", "SupportCastOnCritTriggered",
                requiredLevel: 38, requiredDexterity: 40, requiredIntelligence: 27);

        [Test]
        public void VaalGraceHasDisabledSecondarySkill()
        {
            var (definition, gem) = CreateVaalGraceDefinition();
            IReadOnlyList<Skill> expected = new[]
            {
                Skill.FromGem(gem, true),
                Skill.SecondaryFromGem("Grace", gem, false),
            };
            var sut = CreateSut(definition);

            sut.Parse(gem, Entity.Character, out var actual);

            actual.Should().BeEquivalentTo(expected);
        }

        private static (SkillDefinition, Gem) CreateVaalGraceDefinition() =>
            CreateDefinition("VaalGrace", "Grace", gemTags: new[] {"vaal", "aura"});

        [Test]
        public void VaalArcHasTwoEnabledSkills()
        {
            var (definition, gem) = CreateVaalArcDefinition();
            IReadOnlyList<Skill> expected = new[]
            {
                Skill.FromGem(gem, true),
                Skill.SecondaryFromGem("Arc", gem, true),
            };
            var sut = CreateSut(definition);

            sut.Parse(gem, Entity.Character, out var actual);

            actual.Should().BeEquivalentTo(expected);
        }

        private static (SkillDefinition, Gem) CreateVaalArcDefinition() =>
            CreateDefinition("VaalArc", "Arc", gemTags: new[] {"vaal"});

        private static (SkillDefinition, Gem) CreateDefinition(
            string skillId, string? secondarySkillId = null, string[]? gemTags = null,
            int requiredLevel = 0, int requiredDexterity = 0, int requiredIntelligence = 0, int requiredStrength = 0)
        {
            var activeSkill = CreateActiveSkillDefinition(skillId);
            var level = CreateLevelDefinition(requiredLevel: requiredLevel, requiredDexterity: requiredDexterity,
                requiredIntelligence: requiredIntelligence, requiredStrength: requiredStrength);
            var levels = new Dictionary<int, SkillLevelDefinition> { { 1, level } };
            return (SkillDefinition.CreateActive(skillId, 0, "", secondarySkillId, Array.Empty<string>(),
                    new SkillBaseItemDefinition(skillId, "", default, gemTags ?? Array.Empty<string>()),
                    activeSkill, levels),
                new Gem(skillId, 1, 0, ItemSlot.Belt, 0, 0, true));
        }

        private static GemParser CreateSut(SkillDefinition definition)
        {
            var skillDefinitions = new SkillDefinitions(new[] {definition});
            var builderFactories = new BuilderFactories(new PassiveTreeDefinition(Array.Empty<PassiveNodeDefinition>()), skillDefinitions);
            return new GemParser(skillDefinitions, builderFactories);
        }
    }
}