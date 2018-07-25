﻿using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Data;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;

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
        private readonly Lazy<IReadOnlyList<GivenStatData>> _lazyGivenStats;

        public MonsterGivenStats(IBuilderFactories builderFactories) : base(builderFactories)
        {
            _lazyGivenStats = new Lazy<IReadOnlyList<GivenStatData>>(() => CreateCollection().ToList());
        }

        public IReadOnlyList<Entity> AffectedEntities { get; } =
            new[] { Common.Entity.Enemy, Common.Entity.Totem, Common.Entity.Minion };

        public IReadOnlyList<string> GivenStatLines { get; } = new[]
        {
            "15% additional Physical Damage Reduction per Endurance Charge",
            "+15% to all Elemental Resistances per Endurance Charge",
            "15% increased Attack and Cast Speed per Frenzy Charge",
            "5% increased Movement Speed per Frenzy Charge",
            "4% more Damage per Frenzy Charge",
            "200% increased Critical Strike Chance per Power Charge",
        };

        public IReadOnlyList<GivenStatData> GivenStats => _lazyGivenStats.Value;

        private GivenStatCollection CreateCollection() => new GivenStatCollection
        {
            // base stats
            { BaseSet, Mana, 200 },
            { BaseSet, Mana.Regen.Percent, 100 / 60.0 },
            // minima and maxima
            { BaseSet, Physical.Resistance.Maximum, 75 },
            // - traps, mines and totems
            { BaseSet, Traps.CombinedInstances.Maximum, 3 },
            { BaseSet, Mines.CombinedInstances.Maximum, 5 },
            { BaseSet, Totems.CombinedInstances.Maximum, 1 },
            // - movement speed
            { BaseSet, Stat.MovementSpeed.Maximum, 128 },
        };
    }
}