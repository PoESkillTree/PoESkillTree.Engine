using PoESkillTree.Engine.Computation.Common.Builders.Values;

namespace PoESkillTree.Engine.Computation.Common.Builders.Entities
{
    public interface ICountableEntityBuilder : IEntityBuilder
    {
        ValueBuilder CountNearby { get; }
    }
}