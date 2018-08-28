namespace PoESkillTree.Computation.Common.Builders.Stats
{
    /// <summary>
    /// Represent the regeneration stat of a pool. Its value is the amount of a pool regenerated per second.
    /// </summary>
    public interface IRegenStatBuilder : IStatBuilder
    {
        /// <summary>
        /// Gets a stat representing the percentage of the pool's value that is regenerated per second. The returned
        /// stat's value (as percentage of the pool's value) will be added to the regen stat's value.
        /// </summary>
        IStatBuilder Percent { get; }

        /// <summary>
        /// A stat specifying the Pool this regeneration applies to.
        /// </summary>
        IStatBuilder TargetPool { get; }
    }
}