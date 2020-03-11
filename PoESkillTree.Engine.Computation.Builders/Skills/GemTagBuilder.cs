using PoESkillTree.Engine.Computation.Common.Builders.Skills;

namespace PoESkillTree.Engine.Computation.Builders.Skills
{
    public class GemTagBuilders : IGemTagBuilders
    {
        public IGemTagBuilder From(string internalId)
        {
            return new GemTagBuilder(internalId);
        }
    }

    public class GemTagBuilder : ConstantBuilder<IGemTagBuilder, string>, IGemTagBuilder
    {
        public GemTagBuilder(string gemTag) : base(gemTag)
        {
        }
    }
}