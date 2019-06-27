using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Builders.Stats
{
    public interface ICoreStatBuilder : IResolvable<ICoreStatBuilder>
    {
        ICoreStatBuilder WithEntity(IEntityBuilder entityBuilder);

        IEnumerable<StatBuilderResult> Build(BuildParameters parameters);
    }
}