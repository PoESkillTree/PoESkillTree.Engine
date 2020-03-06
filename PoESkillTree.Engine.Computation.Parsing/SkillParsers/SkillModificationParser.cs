using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class SkillModificationParser
    {
        private readonly IValueCalculationContext _valueCalculationContext;
        private readonly AdditionalSkillLevelParser _levelParser;

        public SkillModificationParser(
            SkillDefinitions skillDefinitions, IBuilderFactories builderFactories, IValueCalculationContext valueCalculationContext)
        {
            _valueCalculationContext = valueCalculationContext;
            _levelParser = new AdditionalSkillLevelParser(skillDefinitions, builderFactories.StatBuilders.Gem, builderFactories.GemTagBuilders,
                builderFactories.ValueBuilders);
        }

        public (ParseResult, IReadOnlyDictionary<Skill, SkillModification>) Parse(
            Skill activeSkill, IReadOnlyList<Skill> supportingSkills, Entity modifierSourceEntity)
        {
            var levels = _levelParser.Parse(activeSkill, supportingSkills, modifierSourceEntity);
            var modifications = levels.ToDictionary(p => p.Key,
                p => new SkillModification(CalculateAdditionalLevel(p.Value)));
            return (ParseResult.Empty, modifications);
        }

        private int CalculateAdditionalLevel(IValue value) =>
            (int) (value.Calculate(_valueCalculationContext).SingleOrNull() ?? 0);
    }
}