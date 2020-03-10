using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Builders.Values;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.GameModel.Skills;
using static PoESkillTree.Engine.Computation.Parsing.ParserTestUtils;
using static PoESkillTree.Engine.Computation.Parsing.SkillParsers.SkillParserTestUtils;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    [TestFixture]
    public class AdditionalSkillLevelMaximumParserTest
    {
        [TestCase(1, ExpectedResult = 1)]
        [TestCase(2, ExpectedResult = 2)]
        [TestCase(1, 2, ExpectedResult = 2)]
        [TestCase(2, 1, ExpectedResult = 2)]
        public int? GivenActiveSkill_WhenParsing_ThenValueIsMaximumLevel(params int[] levels)
        {
            var active = CreateSkillFromGem("a");
            var context = MockValueCalculationContextForActiveSkill(active);
            var sut = CreateSutForActive(levels);

            var (_, _, modifiers) = sut.Parse(active, Array.Empty<Skill>(), default);

            return (int?) GetValueForIdentity(modifiers, StatIdentity(active)).Calculate(context).SingleOrNull();
        }

        [TestCase(1, ExpectedResult = 1)]
        [TestCase(2, ExpectedResult = 2)]
        public int? GivenSupportSkill_WhenParsing_ThenValueIsMaximumLevel(params int[] levels)
        {
            var active = CreateSkillFromGem("a");
            var support = CreateSkillFromGem("s");
            var context = MockValueCalculationContextForActiveSkill(active);
            var sut = CreateSutForSupport(levels);

            var (_, _, modifiers) = sut.Parse(active, new[] {support}, default);

            return (int?) GetValueForIdentity(modifiers, StatIdentity(support)).Calculate(context).SingleOrNull();
        }

        private static Skill CreateSkillFromGem(string id) =>
            Skill.FromGem(new Gem(id, 1, 0, ItemSlot.Belt, id.GetHashCode(), 0, true), true);

        private static AdditionalSkillLevelMaximumParser CreateSutForActive(params int[] activeLevels) =>
            CreateSut(activeLevels, new[] {1});

        private static AdditionalSkillLevelMaximumParser CreateSutForSupport(params int[] supportLevels) =>
            CreateSut(new[] {1}, supportLevels);

        private static AdditionalSkillLevelMaximumParser CreateSut(IEnumerable<int> activeLevels, IEnumerable<int> supportLevels)
        {
            var skillDefinitions = new SkillDefinitions(new[]
            {
                CreateActive("a", CreateActiveSkillDefinition("a"), activeLevels.ToDictionary(l => l, _ => CreateLevelDefinition())),
                CreateSupport("s", CreateSupportSkillDefinition(), supportLevels.ToDictionary(l => l, _ => CreateLevelDefinition())),
            });
            var statFactory = new StatFactory();
            return new AdditionalSkillLevelMaximumParser(skillDefinitions,
                new GemStatBuilders(statFactory),
                new ValueBuilders());
        }

        private static string StatIdentity(Skill skill) =>
            $"Skill.AdditionalLevels.{skill.ItemSlot}.{skill.SocketIndex}.{skill.SkillIndex}.Maximum";
    }
}