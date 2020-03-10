using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class AdditionalSkillStatParser : IParser<AdditionalSkillStatParserParameter>
    {
        private readonly IReadOnlyList<IParser<AdditionalSkillStatParserParameter>> _parsers;

        public AdditionalSkillStatParser(SkillDefinitions skillDefinitions, IBuilderFactories builderFactories)
        {
            _parsers = new IParser<AdditionalSkillStatParserParameter>[]
            {
                new AdditionalSkillLevelParser(skillDefinitions, builderFactories.StatBuilders.Gem, builderFactories.GemTagBuilders,
                    builderFactories.ValueBuilders, builderFactories.MetaStatBuilders),
                new AdditionalSkillLevelMaximumParser(skillDefinitions, builderFactories.StatBuilders.Gem, builderFactories.ValueBuilders),
                new AdditionalSkillQualityParser(skillDefinitions, builderFactories.StatBuilders.Gem,
                    builderFactories.ValueBuilders, builderFactories.MetaStatBuilders),
            };
        }

        public ParseResult Parse(AdditionalSkillStatParserParameter parameter) =>
            ParseResult.Aggregate(_parsers.Select(p => p.Parse(parameter)));
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