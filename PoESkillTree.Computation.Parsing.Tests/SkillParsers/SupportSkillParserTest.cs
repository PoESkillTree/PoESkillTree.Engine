﻿using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PoESkillTree.Computation.Builders;
using PoESkillTree.Computation.Builders.Stats;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.GameModel;
using PoESkillTree.GameModel.Items;
using PoESkillTree.GameModel.PassiveTree;
using PoESkillTree.GameModel.Skills;
using PoESkillTree.GameModel.StatTranslation;
using static PoESkillTree.Computation.Common.Helper;
using static PoESkillTree.Computation.Parsing.ParserTestUtils;
using static PoESkillTree.Computation.Parsing.SkillParsers.SkillParserTestUtils;

namespace PoESkillTree.Computation.Parsing.SkillParsers
{
    [TestFixture]
    public class SupportSkillParserTest
    {
        #region Blasphemy

        [Test]
        public void BlasphemyAddsAuraKeyword()
        {
            var (activeDefinition, activeSkill) = CreateEnfeebleDefinition();
            var (supportDefinition, supportSkill) = CreateBlasphemyDefinition();
            var sut = CreateSut(activeDefinition, supportDefinition);

            var result = sut.Parse(activeSkill, supportSkill);

            Assert.IsTrue(AnyModifierHasIdentity(result.Modifiers, "MainSkill.Has.Aura"));
        }

        [Test]
        public void BlasphemyOverridesBaseCost()
        {
            var (activeDefinition, activeSkill) = CreateEnfeebleDefinition();
            var (supportDefinition, supportSkill) = CreateBlasphemyDefinition();
            var sut = CreateSut(activeDefinition, supportDefinition);

            var result = sut.Parse(activeSkill, supportSkill);

            var modifier = GetFirstModifierWithIdentity(result.Modifiers, "Belt.0.Cost");
            Assert.AreEqual(Form.TotalOverride, modifier.Form);
            Assert.AreEqual(new NodeValue(42), modifier.Value.Calculate(null));
        }

