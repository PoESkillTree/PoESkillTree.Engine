using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class GemsParser : IParser<GemsParserParameter>
    {
        private readonly SkillDefinitions _skillDefinitions;
        private readonly SupportabilityTester _supportabilityTester;
        private readonly IParser<GemParserParameter> _gemParser;
        private readonly IParser<AdditionalSkillStatParserParameter> _additionalSkillStatParser;

        public GemsParser(
            SkillDefinitions skillDefinitions, IParser<GemParserParameter> gemParser,
            IParser<AdditionalSkillStatParserParameter> additionalSkillStatParser)
        {
            _skillDefinitions = skillDefinitions;
            _supportabilityTester = new SupportabilityTester(skillDefinitions);
            _gemParser = gemParser;
            _additionalSkillStatParser = additionalSkillStatParser;
        }

        public ParseResult Parse(GemsParserParameter parameter)
        {
            var (gems, entity) = parameter;
            var results = new List<ParseResult>(gems.Count);
            foreach (var gem in gems)
            {
                results.Add(_gemParser.Parse(gem, entity, out var newSkills));
                parameter.Skills.AddRange(newSkills);
            }
            results.AddRange(ParseAdditionalSkillStats(parameter.Skills, entity));
            return ParseResult.Aggregate(results);
        }

        private IEnumerable<ParseResult> ParseAdditionalSkillStats(IReadOnlyList<Skill> skills, Entity entity)
        {
            var (supportSkills, activeSkills) = skills.Partition(s => _skillDefinitions.GetSkillById(s.Id).IsSupport);
            supportSkills = supportSkills.ToList();
            foreach (var activeSkill in activeSkills)
            {
                var supportingSkills = _supportabilityTester.SelectSupportingSkills(activeSkill, supportSkills).ToList();
                yield return _additionalSkillStatParser.Parse(activeSkill, supportingSkills, entity);
            }
        }
    }

    public static class GemsParserExtensions
    {
        public static ParseResult Parse(this IParser<GemsParserParameter> @this, IReadOnlyList<Gem> gems, Entity entity, out IReadOnlyList<Skill> skills)
        {
            var parameter = new GemsParserParameter(gems, entity);
            var result = @this.Parse(parameter);
            skills = parameter.Skills;
            return result;
        }
    }

    public class GemsParserParameter
    {
        public GemsParserParameter(IReadOnlyList<Gem> gems, Entity entity)
        {
            Gems = gems;
            Entity = entity;
            Skills = new List<Skill>();
        }

        public void Deconstruct(out IReadOnlyList<Gem> gems, out Entity entity) =>
            (gems, entity) = (Gems, Entity);

        public IReadOnlyList<Gem> Gems { get; }
        public Entity Entity { get; }

        public List<Skill> Skills { get; }
    }
}