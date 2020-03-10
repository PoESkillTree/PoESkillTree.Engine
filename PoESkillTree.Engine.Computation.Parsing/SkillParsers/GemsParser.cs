using System.Collections.Generic;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class GemsParser : IParser<GemsParserParameter>
    {
        private readonly IParser<GemParserParameter> _gemParser;

        public GemsParser(IParser<GemParserParameter> gemParser)
        {
            _gemParser = gemParser;
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
            return ParseResult.Aggregate(results);
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