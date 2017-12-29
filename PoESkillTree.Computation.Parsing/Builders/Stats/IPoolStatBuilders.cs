namespace PoESkillTree.Computation.Parsing.Builders.Stats
{
    /// <summary>
    /// Factory interface for pool stats.
    /// </summary>
    public interface IPoolStatBuilders
    {
        IPoolStatBuilder Life { get; }
        IPoolStatBuilder Mana { get; }
        IPoolStatBuilder EnergyShield { get; }
    }
}