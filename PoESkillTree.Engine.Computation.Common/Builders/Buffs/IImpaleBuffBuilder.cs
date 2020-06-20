using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Common.Builders.Buffs
{
    public interface IImpaleBuffBuilder : IBuffBuilder
    {
        IStatBuilder Overwhelm { get; }
    }
}