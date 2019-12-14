namespace PoESkillTree.Engine.Computation.Common.Builders.Stats
{
    public interface ISkillEntityStatBuilders
    {
        /// <summary>
        /// Gets a stat representing the speed with which this entity is placed/thrown.
        /// </summary>
        IStatBuilder Speed { get; }

        /// <summary>
        /// The base time it takes to place/throw this entity.
        /// </summary>
        IStatBuilder BaseTime { get; }

        /// <summary>
        /// The duration this entity lasts in seconds.
        /// </summary>
        IStatBuilder Duration { get; }
    }

    public interface ITrapStatBuilders : ISkillEntityStatBuilders
    {
        IStatBuilder TriggerAoE { get; }
    }

    public interface IMineStatBuilders : ISkillEntityStatBuilders
    {
        IStatBuilder DetonationAoE { get; }
    }
}