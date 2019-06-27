using Moq;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Values;

namespace PoESkillTree.Engine.Computation.Builders
{
    public static class BuildersHelper
    {
        public static ResolveContext MockResolveContext() =>
            new ResolveContext(Mock.Of<IMatchContext<IValueBuilder>>(), Mock.Of<IMatchContext<IReferenceConverter>>());
    }
}