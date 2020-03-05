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
    public class AdditionalSkillLevelParser
    {
        private readonly SkillDefinitions _skillDefinitions;
        private readonly IGemStatBuilders _gemStatBuilders;
        private readonly IGemTagBuilders _gemTagBuilders;
        private readonly IValueBuilders _valueBuilders;
        private readonly IValueCalculationContext _valueCalculationContext;

        public AdditionalSkillLevelParser(
            SkillDefinitions skillDefinitions, IGemStatBuilders gemStatBuilders, IGemTagBuilders gemTagBuilders, IValueBuilders valueBuilders,
            IValueCalculationContext valueCalculationContext)
        {
            _skillDefinitions = skillDefinitions;
            _gemStatBuilders = gemStatBuilders;
            _gemTagBuilders = gemTagBuilders;
            _valueBuilders = valueBuilders;
            _valueCalculationContext = valueCalculationContext;
        }

        public AdditionalSkillLevels Parse(Skill activeSkill, IReadOnlyList<Skill> supportingSkills, Entity modifierSourceEntity)
        {
            var dict = supportingSkills.Select(skill => (skill, ParseSupport(skill))).ToDictionary();
            dict[activeSkill] = ParseActive(activeSkill, dict);
            return AdditionalSkillLevels.Calculate(
                dict.ToDictionary(p => p.Key, p => (IValueBuilder) p.Value),
                _skillDefinitions, modifierSourceEntity, _valueCalculationContext);
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
                var supportDefinition = _skillDefinitions.GetSkillById(supportingSkill.Id);
                var level = supportingSkill.Level + supportAdditionalLevels;

                if (!supportDefinition.Levels.TryGetValue(level, out var levelDefinition))
                {
                    levelDefinition = supportDefinition.Levels
                        .Where(p => p.Key <= level)
                        .MaxBy(p => p.Key)
                        .FirstOrDefault()
                        .Value;
                    if (levelDefinition is null)
                        return 0;
                }

                var value = 0;
                foreach (var untranslatedStat in levelDefinition.Stats)
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
            _skillDefinitions.GetSkillById(skill.Gem!.SkillId).BaseItem;

        private IEnumerable<IGemTagBuilder> GetGemTagBuilders(SkillBaseItemDefinition baseItem) =>
            baseItem.GemTags.Select(_gemTagBuilders.From);
    }
}