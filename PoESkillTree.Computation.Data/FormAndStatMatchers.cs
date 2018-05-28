﻿using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Modifiers;
using PoESkillTree.Computation.Common.Builders.Resolving;
using PoESkillTree.Computation.Common.Data;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;
using static PoESkillTree.Computation.Common.Builders.Values.ValueBuilderUtils;

namespace PoESkillTree.Computation.Data
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="IStatMatchers"/> implementation matching stat parts specifying forms, values and stats.
    /// </summary>
    public class FormAndStatMatchers : StatMatchersBase
    {
        private readonly IModifierBuilder _modifierBuilder;

        public FormAndStatMatchers(
            IBuilderFactories builderFactories, IMatchContexts matchContexts, IModifierBuilder modifierBuilder)
            : base(builderFactories, matchContexts)
        {
            _modifierBuilder = modifierBuilder;
        }

        protected override IEnumerable<MatcherData> CreateCollection() =>
            new FormAndStatMatcherCollection(_modifierBuilder, ValueFactory)
            {
                // attributes
                // offense
                // - damage
                {
                    @"adds # to # ({DamageTypeMatchers}) damage",
                    BaseAdd, ValueFactory.FromMinAndMax(Values[0], Values[1]), Reference.AsDamageType.Damage
                },
                {
                    @"# to # additional ({DamageTypeMatchers}) damage",
                    BaseAdd, ValueFactory.FromMinAndMax(Values[0], Values[1]), Reference.AsDamageType.Damage
                },
                {
                    @"adds # maximum ({DamageTypeMatchers}) damage",
                    BaseAdd, Value.MaximumOnly, Reference.AsDamageType.Damage
                },
                { "deal no ({DamageTypeMatchers}) damage", TotalOverride, 0, Reference.AsDamageType.Damage },
                // - penetration
                {
                    "damage penetrates #% (of enemy )?({DamageTypeMatchers}) resistances?",
                    BaseAdd, Value, Reference.AsDamageType.Penetration
                },
                {
                    "damage (?<inner>with .*|dealt by .*) penetrates #% ({DamageTypeMatchers}) resistances?",
                    BaseAdd, Value, Reference.AsDamageType.Penetration, "${inner}"
                },
                {
                    "penetrate #% ({DamageTypeMatchers}) resistances?",
                    BaseAdd, Value, Reference.AsDamageType.Penetration
                },
                // - crit
                { @"\+#% critical strike chance", BaseAdd, Value, CriticalStrike.Chance },
                {
                    "no critical strike multiplier, no damage multiplier for ailments from critical strikes",
                    TotalOverride, 0, CriticalStrike.Multiplier
                },
                { "never deal critical strikes", TotalOverride, 0, CriticalStrike.Chance },
                // - speed
                // - projectiles
                { "skills fire an additional projectile", BaseAdd, 1, Projectile.Count },
                { "pierces # additional targets", BaseAdd, Value, Projectile.PierceCount },
                { "projectiles pierce an additional target", BaseAdd, 1, Projectile.PierceCount },
                { "projectiles pierce # (additional )?targets", BaseAdd, Value, Projectile.PierceCount },
                {
                    "projectiles pierce all nearby targets",
                    TotalOverride, double.PositiveInfinity, Projectile.PierceCount, Enemy.IsNearby
                },
                { @"skills chain \+# times", BaseAdd, Value, Projectile.ChainCount },
                // - other
                { "your hits can't be evaded", TotalOverride, 0, Evasion.Chance.For(Enemy) },
                // defense
                // - life, mana, defences
                { "maximum life becomes #", TotalOverride, Value, Life },
                { "removes all mana", BaseOverride, 0, Mana },
                { "converts all evasion rating to armour", TotalOverride, 100, Evasion.ConvertTo(Armour) },
                { "cannot evade enemy attacks", TotalOverride, 0, Evasion.Chance },
                // - resistances
                { "immune to ({DamageTypeMatchers}) damage", TotalOverride, 100, Reference.AsDamageType.Resistance },
                { @"\+#% elemental resistances", BaseAdd, Value, Elemental.Resistance },
                { @"\+?#% physical damage reduction", BaseAdd, Value, Physical.Resistance },
                // - leech
                {
                    "life leech is applied to energy shield instead",
                    TotalOverride, 1, Life.Leech.AppliesTo(EnergyShield)
                },
                { "gain life from leech instantly", TotalOverride, 1, Life.InstantLeech },
                { "leech #% of damage as life", BaseAdd, Value, Life.Leech.Of(Damage) },
                // - block
                {
                    "#% of block chance applied to spells",
                    BaseAdd, Value.PercentOf(Block.AttackChance), Block.SpellChance
                },
                // - other
                {
                    "chaos damage does not bypass energy shield",
                    TotalOverride, 100, Chaos.Damage.TakenFrom(EnergyShield).Before(Life)
                },
                {
                    "#% of chaos damage does not bypass energy shield",
                    BaseAdd, Value, Chaos.Damage.TakenFrom(EnergyShield).Before(Life)
                },
                {
                    "#% of physical damage bypasses energy shield",
                    BaseSubtract, Value, Physical.Damage.TakenFrom(EnergyShield).Before(Life)
                },
                {
                    "you take #% reduced extra damage from critical strikes",
                    PercentReduce, Value, CriticalStrike.ExtraDamageTaken
                },
                // regen and recharge 
                // (need to be FormAndStatMatcher because they also exist with flat values)
                {
                    "#% of ({PoolStatMatchers}) regenerated per second",
                    BaseAdd, Value, Reference.AsPoolStat.Regen.Percent
                },
                {
                    "#% of ({PoolStatMatchers}) and ({PoolStatMatchers}) regenerated per second",
                    BaseAdd, Value, References[0].AsPoolStat.Regen.Percent, References[1].AsPoolStat.Regen.Percent
                },
                {
                    "regenerate #%( of)?( their| your)? ({PoolStatMatchers}) per second",
                    BaseAdd, Value, Reference.AsPoolStat.Regen.Percent
                },
                {
                    "# ({PoolStatMatchers}) regenerated per second", BaseAdd, Value,
                    Reference.AsPoolStat.Regen
                },
                {
                    "#% faster start of energy shield recharge", PercentIncrease, Value,
                    EnergyShield.Recharge.Start
                },
                { "life regeneration has no effect", BaseOverride, 0, Life.Regen },
                {
                    "life regeneration is applied to energy shield instead",
                    TotalOverride, 1, Life.Regen.AppliesTo(EnergyShield)
                },
                // gain (need to be FormAndStatMatcher because they also exist with flat values)
                {
                    "#% of ({PoolStatMatchers}) gained",
                    BaseAdd, Value.PercentOf(Reference.AsStat), Reference.AsPoolStat.Gain
                },
                {
                    "recover #% of( their)? ({PoolStatMatchers})",
                    BaseAdd, Value.PercentOf(Reference.AsStat), Reference.AsPoolStat.Gain
                },
                {
                    "removes #% of ({PoolStatMatchers})",
                    BaseSubtract, Value.PercentOf(Reference.AsStat), Reference.AsPoolStat.Gain
                },
                { @"\+# ({PoolStatMatchers}) gained", BaseAdd, Value, Reference.AsPoolStat.Gain },
                // charges
                // skills
                // traps, mines, totems
                {
                    "detonating mines is instant", 
                    TotalOverride, double.PositiveInfinity, Stat.CastSpeed, With(Skill.DetonateMines)
                },
                // minions
                // buffs
                {
                    "(?<!while |chance to )you have ({BuffMatchers})",
                    TotalOverride, 1, Reference.AsBuff.NotAsBuffOn(Self)
                },
                {
                    "(?<!while |chance to )gain ({BuffMatchers})",
                    TotalOverride, 1, Reference.AsBuff.On(Self)
                },
                {
                    "you can have one additional curse", 
                    BaseAdd, 1, Buffs(target: Self).With(Keyword.Curse).CombinedLimit
                },
                {
                    "enemies can have # additional curse",
                    BaseAdd, Value, Buffs(target: Enemy).With(Keyword.Curse).CombinedLimit
                },
                { "grants fortify", TotalOverride, 1, Buff.Fortify.On(Self) },
                {
                    "curse enemies with level # ({SkillMatchers})",
                    TotalOverride, 1, Buff.Curse(skill: Reference.AsSkill, level: Value).On(Enemy)
                },
                { "gain elemental conflux", TotalOverride, 1, Buff.Conflux.Elemental.On(Self) },
                // flags
                // ailments
                { "causes bleeding", TotalOverride, 100, Ailment.Bleed.Chance },
                { "always poison", TotalOverride, 100, Ailment.Poison.Chance },
                {
                    "(you )?can afflict an additional ignite on an enemy",
                    BaseAdd, 1, Ailment.Ignite.InstancesOn(Enemy).Maximum
                },
                { "you are immune to ({AilmentMatchers})", TotalOverride, 100, Reference.AsAilment.Avoidance },
                { "cannot be ({AilmentMatchers})", TotalOverride, 100, Reference.AsAilment.Avoidance },
                {
                    "(immune to|cannot be affected by) elemental ailments",
                    TotalOverride, 100, Ailment.Elemental.Select(a => a.Avoidance)
                },
                {
                    "poison you inflict with critical strikes deals #% more damage",
                    PercentMore, Value, CriticalStrike.Multiplier.With(Ailment.Poison)
                },
                // stun
                { "(you )?cannot be stunned", TotalOverride, 100, Effect.Stun.Avoidance },
                { "your damaging hits always stun enemies", TotalOverride, 100, Effect.Stun.ChanceOn(Enemy), Hit.On() },
                // item quantity/quality
                // range and area of effect
                // other
                { "knocks back enemies", TotalOverride, 100, Effect.Knockback.ChanceOn(Enemy) },
            };
    }
}