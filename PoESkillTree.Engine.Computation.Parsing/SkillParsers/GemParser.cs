using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class GemParser : IParser<GemParserParameter>
    {
        private readonly SkillDefinitions _skillDefinitions;
        private readonly IBuilderFactories _builderFactories;

        public GemParser(SkillDefinitions skillDefinitions, IBuilderFactories builderFactories)
        {
            _skillDefinitions = skillDefinitions;
            _builderFactories = builderFactories;
        }

        public ParseResult Parse(GemParserParameter parameter)
        {
            var (gem, modifierSourceEntity) = parameter;
            var skillDefinition = _skillDefinitions.GetSkillById(gem.SkillId);
            parameter.Skills.AddRange(ParseSkills(gem, skillDefinition));
            return ParseRequirements(gem, modifierSourceEntity, skillDefinition);
        }

        private static IEnumerable<Skill> ParseSkills(Gem gem, SkillDefinition skillDefinition)
        {
            yield return Skill.FromGem(gem, true);
            if (skillDefinition.SecondarySkillId is string secondarySkillId)
            {
                var isEnabled = !skillDefinition.BaseItem?.GemTags.Contains("vaal") ?? true;
                yield return Skill.SecondaryFromGem(secondarySkillId, gem, isEnabled);
            }
        }

        private ParseResult ParseRequirements(Gem gem, Entity modifierSourceEntity, SkillDefinition skillDefinition)
        {
            var levelDefinition = skillDefinition.Levels[gem.Level];
            var modifierSource = new ModifierSource.Local.Gem(gem, skillDefinition.DisplayName);
            var modifiers = new ModifierCollection(_builderFactories, modifierSource, modifierSourceEntity);
            var requirementStats = _builderFactories.StatBuilders.Requirements;

            modifiers.AddLocal(requirementStats.Level, Form.BaseSet, levelDefinition.Requirements.Level);
            if (levelDefinition.Requirements.Dexterity > 0)
            {
                modifiers.AddLocal(requirementStats.Dexterity, Form.BaseSet, levelDefinition.Requirements.Dexterity);
            }
            if (levelDefinition.Requirements.Intelligence > 0)
            {
                modifiers.AddLocal(requirementStats.Intelligence, Form.BaseSet, levelDefinition.Requirements.Intelligence);
            }
            if (levelDefinition.Requirements.Strength > 0)
            {
                modifiers.AddLocal(requirementStats.Strength, Form.BaseSet, levelDefinition.Requirements.Strength);
            }

            return ParseResult.Success(modifiers.Modifiers);
        }
    }

    public static class GemParserExtensions
    {
        public static ParseResult Parse(this IParser<GemParserParameter> @this, Gem gem, Entity entity, out IReadOnlyList<Skill> skills)
        {
            var parameter = new GemParserParameter(gem, entity);
            var result = @this.Parse(parameter);
            skills = parameter.Skills;
            return result;
        }
    }

    public class GemParserParameter
    {
        public GemParserParameter(Gem gem, Entity entity)
        {
            Gem = gem;
            Entity = entity;
            Skills = new List<Skill>();
        }

        public void Deconstruct(out Gem gem, out Entity entity) =>
            (gem, entity) = (Gem, Entity);

        public Gem Gem { get; }
        public Entity Entity { get; }

        public List<Skill> Skills { get; }
    }
}