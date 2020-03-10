using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public abstract class AdditionalSkillStatParserBase : IParser<AdditionalSkillStatParserParameter>
    {
        private readonly SkillDefinitions _skillDefinitions;

        protected AdditionalSkillStatParserBase(SkillDefinitions skillDefinitions)
        {
            _skillDefinitions = skillDefinitions;
        }

        public ParseResult Parse(AdditionalSkillStatParserParameter parameter)
        {
            var (activeSkill, supportingSkills, entity) = parameter;
            var modifiers = Parse(activeSkill, supportingSkills)
                .Select(p => CreateAdditionalStatModifier(p.Key, p.Value, entity))
                .ToList();
            return ParseResult.Success(modifiers);
        }

        protected abstract IReadOnlyDictionary<Skill, ValueBuilder> Parse(Skill activeSkill, IReadOnlyList<Skill> supportingSkills);

        protected Modifier CreateAdditionalStatModifier(Skill skill, IValueBuilder valueBuilder, Entity modifierSourceEntity)
        {
            var stats = GetAdditionalStatBuilder(skill).BuildToStats(modifierSourceEntity).ToList();
            var value = Build(skill, valueBuilder, modifierSourceEntity);
            var modifierSource = new ModifierSource.Global(new ModifierSource.Local.Given());
            return new Modifier(stats, Form.TotalOverride, value, modifierSource);
        }

        protected abstract IStatBuilder GetAdditionalStatBuilder(Skill skill);

        private IValue Build(Skill skill, IValueBuilder valueBuilder, Entity modifierSourceEntity)
        {
            var gem = skill.Gem;
            if (gem is null)
                return new Constant(0);

            var displayName = GetSkillDefinition(gem.SkillId).DisplayName;
            var gemModifierSource = new ModifierSource.Local.Gem(gem, displayName);
            var buildParameters = new BuildParameters(new ModifierSource.Global(gemModifierSource), modifierSourceEntity, default);
            return valueBuilder.Build(buildParameters);
        }

        protected IReadOnlyList<UntranslatedStat> GetLevelStats(Skill skill, int additionalLevels)
        {
            var definition = GetSkillDefinition(skill.Id);
            var level = skill.Level + additionalLevels;

            if (definition.Levels.TryGetValue(level, out var levelDefinition))
            {
                return levelDefinition.Stats;
            }
            else
            {
                return definition.Levels
                    .OrderBy(p => p.Key)
                    .LastOrDefault(p => p.Key <= level)
                    .Value?.Stats ?? Array.Empty<UntranslatedStat>();
            }
        }

        protected SkillDefinition GetSkillDefinition(string skillId) => _skillDefinitions.GetSkillById(skillId);
    }
}