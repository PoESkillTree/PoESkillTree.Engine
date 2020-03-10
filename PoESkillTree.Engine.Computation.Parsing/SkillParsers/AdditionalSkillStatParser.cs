using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class AdditionalSkillStatParser : IParser<AdditionalSkillStatParserParameter>
    {
        private readonly AdditionalSkillLevelParser _levelParser;
        private readonly AdditionalSkillQualityParser _qualityParser;

        public AdditionalSkillStatParser(SkillDefinitions skillDefinitions, IBuilderFactories builderFactories)
        {
            _levelParser = new AdditionalSkillLevelParser(skillDefinitions, builderFactories.StatBuilders.Gem, builderFactories.GemTagBuilders,
                builderFactories.ValueBuilders);
            _qualityParser = new AdditionalSkillQualityParser(skillDefinitions, builderFactories.StatBuilders.Gem, builderFactories.ValueBuilders);
        }

        public ParseResult Parse(AdditionalSkillStatParserParameter parameter)
        {
            var (activeSkill, supportingSkills, modifierSourceEntity) = parameter;
            var results = new[]
            {
                _levelParser.Parse(activeSkill, supportingSkills, modifierSourceEntity),
                _qualityParser.Parse(activeSkill, supportingSkills, modifierSourceEntity),
            };
            return ParseResult.Aggregate(results);
        }
    }

    public static class AdditionalSkillStatParserExtensions
    {
        public static ParseResult Parse(this IParser<AdditionalSkillStatParserParameter> @this,
            Skill activeSkill, IReadOnlyList<Skill> supportingSkills, Entity entity) =>
            @this.Parse(new AdditionalSkillStatParserParameter(activeSkill, supportingSkills, entity));
    }

    public class AdditionalSkillStatParserParameter : ValueObject
    {
        public AdditionalSkillStatParserParameter(Skill activeSkill, IReadOnlyList<Skill> supportingSkills, Entity entity)
            => (ActiveSkill, SupportingSkills, Entity) = (activeSkill, supportingSkills, entity);

        public void Deconstruct(out Skill activeSkill, out IReadOnlyList<Skill> supportingSkills, out Entity entity)
            => (activeSkill, supportingSkills, entity) = (ActiveSkill, SupportingSkills, Entity);

        public Skill ActiveSkill { get; }
        public IReadOnlyList<Skill> SupportingSkills { get; }
        public Entity Entity { get; }

        protected override object ToTuple() => (ActiveSkill, WithSequenceEquality(SupportingSkills), Entity);
    }
}