using System;
using PoESkillTree.Computation.Parsing.Builders.Conditions;
using PoESkillTree.Computation.Parsing.Builders.Matching;
using PoESkillTree.Computation.Parsing.Builders.Values;

namespace PoESkillTree.Computation.Parsing.Builders
{
    public interface IBuilderCollection<out T> : IResolvable<IBuilderCollection<T>>
    {
        ValueBuilder Count(Func<T, IConditionBuilder> predicate = null);

        IConditionBuilder Any(Func<T, IConditionBuilder> predicate = null);
    }
}