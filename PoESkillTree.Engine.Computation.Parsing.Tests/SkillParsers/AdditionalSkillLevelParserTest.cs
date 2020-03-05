using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using PoESkillTree.Engine.Computation.Builders.Skills;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Builders.Values;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.GameModel.Skills;
using static PoESkillTree.Engine.Computation.Parsing.SkillParsers.SkillParserTestUtils;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    [TestFixture]
    public class AdditionalSkillLevelParserTest
    {
        [Test]
        public void GivenActiveSkillWithNoSupportingSkillsAndNoAdditionalLevelStats_WhenParsing_ThenValueIsZero()
        {
            var (active, gem) = CreateSkillAndGem("a");
            var sut = CreateSut();

            var actual = sut.Parse(active, new Skill[0]);

            actual.Keys.Should().Contain(active);
            BuildAndCalculate(actual[active], gem).Should().BeEquivalentTo(new NodeValue(0));
        }

        [Test]
        public void GivenSupportingSkillWithNoAdditionalLevelStats_WhenParsing_ThenValuesAreZero()
        {
            var (active, activeGem) = CreateSkillAndGem("a");
            var (support, supportGem) = CreateSkillAndGem("s1");
            var sut = CreateSut();

            var actual = sut.Parse(active, new[] {support});
            
            actual.Keys.Should().Contain(active);
            BuildAndCalculate(actual[active], activeGem).Should().BeEquivalentTo(new NodeValue(0));
            actual.Keys.Should().Contain(support);
            BuildAndCalculate(actual[support], supportGem).Should().BeEquivalentTo(new NodeValue(0));
        }

        [Test]
        public void GivenMultipleSupportingSkillsWithNoAdditionalLevelStats_WhenParsing_ThenValuesAreZero()
        {
            var (active, _) = CreateSkillAndGem("a");
            var supports = new[] {CreateSkillAndGem("s1"), CreateSkillAndGem("s2"), CreateSkillAndGem("s3")};
            var supportingSkills = supports.Select(t => t.skill).ToList();
            var sut = CreateSut();

            var actual = sut.Parse(active, supportingSkills);

            actual.Keys.Should().Contain(supportingSkills);
            foreach (var support in supports)
            {
                BuildAndCalculate(actual[support.skill], support.gem).Should().BeEquivalentTo(new NodeValue(0));
            }
        }

        [TestCase("Gem.AdditionalLevels.Belt", 3)]
        [TestCase("Gem.AdditionalLevels.ActiveSkill.Belt", 4)]
        [TestCase("Gem.AdditionalLevels.ActiveSkill.g1", 5)]
        [TestCase("Gem.AdditionalLevels.ActiveSkill.spell.g2", 6)]
        [TestCase("Gem.AdditionalLevels.g2.Belt", 7)]
        public void GivenActiveSkillWithAdditionalLevelStatsAndNoSupportingSkills_WhenParsing_ThenValueIsAdditionalLevels(
            string statId, int statValue)
        {
            var (active, activeGem) = CreateSkillAndGem("a");
            var context = MockValueCalculationContextForActiveSkill(active,
                (statId, statValue));
            var sut = CreateSut(true);

            var actual = sut.Parse(active, new Skill[0]);
            var actualValue = actual[active].Build(BuildParameters(activeGem)).Calculate(context);

            actualValue.Should().BeEquivalentTo(new NodeValue(statValue));
        }

        [Test]
        public void GivenNonSpellActiveSkillWithAdditionalSpellLevels_WhenParsing_ThenValueIsZero()
        {
            var (active, activeGem) = CreateSkillAndGem("a");
            var context = MockValueCalculationContextForActiveSkill(active,
                ("Gem.AdditionalLevels.ActiveSkill.spell.g2", 2));
            var sut = CreateSut();

            var actual = sut.Parse(active, new Skill[0]);
            var actualValue = actual[active].Build(BuildParameters(activeGem)).Calculate(context);
            
            actualValue.Should().BeEquivalentTo(new NodeValue(0));
        }

        [Test]
        public void GivenActiveSecondarySkill_WhenParsing_ThenGemTagsOfGemAreUsed()
        {
            var activeGem = new Gem("a", 1, 0, ItemSlot.Belt, 0, 0, true);
            var active = Skill.SecondaryFromGem("b", activeGem, true);
            var context = MockValueCalculationContextForActiveSkill(active,
                ("Gem.AdditionalLevels.Belt", 2));
            var sut = CreateSut();

            var actual = sut.Parse(active, new Skill[0]);
            var actualValue = actual[active].Build(BuildParameters(activeGem)).Calculate(context);

            actualValue.Should().BeEquivalentTo(new NodeValue(2));
        }

        [Test]
        public void GivenActiveSkillWithoutBaseItem_WhenParsing_ThenNoGemTagsAreUsed()
        {
            var (active, activeGem) = CreateSkillAndGem("b");
            var context = MockValueCalculationContextForActiveSkill(active,
                ("Gem.AdditionalLevels.Belt", 2), ("Gem.AdditionalLevels.g2.Belt", 3));
            var sut = CreateSut();

            var actual = sut.Parse(active, new Skill[0]);
            var actualValue = actual[active].Build(BuildParameters(activeGem)).Calculate(context);

            actualValue.Should().BeEquivalentTo(new NodeValue(2));
        }

        [Test]
        public void GivenActiveSkillFromItem_WhenParsing_ThenValueIsZero()
        {
            var active = Skill.FromItem("a", 1, 0, ItemSlot.Belt, 0, true);
            var context = MockValueCalculationContextForActiveSkill(active,
                ("Gem.AdditionalLevels.Belt", 2));
            var sut = CreateSut();

            var actual = sut.Parse(active, new Skill[0]);
            var actualValue = actual[active].Build(new BuildParameters(new ModifierSource.Global(), default, default)).Calculate(context);
            
            actualValue.Should().BeEquivalentTo(new NodeValue(0));
        }

        [TestCase("Gem.AdditionalLevels.Belt", 3)]
        [TestCase("Gem.AdditionalLevels.g2.Belt", 7)]
        public void GivenSupportSkillWithAdditionalLevelStats_WhenParsing_ThenValueIsAdditionalLevels(
            string statId, int statValue)
        {
            var (active, _) = CreateSkillAndGem("a");
            var (support, supportGem) = CreateSkillAndGem("s1");
            var context = MockValueCalculationContextForActiveSkill(active,
                (statId, statValue));
            var sut = CreateSut(true);

            var actual = sut.Parse(active, new[] {support});
            var actualValue = actual[support].Build(BuildParameters(supportGem)).Calculate(context);

            actualValue.Should().BeEquivalentTo(new NodeValue(statValue));
        }

        [Test]
        public void GivenSupportSecondarySkill_WhenParsing_ThenGemTagsOfGemAreUsed()
        {
            var (active, _) = CreateSkillAndGem("a");
            var supportGem = new Gem("s1", 1, 0, ItemSlot.Belt, 0, 0, true);
            var support = Skill.SecondaryFromGem("s2", supportGem, true);
            var context = MockValueCalculationContextForActiveSkill(active,
                ("Gem.AdditionalLevels.Belt", 2));
            var sut = CreateSut();

            var actual = sut.Parse(active, new[] {support});
            var actualValue = actual[support].Build(BuildParameters(supportGem)).Calculate(context);

            actualValue.Should().BeEquivalentTo(new NodeValue(2));
        }

        [Test]
        public void GivenSupportSkillFromItem_WhenParsing_ThenValueIsZero()
        {
            var (active, _) = CreateSkillAndGem("a");
            var support = Skill.FromItem("s1", 1, 0, ItemSlot.Belt, 0, true);
            var context = MockValueCalculationContextForActiveSkill(active,
                ("Gem.AdditionalLevels.Belt", 2));
            var sut = CreateSut();

            var actual = sut.Parse(active, new[] {support});
            var actualValue = actual[support].Build(new BuildParameters(new ModifierSource.Global(), default, default)).Calculate(context);
            
            actualValue.Should().BeEquivalentTo(new NodeValue(0));
        }

        [TestCase("supported_active_skill_gem_level_+", 5)]
        [TestCase("supported_g1_skill_gem_level_+", 6)]
        public void GivenActiveSkillAffectedBySupportAddingLevels_WhenParsing_ThenValueIsIncreased(string statId, int value)
        {
            var (active, activeGem) = CreateSkillAndGem("a");
            var (support, _) = CreateSkillAndGem("s1");
            var levelDefinitions = new Dictionary<int, SkillLevelDefinition>
            {
                {1, CreateLevelDefinition(stats: new[] {new UntranslatedStat(statId, value),})}
            };
            var sut = CreateSut(supportLevelDefinitions: levelDefinitions);

            var actual = sut.Parse(active, new[] {support});

            BuildAndCalculate(actual[active], activeGem).Should().BeEquivalentTo(new NodeValue(value));
        }

        [TestCase("supported_g3_skill_gem_level_+", 2)]
        [TestCase("some_stat", 2)]
        public void GivenActiveSkillNotAffectedBySupportAddingLevels_WhenParsing_ThenValueIsNotIncreased(string statId, int value)
        {
            var (active, activeGem) = CreateSkillAndGem("a");
            var (support, _) = CreateSkillAndGem("s1");
            var levelDefinitions = new Dictionary<int, SkillLevelDefinition>
            {
                {1, CreateLevelDefinition(stats: new[] {new UntranslatedStat(statId, value),})}
            };
            var sut = CreateSut(supportLevelDefinitions: levelDefinitions);

            var actual = sut.Parse(active, new[] {support});

            BuildAndCalculate(actual[active], activeGem).Should().BeEquivalentTo(new NodeValue(0));
        }

        [Test]
        public void GivenActiveSkillAffectedBySupportAddingLevels_WhenParsing_ThenSupportLevelIsUsed()
        {
            var (active, activeGem) = CreateSkillAndGem("a");
            var (support, _) = CreateSkillAndGem("s1", level: 2);
            var levelDefinitions = new Dictionary<int, SkillLevelDefinition>
            {
                {1, CreateLevelDefinition(stats: new[] {new UntranslatedStat("supported_active_skill_gem_level_+", 1),})},
                {2, CreateLevelDefinition(stats: new[] {new UntranslatedStat("supported_active_skill_gem_level_+", 2),})},
            };
            var sut = CreateSut(supportLevelDefinitions: levelDefinitions);

            var actual = sut.Parse(active, new[] {support});

            BuildAndCalculate(actual[active], activeGem).Should().BeEquivalentTo(new NodeValue(2));
        }

        [Test]
        public void GivenActiveSkillAffectedBySupportAddingLevels_WhenParsing_ThenSupportAdditionalStatsAreUsed()
        {
            var (active, activeGem) = CreateSkillAndGem("a");
            var (support, _) = CreateSkillAndGem("s1");
            var levelDefinitions = new Dictionary<int, SkillLevelDefinition>
            {
                {1, CreateLevelDefinition(stats: new[] {new UntranslatedStat("supported_active_skill_gem_level_+", 1),})},
                {2, CreateLevelDefinition(stats: new[] {new UntranslatedStat("supported_active_skill_gem_level_+", 2),})},
                {3, CreateLevelDefinition(stats: new[] {new UntranslatedStat("supported_active_skill_gem_level_+", 3),})},
                {4, CreateLevelDefinition(stats: new[] {new UntranslatedStat("supported_active_skill_gem_level_+", 4),})},
            };
            var context = MockValueCalculationContextForActiveSkill(active,
                ("Gem.AdditionalLevels.g3.Belt", 2));
            var sut = CreateSut(supportLevelDefinitions: levelDefinitions);

            var actual = sut.Parse(active, new[] {support});
            var actualValue = actual[active].Build(BuildParameters(activeGem)).Calculate(context);
            
            actualValue.Should().BeEquivalentTo(new NodeValue(3));
        }

        [Test]
        public void GivenActiveSkillAffectedBySupportIncreasedToUnavailableLevel_WhenParsing_ThenMaximumAvailableSupportLevelIsUsed()
        {
            var (active, activeGem) = CreateSkillAndGem("a");
            var (support, _) = CreateSkillAndGem("s1");
            var levelDefinitions = new Dictionary<int, SkillLevelDefinition>
            {
                {1, CreateLevelDefinition(stats: new[] {new UntranslatedStat("supported_active_skill_gem_level_+", 1),})},
                {2, CreateLevelDefinition(stats: new[] {new UntranslatedStat("supported_active_skill_gem_level_+", 2),})},
                {4, CreateLevelDefinition(stats: new[] {new UntranslatedStat("supported_active_skill_gem_level_+", 4),})},
            };
            var context = MockValueCalculationContextForActiveSkill(active,
                ("Gem.AdditionalLevels.g3.Belt", 2));
            var sut = CreateSut(supportLevelDefinitions: levelDefinitions);

            var actual = sut.Parse(active, new[] {support});
            var actualValue = actual[active].Build(BuildParameters(activeGem)).Calculate(context);
            
            actualValue.Should().BeEquivalentTo(new NodeValue(2));
        }

        private static BuildParameters BuildParameters(Gem gem) =>
            new BuildParameters(new ModifierSource.Global(new ModifierSource.Local.Gem(gem)), Entity.Character, default);

        private static (Skill skill, Gem gem) CreateSkillAndGem(string id, int level = 1)
        {
            var gem = new Gem(id, level, 0, ItemSlot.Belt, 0, 0, true);
            return (Skill.FromGem(gem, true), gem);
        }

        private static AdditionalSkillLevelParser CreateSut(
            bool isSpell = false, Dictionary<int, SkillLevelDefinition>? supportLevelDefinitions = null)
        {
            supportLevelDefinitions ??= new Dictionary<int, SkillLevelDefinition>();
            var skillDefinitions = new SkillDefinitions(new[]
            {
                SkillDefinition.CreateActive("a", 0, "", "b", Array.Empty<string>(),
                    new SkillBaseItemDefinition("a", "a", ReleaseState.Released, new[] {"g1", "g2", isSpell ? "spell" : "attack"}),
                    CreateActiveSkillDefinition("a"), new Dictionary<int, SkillLevelDefinition>()),
                SkillDefinition.CreateActive("b", 1, "", null, Array.Empty<string>(),
                    null, CreateActiveSkillDefinition("b"), new Dictionary<int, SkillLevelDefinition>()),
                SkillDefinition.CreateSupport("s1", 2, "", "s2", Array.Empty<string>(),
                    new SkillBaseItemDefinition("s1", "s1", ReleaseState.Released, new[] {"g2", "g3"}),
                    CreateSupportSkillDefinition(), supportLevelDefinitions),
                SkillDefinition.CreateSupport("s2", 3, "", null, Array.Empty<string>(),
                    null, CreateSupportSkillDefinition(), supportLevelDefinitions),
                SkillDefinition.CreateSupport("s3", 4, "", null, Array.Empty<string>(),
                    null, CreateSupportSkillDefinition(), supportLevelDefinitions),
            });
            return new AdditionalSkillLevelParser(skillDefinitions,
                new GemStatBuilders(new StatFactory()),
                new GemTagBuilders(),
                new ValueBuilders());
        }

        private static NodeValue? BuildAndCalculate(IValueBuilder valueBuilder, Gem gem) =>
            valueBuilder.Build(BuildParameters(gem)).Calculate(Mock.Of<IValueCalculationContext>());
    }
}