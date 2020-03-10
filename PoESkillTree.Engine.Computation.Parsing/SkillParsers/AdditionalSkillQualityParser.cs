using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class AdditionalSkillQualityParser : AdditionalSkillStatParserBase
    {
        private readonly IGemStatBuilders _gemStatBuilders;
        private readonly IValueBuilders _valueBuilders;

        public AdditionalSkillQualityParser(SkillDefinitions skillDefinitions, IGemStatBuilders gemStatBuilders, IValueBuilders valueBuilders)
            : base(skillDefinitions)
        {
            _gemStatBuilders = gemStatBuilders;
            _valueBuilders = valueBuilders;
        }

        protected override IReadOnlyDictionary<Skill, ValueBuilder> Parse(Skill activeSkill, IReadOnlyList<Skill> supportingSkills)
        {
            var dict = supportingSkills.Select(skill => (skill, ParseSupport())).ToDictionary();
            dict[activeSkill] = ParseActive(supportingSkills);
            return dict;
        }

        protected override IStatBuilder GetAdditionalStatBuilder(Skill skill) =>
            _gemStatBuilders.AdditionalQuality(skill);

        private ValueBuilder ParseSupport() =>
            new ValueBuilder(_valueBuilders.Create(0))
            + _gemStatBuilders.AdditionalQualityForModifierSourceItemSlot.Value
            + _gemStatBuilders.AdditionalSupportQualityForModifierSourceItemSlot.Value;

        private ValueBuilder ParseActive(IReadOnlyList<Skill> supportingSkills) =>
            new ValueBuilder(_valueBuilders.Create(0))
            + _gemStatBuilders.AdditionalQualityForModifierSourceItemSlot.Value
            + GetAdditionalQualityFromSupportingSkills(supportingSkills);

        private ValueBuilder GetAdditionalQualityFromSupportingSkills(IReadOnlyList<Skill> supportingSkills)
        {
            var valueBuilder = new ValueBuilder(_valueBuilders.Create(0));
            foreach (var supportingSkill in supportingSkills)
            {
                var supportValueBuilder = _gemStatBuilders.AdditionalLevels(supportingSkill).Value;
                valueBuilder += supportValueBuilder.Select(d => SelectActiveAdditionalQuality(supportingSkill, (int) d),
                    v => $"SelectActiveAdditionalQuality({supportingSkill.Id}, {supportingSkill.Level}, {v})");
            }

            return valueBuilder;

            int SelectActiveAdditionalQuality(Skill supportingSkill, int supportAdditionalLevels)
            {
                var value = 0;
                foreach (var untranslatedStat in GetLevelStats(supportingSkill, supportAdditionalLevels))
                {
                    if (SkillStatIds.SupportedSkillGemQualityRegex.IsMatch(untranslatedStat.StatId))
                    {
                        value += untranslatedStat.Value;
                    }
                }

                return value;
            }
        }
    }
}