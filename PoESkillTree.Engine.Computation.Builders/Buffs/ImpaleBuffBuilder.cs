using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Buffs;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Builders.Buffs
{
    internal class ImpaleBuffBuilder : BuffBuilder, IImpaleBuffBuilder
    {
        public ImpaleBuffBuilder(IStatFactory statFactory) : base(statFactory, CoreBuilder.Create("Impale"))
        {
        }

        public IStatBuilder Overwhelm => FromIdentity(nameof(Overwhelm), typeof(int));
    }
}