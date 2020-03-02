using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    /// <summary>
    /// Parser for active skills
    /// </summary>
    public class ActiveSkillParser : IParser<ActiveSkillParserParameter>
    {
        private readonly SkillDefinitions _skillDefinitions;
        private readonly IBuilderFactories _builderFactories;
        private readonly UntranslatedStatParserFactory _statParserFactory;

        public ActiveSkillParser(
            SkillDefinitions skillDefinitions, IBuilderFactories builderFactories,
            UntranslatedStatParserFactory statParserFactory)
            => (_skillDefinitions, _builderFactories, _statParserFactory) =
                (skillDefinitions, builderFactories, statParserFactory);

        public ParseResult Parse(ActiveSkillParserParameter parameter)
        {
            var (skill, _) = parameter;

            if (!skill.IsEnabled)
                return ParseResult.Empty;

            var modifiers = new List<Modifier>();
            var parsedStats = new List<UntranslatedStat>();

            var preParser = new SkillPreParser(_skillDefinitions, _builderFactories.MetaStatBuilders);
            var preParseResult = preParser.ParseActive(parameter);

            foreach (var partialParser in CreatePartialParsers())
            {
                var (newlyParsedModifiers, newlyParsedStats) = partialParser.Parse(skill, skill, preParseResult);
                modifiers.AddRange(newlyParsedModifiers);
                parsedStats.AddRange(newlyParsedStats);
            }

            var translatingParser = new TranslatingSkillParser(_builderFactories, _statParserFactory);
            return translatingParser.Parse(skill, preParseResult, new PartialSkillParseResult(modifiers, parsedStats));
        }

        private IPartialSkillParser[] CreatePartialParsers()
            => new[]
            {
                new ActiveSkillGeneralParser(_builderFactories),
                SkillKeywordParser.CreateActive(_builderFactories),
                SkillTypeParser.CreateActive(_builderFactories),
                new ActiveSkillLevelParser(_builderFactories),
                new SkillStatParser(_builderFactories),
            };
    }

    public static class ActiveSkillParserExtensions
    {
        public static ParseResult Parse(this IParser<ActiveSkillParserParameter> @this, Skill activeSkill, Entity entity) =>
            @this.Parse(new ActiveSkillParserParameter(activeSkill, entity));
    }

    public class ActiveSkillParserParameter : ValueObject
    {
        public ActiveSkillParserParameter(Skill activeSkill, Entity entity)
            => (ActiveSkill, Entity) = (activeSkill, entity);

        public void Deconstruct(out Skill activeSkill, out Entity entity)
            => (activeSkill, entity) = (ActiveSkill, Entity);

        public Skill ActiveSkill { get; }
        public Entity Entity { get; }

        protected override object ToTuple() => (ActiveSkill, Entity);
    }
}