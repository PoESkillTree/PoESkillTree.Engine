using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public abstract class AdditionalSkillStatParserBase
    {
        private readonly SkillDefinitions _skillDefinitions;

        protected AdditionalSkillStatParserBase(SkillDefinitions skillDefinitions)
        {
            _skillDefinitions = skillDefinitions;
        }

        protected IReadOnlyDictionary<Skill, IValue> Build(IReadOnlyDictionary<Skill, ValueBuilder> valueBuilders, Entity modifierSourceEntity) =>
            valueBuilders.ToDictionary(p => p.Key, p => Build(p.Key, p.Value, modifierSourceEntity));

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