using System.Collections.Generic;

namespace PoESkillTree.Engine.GameModel.Skills
{
    public class SkillDefinitions : DefinitionsBase<string, SkillDefinition>
    {
        public SkillDefinitions(IReadOnlyList<SkillDefinition> skills) : base(skills)
        {
        }

        public IReadOnlyList<SkillDefinition> Skills => Definitions;

        public SkillDefinition GetSkillById(string skillId) => GetDefinitionById(skillId);
    }
}