        [Test]
        public void BlasphemyAddsSkillTypes()
        {
            var (activeDefinition, activeSkill) = CreateEnfeebleDefinition();
            var (supportDefinition, supportSkill) = CreateBlasphemyDefinition();
            var sut = CreateSut(activeDefinition, supportDefinition);

            var result = sut.Parse(activeSkill, supportSkill);

            var modifier = GetFirstModifierWithIdentity(result.Modifiers,
                $"Belt.0.Type.{ActiveSkillType.ManaCostIsReservation}");
            Assert.AreEqual(new NodeValue(1), modifier.Value.Calculate(null));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BlasphemySetsActiveSkill(bool isActiveSkill)
        {
            var expectedItemSlot = isActiveSkill ? (NodeValue?) (int) ItemSlot.Belt : null;
            var expectedSocketIndex = isActiveSkill ? (NodeValue?) 1 : null;
            var (activeDefinition, activeSkill) = CreateEnfeebleDefinition();
            var (supportDefinition, supportSkill) = CreateBlasphemyDefinition();
            var sut = CreateSut(activeDefinition, supportDefinition);
            var context = MockValueCalculationContext(activeSkill, false, isActiveSkill);

            var result = sut.Parse(activeSkill, supportSkill);

            var actualItemSlot =
                GetValueForIdentity(result.Modifiers, "Blasphemy.ActiveSkillItemSlot").Calculate(context);
            Assert.AreEqual(expectedItemSlot, actualItemSlot);
            var actualSocketIndex =
                GetValueForIdentity(result.Modifiers, "Blasphemy.ActiveSkillSocketIndex").Calculate(context);
            Assert.AreEqual(expectedSocketIndex, actualSocketIndex);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BlasphemyParsesPassiveStatsWithParsedSkillIsActiveSkillCondition(bool isActiveSkill)
        {
            var expected = isActiveSkill ? (NodeValue?) 10 : null;
            var (activeDefinition, activeSkill) = CreateEnfeebleDefinition();
            var (supportDefinition, supportSkill) = CreateBlasphemyDefinition();
            var source = new ModifierSource.Local.Skill("Enfeeble", null);
            var parseResult = ParseResult.Success(new[]
                { MockModifier(new Stat("Blasphemy.EffectOn(Character)"), value: new Constant(10)) });
            var parameter = new UntranslatedStatParserParameter(source, new[]
                { new UntranslatedStat("curse_effect_+%", 10), });
            var statParser = Mock.Of<IParser<UntranslatedStatParserParameter>>(p =>
                p.Parse(parameter) == parseResult &&
                p.Parse(EmptyParserParameter(source)) == EmptyParseResult);
            var sut = CreateSut(activeDefinition, supportDefinition, statParser);
            var context = MockValueCalculationContextForActiveSkill(activeSkill,
                ("Blasphemy.ActiveSkillItemSlot", isActiveSkill ? (double?) supportSkill.ItemSlot : null),
                ("Blasphemy.ActiveSkillSocketIndex", 1));

            var result = sut.Parse(activeSkill, supportSkill);

            var actual = GetValueForIdentity(result.Modifiers, "Blasphemy.EffectOn(Character)").Calculate(context);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BlasphemyAddsToAuraSkillInstances(bool isActiveSkill)
        {
            var expected = isActiveSkill ? (NodeValue?) 1 : null;
            var (activeDefinition, activeSkill) = CreateEnfeebleDefinition();
            var (supportDefinition, supportSkill) = CreateBlasphemyDefinition();
            var sut = CreateSut(activeDefinition, supportDefinition);
            var context = MockValueCalculationContext(activeSkill, false, isActiveSkill);

            var result = sut.Parse(activeSkill, supportSkill);

            var actual = GetValueForIdentity(result.Modifiers, "Skills[Aura].Instances").Calculate(context);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BlasphemyDoesNotAddToAuraSkillInstancesWhenSupportingAuras()
        {
            var (activeDefinition, activeSkill) = CreateClarityDefinition();
            var (supportDefinition, supportSkill) = CreateBlasphemyDefinition();
            var sut = CreateSut(activeDefinition, supportDefinition);

            var result = sut.Parse(activeSkill, supportSkill);

            Assert.False(AnyModifierHasIdentity(result.Modifiers, "Skills[Aura].Instances"));
        }

        [Test]
        public void ParseReturnsEmptyResultForDisabledActiveSkill()
        {
            var (activeDefinition, activeSkill) = CreateClarityDefinition(false);
            var (supportDefinition, supportSkill) = CreateBlasphemyDefinition();
            var sut = CreateSut(activeDefinition, supportDefinition);

            var result = sut.Parse(activeSkill, supportSkill);

            Assert.IsEmpty(result.Modifiers);
        }

        [Test]
        public void ParseReturnsEmptyResultForDisabledSupportSkill()
        {
            var (activeDefinition, activeSkill) = CreateClarityDefinition();
            var (supportDefinition, supportSkill) = CreateBlasphemyDefinition(false);
            var sut = CreateSut(activeDefinition, supportDefinition);

            var result = sut.Parse(activeSkill, supportSkill);

            Assert.IsEmpty(result.Modifiers);
        }

        private static (SkillDefinition, Skill) CreateEnfeebleDefinition()
        {
            var activeSkill = CreateActiveSkillDefinition("Enfeeble", new[] { "curse" }, new[] { Keyword.Curse },
                providesBuff: true);
            var level = CreateLevelDefinition();
            var levels = new Dictionary<int, SkillLevelDefinition> { { 1, level } };
            return (CreateActive("Enfeeble", activeSkill, levels),
                new Skill("Enfeeble", 1, 0, ItemSlot.Belt, 0, null));
        }

        private static (SkillDefinition, Skill) CreateClarityDefinition(bool isEnabled = true)
        {
            var activeSkill = CreateActiveSkillDefinition("Clarity", new[] { "aura" }, new[] { Keyword.Aura },
                providesBuff: true);
            var level = CreateLevelDefinition();
            var levels = new Dictionary<int, SkillLevelDefinition> { { 1, level } };
            return (CreateActive("Clarity", activeSkill, levels),
                new Skill("Clarity", 1, 0, ItemSlot.Belt, 0, null, isEnabled));
        }

        private static (SkillDefinition, Skill) CreateBlasphemyDefinition(bool isEnabled = true)
        {
            var supportSkill = CreateSupportSkillDefinition(
                addedActiveSkillTypes: new[]
                    { ActiveSkillType.ManaCostIsReservation, ActiveSkillType.ManaCostIsPercentage },
                addedKeywords: new[] { Keyword.Aura });
            var qualityPassiveStats = new[] { new UntranslatedStat("curse_effect_+%", 500),  };
            var level = CreateLevelDefinition(manaCostOverride: 42, qualityPassiveStats: qualityPassiveStats);
            var levels = new Dictionary<int, SkillLevelDefinition> { { 1, level } };
            return (CreateSupport("Blasphemy", supportSkill, levels),
                new Skill("Blasphemy", 1, 20, ItemSlot.Belt, 1, null, isEnabled));
        }

        #endregion

        #region Physical to Lightning

        [Test]
        public void PhysicalToLightningConversionIsLocal()
        {
            var (activeDefinition, activeSkill) = CreateEnfeebleDefinition();
            var (supportDefinition, supportSkill) = CreatePhysicalToLightningDefinition();
            var sut = CreateSut(activeDefinition, supportDefinition);

            var result = sut.Parse(activeSkill, supportSkill);

            var modifiers = result.Modifiers;
            var expectedIdentity =
                "Physical.Damage.Attack.MainHand.Skill.ConvertTo(Lightning.Damage.Attack.MainHand.Skill)";
            Assert.IsTrue(AnyModifierHasIdentity(modifiers, expectedIdentity));
            var modifier = GetFirstModifierWithIdentity(modifiers, expectedIdentity);
            Assert.IsInstanceOf<ModifierSource.Local.Skill>(modifier.Source);
        }

        private static (SkillDefinition, Skill) CreatePhysicalToLightningDefinition()
        {
            var stats = new[]
            {
                new UntranslatedStat("skill_physical_damage_%_to_convert_to_lightning", 50),
            };
            var level = CreateLevelDefinition(stats: stats);
            var levels = new Dictionary<int, SkillLevelDefinition> { { 1, level } };
            return (CreateSupport("SupportPhysicalToLightning", CreateSupportSkillDefinition(), levels),
                new Skill("SupportPhysicalToLightning", 1, 0, ItemSlot.Belt, 1, null));
        }

        #endregion

        #region Blood Magic

        [TestCase(true)]
        [TestCase(false)]
        public void BloodMagicOverridesReservationPool(bool isActive)
        {
            var expected = isActive ? (NodeValue?) (double) Pool.Life : null;
            var (activeDefinition, activeSkill) = CreateEnfeebleDefinition();
            var (supportDefinition, supportSkill) = CreateBloodMagicDefinition();
            var sut = CreateSut(activeDefinition, supportDefinition);
            var context = MockValueCalculationContext(activeSkill, false, isActive);

            var result = sut.Parse(activeSkill, supportSkill);

            var modifier = GetValueForIdentity(result.Modifiers, "Enfeeble.ReservationPool");
            Assert.AreEqual(expected, modifier.Calculate(context));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BloodMagicOverridesCostPool(bool isMain)
        {
            var expected = isMain ? (NodeValue?) 100 : null;
            var (activeDefinition, activeSkill) = CreateEnfeebleDefinition();
            var (supportDefinition, supportSkill) = CreateBloodMagicDefinition();
            var sut = CreateSut(activeDefinition, supportDefinition);
            var context = MockValueCalculationContext(activeSkill, isMain, false);

            var result = sut.Parse(activeSkill, supportSkill);

            var modifier = GetValueForIdentity(result.Modifiers, "Mana.Cost.ConvertTo(Life.Cost)");
            Assert.AreEqual(expected, modifier.Calculate(context));
        }

        private static (SkillDefinition, Skill) CreateBloodMagicDefinition()
        {
            var stats = new[]
            {
                new UntranslatedStat("base_use_life_in_place_of_mana", 1),
            };
            var level = CreateLevelDefinition(stats: stats);
            var levels = new Dictionary<int, SkillLevelDefinition> { { 1, level } };
            return (CreateSupport("SupportBloodMagic", CreateSupportSkillDefinition(), levels),
                new Skill("SupportBloodMagic", 1, 0, ItemSlot.Belt, 1, null));
        }

        #endregion

        #region Point Blank

        [Test]
        public void PointBlankTranslatesKeystoneStatCorrectly()
        {
            var (activeDefinition, activeSkill) = CreateEnfeebleDefinition();
            var (supportDefinition, supportSkill) = CreatePointBlankDefinition();
            var source = new ModifierSource.Local.Skill("Enfeeble", null);
            var parseResult = ParseResult.Success(new[]
                { CreateModifier("PointBlank", Form.TotalOverride, 1) });
            var parameter = new UntranslatedStatParserParameter(source, new[]
                { new UntranslatedStat("keystone_point_blank", 1), });
            var mainStatParser = Mock.Of<IParser<UntranslatedStatParserParameter>>(p =>
                p.Parse(parameter) == parseResult &&
                p.Parse(EmptyParserParameter(source)) == EmptyParseResult);
            var emptyStatParser = Mock.Of<IParser<UntranslatedStatParserParameter>>(p =>
                p.Parse(EmptyParserParameter(source)) == EmptyParseResult);
            var sut = CreateSut(activeDefinition, supportDefinition, GetStatParser);

            var result = sut.Parse(activeSkill, supportSkill);

            Assert.IsTrue(AnyModifierHasIdentity(result.Modifiers, "PointBlank"));

            IParser<UntranslatedStatParserParameter> GetStatParser(IReadOnlyList<string> translationFileNames)
                => translationFileNames[0] == StatTranslationFileNames.Main
                    ? mainStatParser
                    : emptyStatParser;
        }

        private static (SkillDefinition, Skill) CreatePointBlankDefinition()
        {
            var stats = new[]
            {
                new UntranslatedStat("keystone_point_blank", 1),
            };
            var level = CreateLevelDefinition(stats: stats);
            var levels = new Dictionary<int, SkillLevelDefinition> { { 1, level } };
            return (CreateSupport("SupportPointBlank", CreateSupportSkillDefinition(), levels),
                new Skill("SupportPointBlank", 1, 0, ItemSlot.Belt, 1, null));
        }

        #endregion

        private static SupportSkillParser CreateSut(SkillDefinition activeSkillDefinition, SkillDefinition supportSkillDefinition)
        {
            var statParser = Mock.Of<IParser<UntranslatedStatParserParameter>>(p =>
                p.Parse(It.IsAny<UntranslatedStatParserParameter>()) == ParseResult.Empty);
            return CreateSut(activeSkillDefinition, supportSkillDefinition, _ => statParser);
        }

        private static SupportSkillParser CreateSut(
            SkillDefinition activeSkillDefinition, SkillDefinition supportSkillDefinition,
            IParser<UntranslatedStatParserParameter> statParser)
            => CreateSut(activeSkillDefinition, supportSkillDefinition, _ => statParser);

        private static SupportSkillParser CreateSut(
            SkillDefinition activeSkillDefinition, SkillDefinition supportSkillDefinition,
            UntranslatedStatParserFactory statParserFactory)
        {
            var skillDefinitions = new SkillDefinitions(new[] { activeSkillDefinition, supportSkillDefinition });
            var builderFactories =
                new BuilderFactories(new PassiveTreeDefinition(new PassiveNodeDefinition[0]), skillDefinitions);
            return new SupportSkillParser(skillDefinitions, builderFactories, statParserFactory);
        }
    }
}