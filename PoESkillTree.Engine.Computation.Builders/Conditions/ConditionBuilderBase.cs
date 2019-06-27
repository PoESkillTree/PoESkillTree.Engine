using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;

namespace PoESkillTree.Engine.Computation.Builders.Conditions
{
    public abstract class ConditionBuilderBase : IConditionBuilder
    {
        public abstract IConditionBuilder Resolve(ResolveContext context);

        public IConditionBuilder And(IConditionBuilder condition) =>
            new AndCompositeConditionBuilder(this, condition);

        public IConditionBuilder Or(IConditionBuilder condition) =>
            new OrCompositeConditionBuilder(this, condition);

        public abstract IConditionBuilder Not { get; }

        public abstract ConditionBuilderResult Build(BuildParameters parameters);
    }
}