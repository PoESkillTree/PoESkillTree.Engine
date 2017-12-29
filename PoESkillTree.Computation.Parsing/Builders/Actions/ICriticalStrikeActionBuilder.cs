﻿using PoESkillTree.Computation.Parsing.Builders.Stats;

namespace PoESkillTree.Computation.Parsing.Builders.Actions
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an action that occurs when Self critically hits from any entity and contains stats related to 
    /// critical strikes.
    /// </summary>
    public interface ICriticalStrikeActionBuilder : IActionBuilder
    {
        /// <summary>
        /// Gets a stat representing the critical strike chance.
        /// </summary>
        IStatBuilder Chance { get; }
        
        /// <summary>
        /// Gets a stat representing the critical strike multiplier.
        /// </summary>
        IStatBuilder Multiplier { get; }
        
        /// <summary>
        /// Gets a stat representing the multiplier for ailments from critical strikes.
        /// </summary>
        IStatBuilder AilmentMultiplier { get; }
        
        /// <summary>
        /// Gets a stat representing the extra damage taken from critical strikes.
        /// </summary>
        IStatBuilder ExtraDamageTaken { get; }
    }
}