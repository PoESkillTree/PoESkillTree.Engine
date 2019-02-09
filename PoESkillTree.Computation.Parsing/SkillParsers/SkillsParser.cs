﻿using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using PoESkillTree.GameModel.Skills;

namespace PoESkillTree.Computation.Parsing.SkillParsers
{
    /// <summary>
    /// Parses a group of skills at once. E.g. all equipped skills or all skills for one ItemSlot.
    /// </summary>
    public class SkillsParser : IParser<IReadOnlyCollection<Skill>>
    {
        private readonly SkillDefinitions _skillDefinitions;
        private readonly SupportabilityTester _supportabilityTester;
        private readonly IParser<Skill> _activeSkillParser;
        private readonly IParser<SupportSkillParserParameter> _supportSkillParser;

        public SkillsParser(
            SkillDefinitions skillDefinitions,
            IParser<Skill> activeSkillParser, IParser<SupportSkillParserParameter> supportSkillParser)
        {
            _skillDefinitions = skillDefinitions;
            _supportabilityTester = new SupportabilityTester(skillDefinitions);
            _activeSkillParser = activeSkillParser;
            _supportSkillParser = supportSkillParser;
        }

        public ParseResult Parse(IReadOnlyCollection<Skill> parameter)
        {
            var (supportSkills, activeSkills) = parameter
                .Partition(s => _skillDefinitions.GetSkillById(s.Id).IsSupport);
            supportSkills = supportSkills.ToList();

            var parseResults = new List<ParseResult>();
            foreach (var activeSkill in activeSkills)
            {
                parseResults.Add(_activeSkillParser.Parse(activeSkill));
                var supportingSkills = _supportabilityTester.SelectSupportingSkills(activeSkill, supportSkills);
                parseResults.AddRange(supportingSkills.Select(s => _supportSkillParser.Parse(activeSkill, s)));
            }

            return ParseResult.Aggregate(parseResults);
        }
    }
}