﻿using PoESkillTree.Computation.Common.Builders.Values;

namespace PoESkillTree.Computation.Common.Builders.Stats
{
    /// <summary>
    /// Factory interface for stats and values related to the passive skill tree
    /// </summary>
    public interface IPassiveTreeBuilders
    {
        IStatBuilder NodeSkilled(ushort nodeId);

        ValueBuilder TotalInModifierSourceJewelRadius(IStatBuilder stat);
    }
}