using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;
#if NETSTANDARD2_0
using PoESkillTree.Engine.Utils.Extensions;
#endif

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class AdditionalSkillQualityParser : AdditionalSkillStatParser
    {
        private readonly IGemStatBuilders _gemStatBuilders;
        private readonly IValueBuilders _valueBuilders;

        public AdditionalSkillQualityParser(SkillDefinitions skillDefinitions, IGemStatBuilders gemStatBuilders, IValueBuilders valueBuilders)
            : base(skillDefinitions)
        {
            _gemStatBuilders = gemStatBuilders;
            _valueBuilders = valueBuilders;
        }

        public IReadOnlyDictionary<Skill, IValue> Parse(
            Skill activeSkill, IReadOnlyDictionary<Skill, int> supportingSkillsWithAdditionalLevels, Entity modifierSourceEntity)
        {
            var dict = supportingSkillsWithAdditionalLevels.Keys.Select(skill => (skill, ParseSupport())).ToDictionary();
            dict[activeSkill] = ParseActive(supportingSkillsWithAdditionalLevels);
            return Build(dict, modifierSourceEntity);
        }

        private ValueBuilder ParseSupport() =>
            new ValueBuilder(_valueBuilders.Create(0))
            + _gemStatBuilders.AdditionalQualityForModifierSourceItemSlot.Value
            + _gemStatBuilders.AdditionalSupportQualityForModifierSourceItemSlot.Value;

        private ValueBuilder ParseActive(IReadOnlyDictionary<Skill, int> supportingSkills) =>
            new ValueBuilder(_valueBuilders.Create(0))
            + _gemStatBuilders.AdditionalQualityForModifierSourceItemSlot.Value
            + GetAdditionalQualityFromSupportingSkills(supportingSkills);

        private ValueBuilder GetAdditionalQualityFromSupportingSkills(IReadOnlyDictionary<Skill, int> supportingSkills)
        {
            var valueBuilder = new ValueBuilder(_valueBuilders.Create(0));
            foreach (var (supportingSkill, additionalLevels) in supportingSkills)
            {
                foreach (var untranslatedStat in GetLevelStats(supportingSkill, additionalLevels))
                {
                    if (SkillStatIds.SupportedSkillGemQualityRegex.IsMatch(untranslatedStat.StatId))
                    {
                        valueBuilder += untranslatedStat.Value;
                    }
                }
            }

            return valueBuilder;
        }
    }
}