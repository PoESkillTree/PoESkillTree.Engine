using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Values;

namespace PoESkillTree.Engine.Computation.Builders
{
    internal static class BuilderExtensions
    {
        public static IValue Build(this IValueBuilder @this) => @this.Build(default);

        public static ConditionBuilderResult Build(this IConditionBuilder @this) =>
            @this.Build(default);
    }
}