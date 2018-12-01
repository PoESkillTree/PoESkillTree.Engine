﻿using System.Collections.Generic;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Modifiers;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.GameModel;
using PoESkillTree.GameModel.Skills;

namespace PoESkillTree.Computation.Parsing.SkillParsers
{
    public class GemRequirementParser : IPartialSkillParser
    {
        private readonly IBuilderFactories _builderFactories;
        private readonly IModifierBuilder _modifierBuilder = new ModifierBuilder();

        public GemRequirementParser(IBuilderFactories builderFactories)
            => _builderFactories = builderFactories;

        public PartialSkillParseResult Parse(Skill mainSkill, Skill parsedSkill, SkillPreParseResult preParseResult)
        {
            if (!parsedSkill.GemGroup.HasValue)
                return new PartialSkillParseResult(new Modifier[0], new UntranslatedStat[0]);

            var modifiers = new List<Modifier>();
            var level = preParseResult.LevelDefinition;
            var requirementStats = _builderFactories.StatBuilders.Requirements;

            void AddModifier(IStatBuilder stat, Form form, double value)
            {
                var intermediateModifier = _modifierBuilder
                    .WithStat(stat)
                    .WithForm(_builderFactories.FormBuilders.From(form))
                    .WithValue(_builderFactories.ValueBuilders.Create(value)).Build();
                modifiers.AddRange(intermediateModifier.Build(preParseResult.GemSource, Entity.Character));
            }

            AddModifier(requirementStats.Level, Form.BaseSet, level.Requirements.Level);
            if (level.Requirements.Dexterity > 0)
            {
                AddModifier(requirementStats.Dexterity, Form.BaseSet, level.Requirements.Dexterity);
            }
            if (level.Requirements.Intelligence > 0)
            {
                AddModifier(requirementStats.Intelligence, Form.BaseSet, level.Requirements.Intelligence);
            }
            if (level.Requirements.Strength > 0)
            {
                AddModifier(requirementStats.Strength, Form.BaseSet, level.Requirements.Strength);
            }

            return new PartialSkillParseResult(modifiers, new UntranslatedStat[0]);
        }
    }
}