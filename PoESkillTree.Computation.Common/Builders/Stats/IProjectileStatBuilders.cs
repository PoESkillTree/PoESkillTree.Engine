namespace PoESkillTree.Computation.Common.Builders.Stats
{
    /// <summary>
    /// Factory interface for stats related to projectiles.
    /// </summary>
    public interface IProjectileStatBuilders
    {
        /// <summary>
        /// Gets a stat representing the speed of projectiles.
        /// </summary>
        IStatBuilder Speed { get; }

        /// <summary>
        /// Gets a stat representing the amount of projectiles.
        /// </summary>
        IStatBuilder Count { get; }

        /// <summary>
        /// Gets a stat representing the amount targets pierced by projectiles.
        /// </summary>
        IStatBuilder PierceCount { get; }

        /// <summary>
        /// Gets a stat representing the number of times projectile chain.
        /// </summary>
        IStatBuilder ChainCount { get; }

        /// <summary>
        /// Gets a stat representing the distance traveled by projectiles.
        /// </summary>
        IStatBuilder TravelDistance { get; }
    }
}