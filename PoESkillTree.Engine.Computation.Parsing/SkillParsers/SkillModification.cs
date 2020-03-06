using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class SkillModification : ValueObject
    {
        public SkillModification(int additionalLevels, int additionalQuality)
        {
            AdditionalLevels = additionalLevels;
            AdditionalQuality = additionalQuality;
        }

        public int AdditionalLevels { get; }
        public int AdditionalQuality { get; }

        protected override object ToTuple() => (AdditionalLevels, AdditionalQuality);
    }
}