using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class AdditionalSkillLevelMaximumParser : AdditionalSkillStatParserBase
    {
        private readonly IGemStatBuilders _gemStatBuilders;
        private readonly IValueBuilders _valueBuilders;

        public AdditionalSkillLevelMaximumParser(SkillDefinitions skillDefinitions, IGemStatBuilders gemStatBuilders, IValueBuilders valueBuilders)
            : base(skillDefinitions)
        {
            _gemStatBuilders = gemStatBuilders;
            _valueBuilders = valueBuilders;
        }

        protected override IReadOnlyDictionary<Skill, ValueBuilder> Parse(Skill activeSkill, IReadOnlyList<Skill> supportingSkills) =>
            supportingSkills.Append(activeSkill).ToDictionary(s => s, Parse);

        private ValueBuilder Parse(Skill skill)
        {
            var max = GetSkillDefinition(skill.Id).Levels.Keys.Max();
            return new ValueBuilder(_valueBuilders.Create(max));
        }

        protected override IStatBuilder GetAdditionalStatBuilder(Skill skill) =>
            _gemStatBuilders.AdditionalLevels(skill).Maximum;
    }
}