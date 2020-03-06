using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class SkillModification : ValueObject
    {
        public SkillModification(int additionalLevels)
        {
            AdditionalLevels = additionalLevels;
        }

        public int AdditionalLevels { get; }

        protected override object ToTuple() => AdditionalLevels;
    }
}