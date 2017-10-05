﻿using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;
using PoESkillTree.Computation.Parsing.Builders;
using PoESkillTree.Computation.Parsing.Builders.Matching;
using PoESkillTree.Computation.Parsing.Data;
using PoESkillTree.Computation.Parsing.ModifierBuilding;
using static PoESkillTree.Computation.Parsing.Builders.Values.ValueBuilderUtils;

namespace PoESkillTree.Computation.Data
{
    public class FormAndStatMatchers : UsesMatchContext, IStatMatchers
    {
        private readonly IModifierBuilder _modifierBuilder;
        private readonly Lazy<IReadOnlyList<MatcherData>> _lazyMatchers;

        public FormAndStatMatchers(IBuilderFactories builderFactories,
            IMatchContexts matchContexts, IModifierBuilder modifierBuilder)
            : base(builderFactories, matchContexts)
        {
            _modifierBuilder = modifierBuilder;
            _lazyMatchers = new Lazy<IReadOnlyList<MatcherData>>(() => CreateCollection().ToList());
        }

        public IReadOnlyList<MatcherData> Matchers => _lazyMatchers.Value;

        private FormAndStatMatcherCollection CreateCollection() => new FormAndStatMatcherCollection(
            _modifierBuilder, ValueFactory)
        {
            // attributes
            // offense
            // - damage
            {
                @"adds # to # ({DamageTypeMatchers}) damage",
                (MinBaseAdd, MaximumAdd), (Values[0], Values[1]), Group.AsDamageType.Damage
            },
            {
                @"# to # additional ({DamageTypeMatchers}) damage",
                (MinBaseAdd, MaximumAdd), (Values[0], Values[1]), Group.AsDamageType.Damage
            },
            {
                @"adds # maximum ({DamageTypeMatchers}) damage",
                MaxBaseAdd, Value, Group.AsDamageType.Damage
            },
            { "deal no ({DamageTypeMatchers}) damage", TotalOverride, 0, Group.AsDamageType.Damage },
            // - penetration
            {
                "damage penetrates #% ({DamageTypeMatchers}) resistances?",
                BaseAdd, Value, Group.AsDamageType.Penetration
            },
            {
                "damage (with .*) penetrates #% ({DamageTypeMatchers}) resistances?",
                BaseAdd, Value, Group.AsDamageType.Penetration, "$1"
            },
            {
                "penetrate #% ({DamageTypeMatchers}) resistances?",
                BaseAdd, Value, Group.AsDamageType.Penetration, "$1"
            },
            // - crit
            { @"\+#% critical strike chance", BaseAdd, Value, CriticalStrike.Chance },
            { "no critical strike multiplier", TotalOverride, 0, CriticalStrike.Multiplier },
            {
                "no damage multiplier for ailments from critical strikes",
                TotalOverride, 0, CriticalStrike.AilmentMultiplier
            },
            { "never deal critical strikes", TotalOverride, 0, CriticalStrike.Chance },
            // - speed
            // - projectiles
            { "skills fire an additional projectile", BaseAdd, 1, Projectile.Count },
            { "pierces # additional targets", BaseAdd, Value, Projectile.PierceCount },
            { "projectiles pierce an additional target", BaseAdd, 1, Projectile.PierceCount },
            { "projectiles pierce # targets", BaseAdd, Value, Projectile.PierceCount },
            {
                "projectiles pierce all nearby targets",
                TotalOverride, double.PositiveInfinity, Projectile.PierceCount, Enemy.IsNearby
            },
            { @"skills chain \+# times", BaseAdd, Value, Projectile.ChainCount },
            // - other
            { "your hits can't be evaded", TotalOverride, 0, Enemy.Stat(Evasion.Chance) },
            // defense
            // - life, mana, defences
            { "maximum life becomes #", TotalOverride, Value, Life },
            { "removes all mana", TotalOverride, 0, Mana },
            {
                "converts all evasion rating to armour",
                TotalOverride, 100, Evasion.ConvertTo(Armour)
            },
            { "cannot evade enemy attacks", TotalOverride, 0, Evasion.Chance },
            // - resistances
            {
                "immune to ({DamageTypeMatchers}) damage",
                TotalOverride, 100, Group.AsDamageType.Resistance
            },
            // - leech
            {
                "life leech is applied to energy shield instead", TotalOverride, 1,
                Life.Leech.AppliesTo(EnergyShield)
            },
            { "gain life from leech instantly", TotalOverride, 1, Life.InstantLeech },
            { "leech #% of damage as life", BaseAdd, Value, Life.Leech.Of(Damage) },
            // - block
            // - other
            {
                "chaos damage does not bypass energy shield",
                TotalOverride, 100, Chaos.Damage.TakenFrom(EnergyShield).Before(Life)
            },
            {
                "#% of chaos damage does not bypass energy shield",
                BaseAdd, Value, Chaos.Damage.TakenFrom(EnergyShield).Before(Life),
                Chaos.Damage.TakenFrom(EnergyShield).Before(Mana)
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
                BaseAdd, Value, Group.AsPoolStat.Regen.Percent
            },
            {
                "#% of ({PoolStatMatchers}) and ({PoolStatMatchers}) regenerated per second",
                BaseAdd, Value,
                Groups[0].AsPoolStat.Regen.Percent,
                Groups[1].AsPoolStat.Regen.Percent
            },
            {
                "regenerate #%( of)?( their)? ({PoolStatMatchers}) per second",
                BaseAdd, Value, Group.AsPoolStat.Regen.Percent
            },
            {
                "# ({PoolStatMatchers}) regenerated per second", BaseAdd, Value,
                Group.AsPoolStat.Regen
            },
            {
                "#% faster start of energy shield recharge", PercentIncrease, Value,
                EnergyShield.Recharge.Start
            },
            { "life regeneration has no effect", PercentLess, 100, Life.Regen },
            {
                "life regeneration is applied to energy shield instead", TotalOverride, 1,
                Life.Regen.AppliesTo(EnergyShield)
            },
            // gain (need to be FormAndStatMatcher because they also exist with flat values)
            {
                "#% of ({PoolStatMatchers}) gained",
                BaseAdd, Value, Group.AsPoolStat.Gain, PercentOf(Group.AsStat)
            },
            {
                "recover #% of( their)? ({PoolStatMatchers})",
                BaseAdd, Value, Group.AsPoolStat.Gain, PercentOf(Group.AsStat)
            },
            {
                "removes #% of ({PoolStatMatchers})",
                BaseSubtract, Value, Group.AsPoolStat.Gain, PercentOf(Group.AsStat)
            },
            { @"\+# ({PoolStatMatchers}) gained", BaseAdd, Value, Group.AsPoolStat.Gain },
            // charges
            // skills
            // traps, mines, totems
            {
                "detonating mines is instant",
                TotalOverride, double.PositiveInfinity, Skill.DetonateMines.Speed
            },
            // minions
            // buffs
            {
                "you can have one additional curse",
                BaseAdd, 1, Buffs(target: Self).With(Keyword.Curse).CombinedLimit
            },
            {
                "enemies can have # additional curse",
                BaseAdd, Value, Buffs(target: Enemy).With(Keyword.Curse).CombinedLimit
            },
            { "grants fortify", TotalOverride, 1, Buff.Fortify.On(Self) },
            { "you have fortify", TotalOverride, 1, Buff.Fortify.On(Self) },
            {
                @"curse enemies with level # ({SkillMatchers})",
                TotalOverride, 1, Buff.Curse(skill: Group.AsSkill, level: Value).On(Enemy)
            },
            { "gain elemental conflux", TotalOverride, 1, Buff.Conflux.Elemental.On(Self) },
            // flags
            {
                "(?<!while )(you have|gain) ({FlagMatchers})", TotalOverride, 1,
                Group.AsFlagStat
            },
            // ailments
            { "causes bleeding", TotalOverride, 100, Ailment.Bleed.Chance },
            { "always poison", TotalOverride, 100, Ailment.Poison.Chance },
            {
                "(you )?can afflict an additional ignite on an enemy",
                BaseAdd, 1, Ailment.Ignite.InstancesOn(Enemy).Maximum
            },
            { "you are immune to ({AilmentMatchers})", TotalOverride, 100, Group.AsAilment.Avoidance },
            { "cannot be ({AilmentMatchers})", TotalOverride, 100, Group.AsAilment.Avoidance },
            {
                "(immune to|cannot be affected by) elemental ailments",
                TotalOverride, 100, Ailment.Elemental.Select(a => a.Avoidance)
            },
            // stun
            { "(you )?cannot be stunned", TotalOverride, 100, Effect.Stun.Avoidance },
            { "your damaging hits always stun enemies", TotalOverride, 100, Effect.Stun.ChanceOn(Enemy) },
            // item quantity/quality
            // range and area of effect
            // other
            { "knocks back enemies", TotalOverride, 100, Effect.Knockback.ChanceOn(Enemy) },
        };
    }
}
