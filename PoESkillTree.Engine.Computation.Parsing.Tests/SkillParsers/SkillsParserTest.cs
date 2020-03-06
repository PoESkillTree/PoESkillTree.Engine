using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PoESkillTree.Engine.Computation.Builders;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.GameModel.PassiveTree;
using PoESkillTree.Engine.GameModel.Skills;
using static PoESkillTree.Engine.Computation.Common.Helper;
using static PoESkillTree.Engine.Computation.Parsing.SkillParsers.SkillParserTestUtils;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    [TestFixture]
    public class SkillsParserTest
    {
        [Test]
        public void ParsesSingleActiveSkillCorrectly()
        {
            var expected = CreateParseResultForActive("0").Modifiers;
            var skill = CreateSkill("0", 0);
            var sut = CreateSut();

            var actual = Parse(sut, new[] { skill });

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParsesSingleSupportedActiveSkillCorrectly()
        {
            var expected = CreateParseResult("0", "a", "b");
            var active = CreateSkill("0", 0);
            var supports = new[] { CreateSkill("a", 0), CreateSkill("b", 0) };
            var sut = CreateSut();

            var actual = Parse(sut, supports.Prepend(active).ToList());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UsesSupportabilityTester()
        {
            var expected = CreateParseResult("0", "b");
            var active = CreateSkill("0", 0);
            var supports = new[] { CreateSkill("a", 1), CreateSkill("b", 0) };
            var sut = CreateSut();

            var actual = Parse(sut, supports.Prepend(active).ToList());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseMultipleSupportedActiveSkillsCorrectly()
        {
            var expected = CreateParseResult("0", "b", "c")
                    .Concat(CreateParseResult("1", "b"))
                    .Concat(CreateParseResult("2", "b", "a", "d"));
            var actives = new[]
            {
                CreateSkill("0", 0),
                CreateSkill("1", 1),
                CreateSkill("2", 2),
            };
            var supports = new[]
            {
                CreateSkill("a", 2), 
                CreateSkill("b", null),
                CreateSkill("c", 0),
                CreateSkill("d", 2),
            };
            var sut = CreateSut();

            var actual = Parse(sut, actives.Concat(supports).ToList());

            Assert.AreEqual(expected, actual);
        }

        private static SkillsParser CreateSut()
        {
            var skillDefinitions = CreateSkillDefinitions();
            var activeParser = new Mock<IParser<ActiveSkillParserParameter>>();
            activeParser.Setup(p => p.Parse(It.IsAny<ActiveSkillParserParameter>()))
                .Returns((ActiveSkillParserParameter p) => CreateParseResultForActive(p.ActiveSkill.Id));
            var supportParser = new Mock<IParser<SupportSkillParserParameter>>();
            supportParser.Setup(p => p.Parse(It.IsAny<SupportSkillParserParameter>()))
                .Returns((SupportSkillParserParameter p)
                    => CreateParseResultForSupport(p.ActiveSkill.Id, p.SupportSkill.Id));
            var skillModificationParser = new SkillModificationParser(skillDefinitions,
                new BuilderFactories(new PassiveTreeDefinition(new PassiveNodeDefinition[0]), skillDefinitions),
                Mock.Of<IValueCalculationContext>());
            return new SkillsParser(skillDefinitions, activeParser.Object, supportParser.Object, skillModificationParser);
        }

        private static SkillDefinitions CreateSkillDefinitions()
        {
            var actives = Enumerable.Range(0, 3).Select(i => CreateActive(
                i.ToString(),
                CreateActiveSkillDefinition(i.ToString(), activeSkillTypes: new[] { "ast" }),
                new Dictionary<int, SkillLevelDefinition>()));
            var supports = Enumerable.Range(0, 4).Select(i => CreateSupport(
                ((char) (i + 97)).ToString(),
                CreateSupportSkillDefinition(new[] { "ast" }),
                new Dictionary<int, SkillLevelDefinition>()));
            return new SkillDefinitions(actives.Concat(supports).ToList());
        }

        private static Skill CreateSkill(string id, int? gemGroup)
        {
            if (gemGroup.HasValue)
                return Skill.FromGem(new Gem(id, 1, 0, ItemSlot.Belt, 0, gemGroup.Value, true), true);
            else
                return Skill.FromItem(id, 1, 0, ItemSlot.Belt, 0, true);
        }

        private static ParseResult CreateParseResultForActive(string activeId)
            => ParseResult.Success(new[] { CreateModifier(activeId) });

        private static ParseResult CreateParseResultForSupport(string activeId, string supportId)
            => ParseResult.Success(new[] { CreateModifier($"{activeId} {supportId}") });

        private static IEnumerable<Modifier> CreateParseResult(string activeId, params string[] supportIds)
        {
            return supportIds
                .Select(s => CreateModifier($"{activeId} {s}"))
                .Prepend(CreateModifier(activeId));
        }

        private static Modifier CreateModifier(string id)
            => MockModifier(new Stat(id), value: new Constant(0));

        public static IEnumerable<Modifier> Parse(IParser<SkillsParserParameter> sut, IReadOnlyList<Skill> skills)
        {
            var result = sut.Parse(skills, Entity.Character);
            return result.Modifiers.Where(m => m.Stats.All(s => !s.Identity.Contains("Skill.Additional")));
        }
    }
}