using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    /// <summary>
    /// Partial parser of <see cref="SupportSkillParser"/> that parses level-dependent modifiers.
    /// </summary>
    public class SupportSkillLevelParser : IPartialSkillParser
    {
        private readonly IBuilderFactories _builderFactories;

        public SupportSkillLevelParser(IBuilderFactories builderFactories)
            => _builderFactories = builderFactories;

        private IMetaStatBuilders MetaStats => _builderFactories.MetaStatBuilders;

        public PartialSkillParseResult Parse(Skill mainSkill, Skill parsedSkill, SkillPreParseResult preParseResult)
        {
            var level = preParseResult.LevelDefinition;
            var modifiers = new ModifierCollection(_builderFactories, preParseResult.LocalSource, preParseResult.ModifierSourceEntity);

            if (level.ManaMultiplier is double multiplier)
            {
                var moreMultiplier = multiplier * 100 - 100;
                modifiers.AddGlobal(_builderFactories.SkillBuilders.FromId(mainSkill.Id).Cost,
                    Form.More, moreMultiplier, preParseResult.IsActiveSkill);
            }

            if (level.ManaCostOverride is int manaCostOverride)
            {
                modifiers.AddGlobal(MetaStats.SkillBaseCost(mainSkill), Form.TotalOverride, manaCostOverride);
            }

            if (level.Cooldown is int cooldown && preParseResult.MainSkillDefinition.Levels[mainSkill.Level].Cooldown is null)
            {
                modifiers.AddGlobal(_builderFactories.StatBuilders.Cooldown, Form.BaseSet, cooldown,
                    preParseResult.IsMainSkill.And(MetaStats.MainSkillHasKeyword(Keyword.Triggered).IsTrue));
            }

            return new PartialSkillParseResult(modifiers.Modifiers, new UntranslatedStat[0]);
        }
    }
}