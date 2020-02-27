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
            => Parse(parameter.ActiveSkill, parameter.ActiveSkill, parameter.Entity);

        public SkillPreParseResult ParseSupport(SupportSkillParserParameter parameter)
            => Parse(parameter.ActiveSkill, parameter.SupportSkill, parameter.Entity);

        private SkillPreParseResult Parse(Skill mainSkill, Skill parsedSkill, Entity entity)
        {
            var mainSkillDefinition = _skillDefinitions.GetSkillById(mainSkill.Id);
            var parsedSkillDefinition = _skillDefinitions.GetSkillById(parsedSkill.Id);
            var parsedSkillLevel = parsedSkillDefinition.Levels[parsedSkill.Level];

            var displayName = parsedSkillDefinition.IsSupport
                ? parsedSkillDefinition.BaseItem?.DisplayName
                : parsedSkillDefinition.ActiveSkill.DisplayName;
            var localSource = new ModifierSource.Local.Skill(mainSkill.Id, displayName);
            var globalSource = new ModifierSource.Global(localSource);
            var gemSource = new ModifierSource.Local.Gem(parsedSkill.ItemSlot, parsedSkill.SocketIndex, mainSkill.Id,
                displayName);

            var isMainSkill = _metaStatBuilders.IsMainSkill(mainSkill);
            var isActiveSkill = _metaStatBuilders.IsActiveSkill(mainSkill);

            return new SkillPreParseResult(parsedSkillDefinition, parsedSkillLevel, mainSkillDefinition,
                localSource, globalSource, gemSource, entity,
                isMainSkill, isActiveSkill);
        }
    }
}