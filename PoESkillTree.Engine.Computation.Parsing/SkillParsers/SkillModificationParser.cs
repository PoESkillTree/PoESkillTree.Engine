using System.Collections.Generic;
using System.Linq;
using MoreLinq;
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
        private readonly IValueCalculationContext _valueCalculationContext;
        private readonly AdditionalSkillLevelParser _levelParser;
        private readonly AdditionalSkillQualityParser _qualityParser;

        public SkillModificationParser(
            SkillDefinitions skillDefinitions, IBuilderFactories builderFactories, IValueCalculationContext valueCalculationContext)
        {
            _builderFactories = builderFactories;
            _valueCalculationContext = valueCalculationContext;
            _levelParser = new AdditionalSkillLevelParser(skillDefinitions, builderFactories.StatBuilders.Gem, builderFactories.GemTagBuilders,
                builderFactories.ValueBuilders);
            _qualityParser = new AdditionalSkillQualityParser(skillDefinitions, builderFactories.StatBuilders.Gem, builderFactories.ValueBuilders);
        }

        public (ParseResult, IReadOnlyDictionary<Skill, SkillModification>) Parse(
            Skill activeSkill, IReadOnlyList<Skill> supportingSkills, Entity modifierSourceEntity)
        {
            var levelValues = _levelParser.Parse(activeSkill, supportingSkills, modifierSourceEntity);
            var levels = levelValues.ToDictionary(p => p.Key, p => CalculateAdditionalStat(p.Value));
            var qualityValues = _qualityParser.Parse(activeSkill, levels, modifierSourceEntity);
            var qualities = qualityValues.ToDictionary(p => p.Key, p => CalculateAdditionalStat(p.Value));

            var levelModifiers = levelValues.Select(p => CreateAdditionalLevelModifier(p.Key, p.Value, modifierSourceEntity));
            var qualityModifiers = qualityValues.Select(p => CreateAdditionalQualityModifier(p.Key, p.Value, modifierSourceEntity));
            var modifiers = levelModifiers.Concat(qualityModifiers).ToList();

            var modifications = Enumerable.Prepend(supportingSkills, activeSkill)
                .Select(s => (s, new SkillModification(levels[s], qualities[s])))
                .ToDictionary();
            return (ParseResult.Success(modifiers), modifications);
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

        private int CalculateAdditionalStat(IValue value) =>
            (int) (value.Calculate(_valueCalculationContext).SingleOrNull() ?? 0);
    }
}