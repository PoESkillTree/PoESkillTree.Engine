using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.Computation.Data.Collections;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Data.GivenStats
{
    public class EnemyGivenStats : LevelBasedStats
    {
        public EnemyGivenStats(
            IBuilderFactories builderFactories, IModifierBuilder modifierBuilder, MonsterBaseStats monsterBaseStats)
            : base(builderFactories, modifierBuilder, monsterBaseStats)
        {
        }

        public override IReadOnlyList<Entity> AffectedEntities { get; } = new[] { GameModel.Entity.Enemy };

        protected override GivenStatCollection CreateCollection()
            => new GivenStatCollection(ModifierBuilder, ValueFactory)
            {
                { PercentIncrease, Ground.Consecrated.AddStat(CriticalStrike.Chance.For(OpponentOfSelf)), 100 },
                // Level based
                { BaseSet, Stat.Level, ValueFactory.Minimum(Stat.Level.For(Entity.Character).Value, 84) },
                { BaseSet, Life, LevelBased(l => MonsterBaseStats.EnemyLife(l), "EnemyLife") },
                { BaseSet, Stat.Accuracy, LevelBased(l => MonsterBaseStats.Accuracy(l), "Accuracy") },
                { BaseSet, Stat.Evasion, LevelBased(l => MonsterBaseStats.Evasion(l), "Evasion") },
                {
                    BaseSet, Physical.Damage.WithSkills(DamageSource.Attack),
                    LevelBased(l => MonsterBaseStats.PhysicalDamage(l), "PhysicalDamage") * 1.5
                },
                { BaseSet, Stat.Armour, LevelBased(l => MonsterBaseStats.Armour(l), "Armour") },
                // buff configuration
                { TotalOverride, Buff.Maim.On(Self), 1, Condition.Unique("Maim.ExplicitlyActive") },
                { TotalOverride, Buff.Hinder.On(Self), 1, Condition.Unique("Hinder.ExplicitlyActive") },
                { TotalOverride, Buff.Blind.On(Self), 1, Condition.Unique("Blind.ExplicitlyActive") },
                { TotalOverride, Buff.Intimidate.On(Self), 1, Condition.Unique("Intimidate.ExplicitlyActive") },
                { TotalOverride, Buff.CoveredInAsh.On(Self), 1, Condition.Unique("CoveredInAsh.ExplicitlyActive") },
            };
    }
}