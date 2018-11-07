﻿using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Modifiers;
using PoESkillTree.Computation.Common.Data;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;
using PoESkillTree.GameModel;

namespace PoESkillTree.Computation.Data.GivenStats
{
    /// <summary>
    /// Given stats of all monsters, including enemies and the character's totems and minions
    /// </summary>
    /// <remarks>
    /// See https://pathofexile.gamepedia.com/Monster and Metadata/Monsters/Monster.ot in GGPK.
    /// </remarks>
    public class MonsterGivenStats : UsesStatBuilders, IGivenStats
    {
        private readonly IModifierBuilder _modifierBuilder;
        private readonly Lazy<IReadOnlyList<IIntermediateModifier>> _lazyGivenStats;

        public MonsterGivenStats(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories)
        {
            _modifierBuilder = modifierBuilder;
            _lazyGivenStats = new Lazy<IReadOnlyList<IIntermediateModifier>>(() => CreateCollection().ToList());
        }

        public IReadOnlyList<Entity> AffectedEntities { get; } =
            new[] { GameModel.Entity.Enemy, GameModel.Entity.Totem, GameModel.Entity.Minion };

        public IReadOnlyList<string> GivenStatLines { get; } = new[]
        {
            "15% additional Physical Damage Reduction per Endurance Charge",
            "+15% to all Elemental Resistances per Endurance Charge",
            "15% increased Attack and Cast Speed per Frenzy Charge",
            "5% increased Movement Speed per Frenzy Charge",
            "4% more Damage per Frenzy Charge",
            "200% increased Critical Strike Chance per Power Charge",
        };

        public IReadOnlyList<IIntermediateModifier> GivenModifiers => _lazyGivenStats.Value;

        private GivenStatCollection CreateCollection() => new GivenStatCollection(_modifierBuilder, ValueFactory)
        {
            // pools
            { BaseSet, Mana, 200 },
            { BaseSet, Mana.Regen.Percent, 100 / 60.0 },
            // other basic stats
            { BaseSet, CriticalStrike.Multiplier, 130 },
            // resistances
            { BaseSet, Physical.Resistance.Maximum, 75 },
            // traps, mines and totems
            { BaseSet, Traps.CombinedInstances.Maximum, 3 },
            { BaseSet, Mines.CombinedInstances.Maximum, 5 },
            { BaseSet, Totems.CombinedInstances.Maximum, 1 },
        };
    }
}