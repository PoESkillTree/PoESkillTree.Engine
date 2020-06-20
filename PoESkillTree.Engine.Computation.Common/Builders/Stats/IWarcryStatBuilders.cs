using PoESkillTree.Engine.Computation.Common.Builders.Values;

namespace PoESkillTree.Engine.Computation.Common.Builders.Stats
{
    public interface IWarcryStatBuilders
    {
        public IStatBuilder PowerMultiplier { get; }
        public IStatBuilder MinimumPower { get; }

        public IStatBuilder ExertedAttacks { get; }

        public ValueBuilder AllyPower { get; }
        public ValueBuilder EnemyPower { get; }
        public ValueBuilder CorpsePower { get; }
        public ValueBuilder LastPower { get; }
    }
}