using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class SkillPreParseResult
    {
        public SkillPreParseResult(
            SkillDefinition skillDefinition, SkillLevelDefinition levelDefinition, SkillDefinition mainSkillDefinition,
            ModifierSource.Local.Skill localSource, ModifierSource.Global globalSource, Entity modifierSourceEntity,
            IConditionBuilder isMainSkill, IConditionBuilder isActiveSkill)
        {
            SkillDefinition = skillDefinition;
            LevelDefinition = levelDefinition;
            MainSkillDefinition = mainSkillDefinition;
            LocalSource = localSource;
            GlobalSource = globalSource;
            ModifierSourceEntity = modifierSourceEntity;
            IsMainSkill = isMainSkill;
            IsActiveSkill = isActiveSkill;
        }

        /// <summary>
        /// The definition of the parsed skill.
        /// </summary>
        public SkillDefinition SkillDefinition { get; }

        /// <summary>
        /// The relevant level definition of <see cref="SkillDefinition"/>.
        /// </summary>
        public SkillLevelDefinition LevelDefinition { get; }

        /// <summary>
        /// Same as <see cref="SkillDefinition"/> when parsing active skills. The active skill when parsing support
        /// skills.
        /// </summary>
        public SkillDefinition MainSkillDefinition { get; }

        public ModifierSource.Local.Skill LocalSource { get; }
        public ModifierSource.Global GlobalSource { get; }

        public Entity ModifierSourceEntity { get; }

        public IConditionBuilder IsMainSkill { get; }
        public IConditionBuilder IsActiveSkill { get; }
    }
}