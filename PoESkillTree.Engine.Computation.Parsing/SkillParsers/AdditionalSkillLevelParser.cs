using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders.Skills;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;
#if NETSTANDARD2_0
using PoESkillTree.Engine.Utils.Extensions;
#endif

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class AdditionalSkillLevelParser : AdditionalSkillStatParser
    {
        private readonly IGemStatBuilders _gemStatBuilders;
        private readonly IGemTagBuilders _gemTagBuilders;
        private readonly IValueBuilders _valueBuilders;

        public AdditionalSkillLevelParser(
            SkillDefinitions skillDefinitions, IGemStatBuilders gemStatBuilders, IGemTagBuilders gemTagBuilders, IValueBuilders valueBuilders)
            : base(skillDefinitions)
        {
            _gemStatBuilders = gemStatBuilders;
            _gemTagBuilders = gemTagBuilders;
            _valueBuilders = valueBuilders;
        }

        public IReadOnlyDictionary<Skill, IValue> Parse(Skill activeSkill, IReadOnlyList<Skill> supportingSkills, Entity modifierSourceEntity)
        {
            var dict = supportingSkills.Select(skill => (skill, ParseSupport(skill))).ToDictionary();
            dict[activeSkill] = ParseActive(activeSkill, dict);
            return Build(dict, modifierSourceEntity);
        }

        private ValueBuilder ParseSupport(Skill supportingSkill)
        {
            var value = new ValueBuilder(_valueBuilders.Create(0));
            if (supportingSkill.Gem is null)
                return value;

            value += _gemStatBuilders.AdditionalLevelsForModifierSourceItemSlot().Value;
            
            var baseItem = GetBaseItem(supportingSkill);
            if (baseItem is null)
                return value;

            foreach (var gemTagBuilder in GetGemTagBuilders(baseItem))
            {
                value += _gemStatBuilders.AdditionalLevelsForModifierSourceItemSlot(gemTagBuilder).Value;
            }
            return value;
        }

        private ValueBuilder ParseActive(Skill activeSkill, IReadOnlyDictionary<Skill, ValueBuilder> supportingSkills)
        {
            var value = new ValueBuilder(_valueBuilders.Create(0));
            if (activeSkill.Gem is null)
                return value;

            value += _gemStatBuilders.AdditionalLevelsForModifierSourceItemSlot().Value
                     + _gemStatBuilders.AdditionalActiveLevelsForModifierSourceItemSlot().Value;

            var baseItem = GetBaseItem(activeSkill);
            if (baseItem is null)
                return value;

            value += GetAdditionalValueFromSupportingSkills(supportingSkills, baseItem);

            var isSpell = baseItem.GemTags.Contains("spell");
            foreach (var gemTagBuilder in GetGemTagBuilders(baseItem))
            {
                value += _gemStatBuilders.AdditionalActiveLevels(gemTagBuilder).Value
                         + _gemStatBuilders.AdditionalLevelsForModifierSourceItemSlot(gemTagBuilder).Value;
                if (isSpell)
                {
                    value += _gemStatBuilders.AdditionalActiveSpellLevels(gemTagBuilder).Value;
                }
            }
            return value;
        }

        private ValueBuilder GetAdditionalValueFromSupportingSkills(
            IReadOnlyDictionary<Skill, ValueBuilder> supportingSkills, SkillBaseItemDefinition baseItem)
        {
            var valueBuilder = new ValueBuilder(_valueBuilders.Create(0));
            foreach (var (supportingSkill, supportValueBuilder) in supportingSkills)
            {
                valueBuilder += supportValueBuilder.Select(d => SelectActiveAdditionalLevels(supportingSkill, (int) d),
                    v => $"SelectActiveAdditionalLevels({supportingSkill.Id}, {supportingSkill.Level}, {v})");
            }

            return valueBuilder;

            int SelectActiveAdditionalLevels(Skill supportingSkill, int supportAdditionalLevels)
            {
                var value = 0;
                foreach (var untranslatedStat in GetLevelStats(supportingSkill, supportAdditionalLevels))
                {
                    var match = SkillStatIds.SupportedSkillGemLevelRegex.Match(untranslatedStat.StatId);
                    var tag = match.Groups[1].Value;
                    if (tag == "active" || baseItem.GemTags.Contains(tag))
                    {
                        value += untranslatedStat.Value;
                    }
                }

                return value;
            }
        }

        private SkillBaseItemDefinition? GetBaseItem(Skill skill) =>
            GetSkillDefinition(skill.Gem!.SkillId).BaseItem;

        private IEnumerable<IGemTagBuilder> GetGemTagBuilders(SkillBaseItemDefinition baseItem) =>
            baseItem.GemTags.Select(_gemTagBuilders.From);
    }
}