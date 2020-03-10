﻿using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class SkillModificationParser
    {
        private readonly IBuilderFactories _builderFactories;
        private readonly AdditionalSkillLevelParser _levelParser;
        private readonly AdditionalSkillQualityParser _qualityParser;

        public SkillModificationParser(SkillDefinitions skillDefinitions, IBuilderFactories builderFactories)
        {
            _builderFactories = builderFactories;
            _levelParser = new AdditionalSkillLevelParser(skillDefinitions, builderFactories.StatBuilders.Gem, builderFactories.GemTagBuilders,
                builderFactories.ValueBuilders);
            _qualityParser = new AdditionalSkillQualityParser(skillDefinitions, builderFactories.StatBuilders.Gem, builderFactories.ValueBuilders);
        }

        public ParseResult Parse(
            Skill activeSkill, IReadOnlyList<Skill> supportingSkills, Entity modifierSourceEntity)
        {
            var levelValues = _levelParser.Parse(activeSkill, supportingSkills, modifierSourceEntity);
            var qualityValues = _qualityParser.Parse(activeSkill, supportingSkills, modifierSourceEntity);

            var levelModifiers = levelValues.Select(p => CreateAdditionalLevelModifier(p.Key, p.Value, modifierSourceEntity));
            var qualityModifiers = qualityValues.Select(p => CreateAdditionalQualityModifier(p.Key, p.Value, modifierSourceEntity));
            var modifiers = levelModifiers.Concat(qualityModifiers).ToList();

            return ParseResult.Success(modifiers);
        }

        private Modifier CreateAdditionalLevelModifier(Skill skill, IValue value, Entity modifierSourceEntity) =>
            CreateAdditionalStatModifier(_builderFactories.StatBuilders.Gem.AdditionalLevels(skill), value, modifierSourceEntity);

        private Modifier CreateAdditionalQualityModifier(Skill skill, IValue value, Entity modifierSourceEntity) =>
            CreateAdditionalStatModifier(_builderFactories.StatBuilders.Gem.AdditionalQuality(skill), value, modifierSourceEntity);

        private static Modifier CreateAdditionalStatModifier(IStatBuilder statBuilder, IValue value, Entity modifierSourceEntity)
        {
            var stats = statBuilder.BuildToStats(modifierSourceEntity).ToList();
            var modifierSource = new ModifierSource.Global(new ModifierSource.Local.Given());
            return new Modifier(stats, Form.TotalOverride, value, modifierSource);
        }
    }
}