﻿using PoESkillTree.Computation.Common.Builders.Damage;
using PoESkillTree.Computation.Common.Builders.Effects;
using PoESkillTree.Computation.Common.Builders.Values;

namespace PoESkillTree.Computation.Common.Builders.Stats
{
    /// <summary>
    /// Contains stat builders that do not partake in parsing but are relevant for calculations.
    /// </summary>
    /// <remarks>
    /// "EffectiveChance" stats are probabilities between 0 and 1. "Chance" stats are percentages between 0 and 100,
    /// same as for non-meta stats.
    /// </remarks>
    public interface IMetaStatBuilders
    {
        IStatBuilder EffectiveRegen(Pool pool);
        IStatBuilder EffectiveRecharge(Pool pool);
        IStatBuilder RechargeStartDelay(Pool pool);
        IStatBuilder EffectiveLeechRate(Pool pool);
        IStatBuilder AbsoluteLeechRate(Pool pool);
        IStatBuilder AbsoluteLeechRateLimit(Pool pool);
        IStatBuilder TimeToReachLeechRateLimit(Pool pool);

        /// <summary>
        /// The value of the pool that is the target pool of <see cref="sourcePool"/>'s regen.
        /// </summary>
        ValueBuilder RegenTargetPoolValue(Pool sourcePool);

        ValueBuilder LeechTargetPoolValue(Pool sourcePool);

        // Damage calculation
        IDamageRelatedStatBuilder EnemyResistanceAgainstNonCrits(DamageType damageType);  // with hits
        IDamageRelatedStatBuilder EnemyResistanceAgainstCrits(DamageType damageType);  // with hits
        IDamageRelatedStatBuilder EffectiveDamageMultiplierWithNonCrits(DamageType damageType);
        IDamageRelatedStatBuilder EffectiveDamageMultiplierWithCrits(DamageType damageType);  // with hits and ailments
        IDamageRelatedStatBuilder DamageWithNonCrits(DamageType damageType);
        IDamageRelatedStatBuilder DamageWithCrits(DamageType damageType);  // with hits and ailments
        IDamageRelatedStatBuilder DamageWithNonCrits();
        IDamageRelatedStatBuilder DamageWithCrits();  // with hits and ailments
        IDamageRelatedStatBuilder AverageDamagePerHit { get; }  // with hits
        IDamageRelatedStatBuilder AverageDamage { get; }
        IStatBuilder AverageHitDamage { get; }
        IStatBuilder SkillDpsWithHits { get; }
        IStatBuilder SkillDpsWithDoTs { get; }
        IStatBuilder AverageIgniteDamage { get; }
        IStatBuilder IgniteDps { get; }
        IStatBuilder AverageBleedDamage { get; }
        IStatBuilder BleedDps { get; }

        IStatBuilder CastRate { get; }
        IStatBuilder CastTime { get; }

        IStatBuilder AilmentDealtDamageType(Ailment ailment);
        IDamageRelatedStatBuilder AilmentChanceWithCrits(Ailment ailment);
        IDamageRelatedStatBuilder AilmentEffectiveChance(Ailment ailment);
        IStatBuilder AilmentCombinedEffectiveChance(Ailment ailment);

        IDamageRelatedStatBuilder EffectiveCritChance { get; }

        IStatBuilder ResistanceAgainstHits(DamageType damageType);
        IStatBuilder MitigationAgainstHits(DamageType damageType);
        IStatBuilder MitigationAgainstDoTs(DamageType damageType);
        IStatBuilder ChanceToAvoidMeleeAttacks { get; }
        IStatBuilder ChanceToAvoidProjectileAttacks { get; }
        IStatBuilder ChanceToAvoidSpells { get; }

        IDamageRelatedStatBuilder EffectiveStunThreshold { get; }
        IStatBuilder StunAvoidanceWhileCasting { get; }

        IStatBuilder SkillHitDamageSource { get; }
    }
}