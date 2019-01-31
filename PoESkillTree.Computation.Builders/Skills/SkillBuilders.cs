﻿using PoESkillTree.Computation.Builders.Stats;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Skills;
using PoESkillTree.Computation.Common.Parsing;
using PoESkillTree.GameModel.Skills;

namespace PoESkillTree.Computation.Builders.Skills
{
    public class SkillBuilders : ISkillBuilders
    {
        private readonly IStatFactory _statFactory;
        private readonly SkillDefinitions _skills;

        public SkillBuilders(IStatFactory statFactory, SkillDefinitions skills)
        {
            _statFactory = statFactory;
            _skills = skills;
        }

        public ISkillBuilderCollection AllSkills => CreateCollection();
        public ISkillBuilderCollection this[IKeywordBuilder keyword] => CreateCollection(keyword);

        private ISkillBuilderCollection CreateCollection(params IKeywordBuilder[] keywords)
            => new SkillBuilderCollection(_statFactory, keywords, _skills.Skills);

        public ISkillBuilder SummonSkeleton => FromId("SummonSkeletons");
        public ISkillBuilder VaalSummonSkeletons => FromId("VaalSummonSkeletons");
        public ISkillBuilder RaiseSpectre => FromId("RaiseSpectre");
        public ISkillBuilder RaiseZombie => FromId("RaiseZombie");
        public ISkillBuilder DetonateMines => FromId("GemDetonateMines");

        public ISkillBuilder FromId(string skillId)
            => new SkillBuilder(_statFactory, CoreBuilder.Create(_skills.GetSkillById(skillId)));

        public ISkillBuilder ModifierSourceSkill
            => new SkillBuilder(_statFactory, CoreBuilder.Create(BuildModifierSourceSkill));

        private SkillDefinition BuildModifierSourceSkill(BuildParameters parameters)
        {
            var modifierSource = parameters.ModifierSource;
            if (modifierSource is ModifierSource.Global global)
                modifierSource = global.LocalSource;

            switch (modifierSource)
            {
                case ModifierSource.Local.Skill skillSource:
                    return _skills.GetSkillById(skillSource.SkillId);
                case ModifierSource.Local.Gem gemSource:
                    return _skills.GetSkillById(gemSource.SkillId);
                default:
                    throw new ParseException($"ModifierSource must be a skill, {parameters.ModifierSource} given");
            }
        }
    }
}