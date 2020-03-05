using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    /// <summary>
    /// Parses a group of skills at once. E.g. all equipped skills or all skills for one ItemSlot.
    /// </summary>
    public class SkillsParser : IParser<SkillsParserParameter>
    {
        public delegate AdditionalSkillLevels AdditionalSkillLevelParserDelegate(
            Skill activeSkill, IReadOnlyList<Skill> supportingSkills, Entity modifierSourceEntity);

        private readonly SkillDefinitions _skillDefinitions;
        private readonly SupportabilityTester _supportabilityTester;
        private readonly IParser<ActiveSkillParserParameter> _activeSkillParser;
        private readonly IParser<SupportSkillParserParameter> _supportSkillParser;
        private readonly AdditionalSkillLevelParserDelegate _additionalSkillLevelParser;

        public SkillsParser(
            SkillDefinitions skillDefinitions,
            IParser<ActiveSkillParserParameter> activeSkillParser, IParser<SupportSkillParserParameter> supportSkillParser,
            AdditionalSkillLevelParserDelegate additionalSkillLevelParser)
        {
            _skillDefinitions = skillDefinitions;
            _supportabilityTester = new SupportabilityTester(skillDefinitions);
            _activeSkillParser = activeSkillParser;
            _supportSkillParser = supportSkillParser;
            _additionalSkillLevelParser = additionalSkillLevelParser;
        }

        public ParseResult Parse(SkillsParserParameter parameter)
        {
            var (skills, entity) = parameter;
            var (supportSkills, activeSkills) = skills.Partition(s => _skillDefinitions.GetSkillById(s.Id).IsSupport);
            return ParseResult.Aggregate(Parse(activeSkills.ToList(), supportSkills.ToList(), entity));
        }

        private IEnumerable<ParseResult> Parse(IEnumerable<Skill> activeSkills, IReadOnlyList<Skill> supportSkills, Entity entity)
        {
            foreach (var activeSkill in activeSkills)
            {
                var supportingSkills = _supportabilityTester.SelectSupportingSkills(activeSkill, supportSkills);
                var additionalSkillLevels = _additionalSkillLevelParser(activeSkill, supportSkills, entity);
                yield return _activeSkillParser.Parse(activeSkill, entity, additionalSkillLevels);
                foreach (var supportingSkill in supportingSkills)
                {
                    yield return _supportSkillParser.Parse(activeSkill, supportingSkill, entity, additionalSkillLevels);
                }
            }
        }
    }

    public static class SkillsParserExtensions
    {
        public static ParseResult Parse(this IParser<SkillsParserParameter> @this, IReadOnlyList<Skill> skills, Entity entity) =>
            @this.Parse(new SkillsParserParameter(skills, entity));
    }

    public class SkillsParserParameter : ValueObject
    {
        public SkillsParserParameter(IReadOnlyList<Skill> skills, Entity entity) =>
            (Skills, Entity) = (skills, entity);

        public void Deconstruct(out IReadOnlyList<Skill> skills, out Entity entity) =>
            (skills, entity) = (Skills, Entity);

        public IReadOnlyList<Skill> Skills { get; }
        public Entity Entity { get; }

        protected override object ToTuple() => (WithSequenceEquality(Skills), Entity);
    }
}