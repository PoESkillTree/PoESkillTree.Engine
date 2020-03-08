using System.Linq;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    /// <summary>
    /// Collects properties for <see cref="ActiveSkillParser"/> and <see cref="SupportSkillParser"/> that are used
    /// in different partial parsers.
    /// </summary>
    public class SkillPreParser
    {
        private readonly SkillDefinitions _skillDefinitions;
        private readonly IMetaStatBuilders _metaStatBuilders;

        public SkillPreParser(SkillDefinitions skillDefinitions, IMetaStatBuilders metaStatBuilders)
            => (_skillDefinitions, _metaStatBuilders) = (skillDefinitions, metaStatBuilders);

        public SkillPreParseResult ParseActive(ActiveSkillParserParameter parameter)
            => Parse(parameter.ActiveSkill, parameter.ActiveSkill, parameter.Entity, parameter.Modification);

        public SkillPreParseResult ParseSupport(SupportSkillParserParameter parameter)
            => Parse(parameter.ActiveSkill, parameter.SupportSkill, parameter.Entity, parameter.SupportModification);

        private SkillPreParseResult Parse(Skill mainSkill, Skill parsedSkill, Entity entity, SkillModification parsedSkillModification)
        {
            var mainSkillDefinition = _skillDefinitions.GetSkillById(mainSkill.Id);
            var parsedSkillDefinition = _skillDefinitions.GetSkillById(parsedSkill.Id);

            var actualLevel = parsedSkill.Level + parsedSkillModification.AdditionalLevels;
            if (!parsedSkillDefinition.Levels.TryGetValue(actualLevel, out var parsedSkillLevel))
            {
                parsedSkillLevel = parsedSkillDefinition.Levels
                    .OrderBy(p => p.Key)
                    .Last(p => p.Key <= actualLevel)
                    .Value;
            }

            var displayName = parsedSkillDefinition.DisplayName;
            var localSource = new ModifierSource.Local.Skill(mainSkill.Id, displayName);
            var globalSource = new ModifierSource.Global(localSource);

            var isMainSkill = _metaStatBuilders.IsMainSkill(mainSkill);
            var isActiveSkill = _metaStatBuilders.IsActiveSkill(mainSkill);

            return new SkillPreParseResult(parsedSkillDefinition, parsedSkillLevel, mainSkillDefinition,
                localSource, globalSource, entity,
                isMainSkill, isActiveSkill);
        }
    }
}