using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Base;
using PoESkillTree.Engine.Computation.Data.Collections;
using PoESkillTree.Engine.GameModel.Items;
using static PoESkillTree.Engine.Computation.Common.Builders.Values.ValueBuilderUtils;

namespace PoESkillTree.Engine.Computation.Data
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="IStatMatchers"/> implementation matching stat parts specifying forms, values and stats.
    /// </summary>
    public class FormAndStatMatchers : StatMatchersBase
    {
        private readonly IModifierBuilder _modifierBuilder;

        public FormAndStatMatchers(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories)
        {
            _modifierBuilder = modifierBuilder;
        }

        protected override IReadOnlyList<MatcherData> CreateCollection() =>
            new FormAndStatMatcherCollection(_modifierBuilder, ValueFactory)
            {
                // attributes
                // offense
                // - damage
                {
                    @"adds # to # ({DamageTypeMatchers}) damage",
                    BaseAdd, ValueFactory.FromMinAndMax(Values[0], Values[1]), Reference.AsDamageType.Damage.WithHits
                },
                {
                    @"# to # added ({DamageTypeMatchers}) damage",
                    BaseAdd, ValueFactory.FromMinAndMax(Values[0], Values[1]), Reference.AsDamageType.Damage.WithHits
                },
                {
                    @"# to # added attack ({DamageTypeMatchers}) damage",
                    BaseAdd, ValueFactory.FromMinAndMax(Values[0], Values[1]),
                    Reference.AsDamageType.Damage.WithSkills(DamageSource.Attack)
                },
                {
                    "# to # ({DamageTypeMatchers}) damage",
                    BaseAdd, ValueFactory.FromMinAndMax(Values[0], Values[1]), Reference.AsDamageType.Damage.WithHits
                },
                {
                    @"adds # to # ({DamageTypeMatchers}) (damage to attacks|attack damage)",
                    BaseAdd, ValueFactory.FromMinAndMax(Values[0], Values[1]),
                    Reference.AsDamageType.Damage.WithSkills(DamageSource.Attack)
                },
                {
                    @"adds # to # ({DamageTypeMatchers}) damage to unarmed attacks",
                    BaseAdd, ValueFactory.FromMinAndMax(Values[0], Values[1]),
                    Reference.AsDamageType.Damage.WithSkills(DamageSource.Attack).With(Keyword.Melee),
                    Not(MainHand.HasItem)
                },
                {
                    @"adds # to # ({DamageTypeMatchers}) damage to bow attacks",
                    BaseAdd, ValueFactory.FromMinAndMax(Values[0], Values[1]),
                    Reference.AsDamageType.Damage.WithSkills(DamageSource.Attack).With(Keyword.Melee),
                    MainHand.Has(Tags.Bow)
                },
                {
                    @"adds # to # ({DamageTypeMatchers}) (damage to spells|spell damage)",
                    BaseAdd, ValueFactory.FromMinAndMax(Values[0], Values[1]),
                    Reference.AsDamageType.Damage.WithSkills(DamageSource.Spell)
                },
                {
                    @"adds # to # ({DamageTypeMatchers}) damage to spells and attacks",
                    BaseAdd, ValueFactory.FromMinAndMax(Values[0], Values[1]),
                    Reference.AsDamageType.Damage.WithSkills(DamageSource.Spell),
                    Reference.AsDamageType.Damage.WithSkills(DamageSource.Attack)
                },
                {
                    @"# to # additional ({DamageTypeMatchers}) damage",
                    BaseAdd, ValueFactory.FromMinAndMax(Values[0], Values[1]), Reference.AsDamageType.Damage.WithHits
                },
                {
                    @"adds # maximum ({DamageTypeMatchers}) damage",
                    BaseAdd, Value.MaximumOnly, Reference.AsDamageType.Damage.WithHits
                },
                {
                    @"adds # maximum ({DamageTypeMatchers}) damage to attacks",
                    BaseAdd, Value.MaximumOnly, Reference.AsDamageType.Damage.WithSkills(DamageSource.Attack)
                },
                { "deal no ({DamageTypeMatchers}) damage", TotalOverride, 0, Reference.AsDamageType.Damage },
                {
                    @"explosion deals (base )?({DamageTypeMatchers}) damage equal to #% of the (corpse|monster)'s maximum life",
                    BaseSet, Value.AsPercentage * Life.For(Enemy).Value,
                    Reference.AsDamageType.Damage.WithSkills(DamageSource.Secondary)
                },
                {
                    @"explosion deals # to # base ({DamageTypeMatchers}) damage",
                    BaseSet, ValueFactory.FromMinAndMax(Values[0], Values[1]),
                    Reference.AsDamageType.Damage.WithSkills(DamageSource.Secondary)
                },
                {
                    "deals # to # base ({DamageTypeMatchers}) damage",
                    BaseSet, ValueFactory.FromMinAndMax(Values[0], Values[1]),
                    Reference.AsDamageType.Damage.WithSkills(DamageSource.Spell)
                },
                {
                    "# to # base off hand ({DamageTypeMatchers}) damage",
                    BaseSet, ValueFactory.FromMinAndMax(Values[0], Values[1]),
                    Reference.AsDamageType.Damage.WithSkills.With(AttackDamageHand.OffHand)
                },
                {
                    "deal up to #% more melee damage to enemies, based on proximity",
                    PercentMore, Value * ValueFactory.LinearScale(OpponentsOfSelf.Distance, (15, 1), (40, 0)),
                    Damage.With(Keyword.Melee)
                },
                // - damage taken
                {
                    "cold damage taken increased by chill effect",
                    PercentIncrease, 100 * (Ailment.ChillEffect.Value - 1),
                    Cold.Damage.Taken.With(DamageSource.OverTime)
                },
                // - damage taken as
                // - conversion and gain
                {
                    "(gain )?#% of ({DamageTypeMatchers}) damage (gained |added )?as (extra )?({DamageTypeMatchers}) damage",
                    BaseAdd, Value, References[0].AsDamageType.Damage.WithHitsAndAilments
                        .GainAs(References[1].AsDamageType.Damage.WithHitsAndAilments)
                },
                {
                    "gain #% of ({DamageTypeMatchers}) damage as extra damage of a random element",
                    BaseAdd, Value, Reference.AsDamageType.Damage.WithHitsAndAilments
                        .GainAs(RandomElement.Damage.WithHitsAndAilments)
                },
                {
                    "gain #% of wand ({DamageTypeMatchers}) damage as extra ({DamageTypeMatchers}) damage",
                    BaseAdd, Value,
                    References[0].AsDamageType.Damage.With(AttackDamageHand.MainHand)
                        .GainAs(References[1].AsDamageType.Damage.With(AttackDamageHand.MainHand))
                        .WithCondition(MainHand.Has(Tags.Wand)),
                    References[0].AsDamageType.Damage.With(AttackDamageHand.OffHand)
                        .GainAs(References[1].AsDamageType.Damage.With(AttackDamageHand.OffHand))
                        .WithCondition(OffHand.Has(Tags.Wand))
                },
                {
                    "({DamageTypeMatchers}) spells have #% of ({DamageTypeMatchers}) damage converted to ({DamageTypeMatchers}) damage",
                    BaseAdd, Value, References[1].AsDamageType.Damage.With(DamageSource.Spell)
                        .ConvertTo(References[2].AsDamageType.Damage.With(DamageSource.Spell)),
                    With(References[0].AsDamageType)
                },
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
                    "penetrates? #% ({DamageTypeMatchers}) resistances?",
                    BaseAdd, Value, Reference.AsDamageType.Penetration
                },
                {
                    "({KeywordMatchers}) damage penetrates #% ({DamageTypeMatchers}) resistances?",
                    BaseAdd, Value, References[1].AsDamageType.Penetration.With(References[0].AsKeyword)
                },
                {
                    "({KeywordMatchers}) damage (?<inner>with .*|dealt by .*) penetrates #% ({DamageTypeMatchers}) resistances?",
                    BaseAdd, Value, References[1].AsDamageType.Penetration.With(References[1].AsKeyword), "${inner}"
                },
                {
                    "hits ignore enemy monster ({DamageTypeMatchers}) resistance",
                    TotalOverride, 1, Reference.AsDamageType.IgnoreResistance
                },
                { "enemies have #% to total physical damage reduction against your hits", BaseAdd, Value, Physical.Penetration },
                {
                    "enemies (you impale|impaled by supported skills) have #% to total physical damage reduction against impale hits",
                    BaseAdd, Value, Buff.Impale.Penetration
                },
                // - exposure
                {
                    @"(?<damageType>({DamageTypeMatchers})) exposure applies #% to \k<damageType> resistance",
                    BaseSet, Value, Reference.AsDamageType.Exposure
                },
                {
                    @"fire, cold and lightning exposure (?<inner>.*), applying #% to those resistances",
                    BaseSet, Value, ElementalDamageTypes.Select(t => t.Exposure).Aggregate((l, r) => l.Concat(r))
                },
                // - crit
                { @"\+#% critical strike chance", BaseAdd, Value, CriticalStrike.Chance },
                { @"\+#% critical strike multiplier", BaseAdd, Value, CriticalStrike.Multiplier },
                { "no critical strike multiplier", TotalOverride, 100, CriticalStrike.Multiplier },
                { "your critical strikes do not deal extra damage", TotalOverride, 100, CriticalStrike.Multiplier },
                {
                    "ailments never count as being from critical strikes",
                    TotalOverride, 0, CriticalStrike.Chance.WithAilments
                },
                { "never deal critical strikes", TotalOverride, 0, CriticalStrike.Chance },
                { "your critical strike chance is lucky", TotalOverride, 1, Flag.CriticalStrikeChanceIsLucky },
                {
                    "base off hand critical strike chance is #%",
                    BaseSet, Value, CriticalStrike.Chance.WithSkills.With(AttackDamageHand.OffHand)
                },
                // - speed
                { "actions are #% slower", PercentLess, Value, Stat.ActionSpeed },
                {
                    "action speed cannot be modified to below base value",
                    TotalOverride, 1, Stat.ActionSpeed.Minimum
                },
                { @"\+# seconds to attack time", BaseAdd, Value, Stat.BaseCastTime.With(DamageSource.Attack) },
                {
                    "base off hand attack time is # seconds",
                    (BaseSet, Value, Stat.BaseCastTime.With(AttackDamageHand.OffHand)),
                    (BaseSet, Stat.BaseCastTime.With(AttackDamageHand.OffHand).Value.Invert, Stat.CastRate.With(AttackDamageHand.OffHand))
                },
                // - projectiles
                { "fires? # additional projectiles", BaseAdd, Value, Projectile.Count },
                { "fires? # additional arrows", BaseAdd, Value, Projectile.Count, With(Keyword.Attack) },
                { "fires? an additional projectile", BaseAdd, 1, Projectile.Count },
                { "fires? an additional arrow", BaseAdd, 1, Projectile.Count, With(Keyword.Attack) },
                {
                    "bow attacks fire an additional arrow",
                    BaseAdd, 1, Projectile.Count, And(With(Keyword.Attack), MainHand.Has(Tags.Bow))
                },
                { "skills fire an additional projectile", BaseAdd, 1, Projectile.Count },
                { "skills fire # additional projectiles", BaseAdd, Value, Projectile.Count },
                { "supported skills fire # additional projectiles", BaseAdd, Value, Projectile.Count },
                { "totems fire # additional projectiles", BaseAdd, 1, Projectile.Count, With(Keyword.Totem) },
                { "pierces # additional targets", BaseAdd, Value, Projectile.PierceCount },
                { "projectiles pierce an additional target", BaseAdd, 1, Projectile.PierceCount },
                { "arrows pierce an additional target", BaseAdd, 1, Projectile.PierceCount, With(Keyword.Attack) },
                { "projectiles pierce # (additional )?targets", BaseAdd, Value, Projectile.PierceCount },
                {
                    "arrows pierce # (additional )?targets",
                    BaseAdd, Value, Projectile.PierceCount, With(Keyword.Attack)
                },
                {
                    "projectiles from supported skills pierce # additional targets", BaseAdd, Value,
                    Projectile.PierceCount
                },
                {
                    "projectiles pierce all nearby targets",
                    TotalOverride, double.PositiveInfinity, Projectile.PierceCount, OpponentsOfSelf.IsNearby
                },
                {
                    "projectiles pierce all targets",
                    TotalOverride, double.PositiveInfinity, Projectile.PierceCount
                },
                {
                    "arrows pierce all targets",
                    TotalOverride, double.PositiveInfinity, Projectile.PierceCount, With(Keyword.Attack)
                },
                { @"chains \+# times", BaseAdd, Value, Projectile.ChainCount },
                { @"(supported )?skills chain \+# times", BaseAdd, Value, Projectile.ChainCount },
                {
                    "fires? projectiles sequentially",
                    TotalOverride, Projectile.Count.Value, Stat.SkillNumberOfHitsPerCast
                },
                // - other
                { "(your )?hits can't be evaded", TotalOverride, 100, Stat.ChanceToHit },
                { "can't be evaded", TotalOverride, 100, Stat.ChanceToHit },
                // defense
                // - life, mana, defences
                { "maximum life becomes #", TotalOverride, Value, Life },
                { "removes all mana", TotalOverride, 0, Mana },
                {
                    "gain #% of maximum ({PoolStatMatchers}) as extra maximum energy shield",
                    BaseAdd, Value, Reference.AsPoolStat.GainAs(EnergyShield)
                },
                { "converts all evasion rating to armour", TotalOverride, 100, Evasion.ConvertTo(Armour) },
                { "cannot evade enemy attacks", TotalOverride, 0, Evasion.Chance },
                { @"\+# evasion rating", BaseAdd, Value, Evasion },
                // - resistances
                { "immune to ({DamageTypeMatchers}) damage", TotalOverride, 100, Reference.AsDamageType.Resistance },
                { @"\+#% elemental resistances", BaseAdd, Value, Elemental.Resistance },
                { @"\+?#% physical damage reduction", BaseAdd, Value, Physical.Resistance },
                // - leech
                {
                    "leech energy shield instead of life",
                    TotalOverride, 100, Life.Leech.Of(Damage).ConvertTo(EnergyShield.Leech.Of(Damage))
                },
                { "gain life from leech instantly", TotalOverride, 1, Life.Leech.IsInstant },
                { "leech #% of damage as life", BaseAdd, Value, Life.Leech.Of(Damage) },
                { "cannot leech mana", TotalOverride, 0, Mana.Leech.Of(Damage) },
                // - block
                {
                    "#% of block chance applied to spells",
                    BaseAdd, Value.PercentOf(Block.AttackChance), Block.SpellChance
                },
                // - other
                {
                    "chaos damage does not bypass energy shield",
                    TotalOverride, 100, Chaos.DamageTakenFrom(EnergyShield).Before(Life)
                },
                {
                    "#% of chaos damage does not bypass energy shield",
                    BaseAdd, Value, Chaos.DamageTakenFrom(EnergyShield).Before(Life)
                },
                {
                    "#% of physical damage bypasses energy shield",
                    BaseSubtract, Value, Physical.DamageTakenFrom(EnergyShield).Before(Life)
                },
                {
                    "you take no extra damage from critical strikes",
                    PercentLess, 100, CriticalStrike.ExtraDamageTaken
                },
                // regen and recharge 
                // (need to be FormAndStatMatcher because they also exist with flat values)
                {
                    "#%( of)? ({PoolStatMatchers}) regenerated per second",
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
                    "lose #%( of)?( their| your)? ({PoolStatMatchers}) per second",
                    BaseSubtract, Value, Reference.AsPoolStat.Regen.Percent
                },
                {
                    "# ({PoolStatMatchers}) regenerated per second",
                    BaseAdd, Value, Reference.AsPoolStat.Regen
                },
                {
                    "regenerate # ({PoolStatMatchers}) per second",
                    BaseAdd, Value, Reference.AsPoolStat.Regen
                },
                {
                    "#% of ({StatMatchers}) is regenerated as ({PoolStatMatchers}) per second",
                    BaseAdd, Value.PercentOf(References[0].AsStat), References[1].AsPoolStat.Regen
                },
                {
                    "#% faster start of energy shield recharge", PercentIncrease, Value,
                    EnergyShield.Recharge.Start
                },
                { "life regeneration has no effect", PercentLess, 100, Life.Regen },
                {
                    "life regeneration is applied to energy shield instead",
                    TotalOverride, (int) Pool.EnergyShield, Life.Regen.TargetPool
                },
                {
                    "regenerate #% of ({PoolStatMatchers}) over # seconds when you consume a corpse",
                    BaseAdd, Values[0] / Values[1], Reference.AsPoolStat.Regen.Percent,
                    Action.ConsumeCorpse.InPastXSeconds(Values[1])
                },
                // degen
                {
                    "you (take|burn for) #% of your ({PoolStatMatchers}) per second as ({DamageTypeMatchers}) damage",
                    BaseAdd, Value.AsPercentage * References[0].AsPoolStat.Value,
                    References[0].AsPoolStat.Degeneration(References[1].AsDamageType)
                },
                // gain (need to be FormAndStatMatcher because they also exist with flat values)
                {
                    "#% of ({PoolStatMatchers}) gained",
                    BaseAdd, Value.PercentOf(Reference.AsStat), Reference.AsPoolStat.Gain
                },
                {
                    "recover #% of( their| your)? ({PoolStatMatchers})",
                    BaseAdd, Value.PercentOf(Reference.AsStat), Reference.AsPoolStat.Gain
                },
                {
                    "recover #% of( their| your)? ({PoolStatMatchers}) and ({PoolStatMatchers})",
                    (BaseAdd, Value.PercentOf(References[0].AsStat), References[0].AsPoolStat.Gain),
                    (BaseAdd, Value.PercentOf(References[1].AsStat), References[1].AsPoolStat.Gain)
                },
                {
                    "(removes|lose) #% of ({PoolStatMatchers})",
                    BaseSubtract, Value.PercentOf(Reference.AsStat), Reference.AsPoolStat.Gain
                },
                { @"\+# ({PoolStatMatchers}) gained", BaseAdd, Value, Reference.AsPoolStat.Gain },
                { @"gain \+# ({PoolStatMatchers})", BaseAdd, Value, Reference.AsPoolStat.Gain },
                { "replenishes energy shield by #% of armour", BaseAdd, Value.PercentOf(Armour), EnergyShield.Gain },
                {
                    "recover ({PoolStatMatchers}) equal to #% of your evasion rating",
                    BaseAdd, Value.PercentOf(Evasion), Reference.AsPoolStat.Gain
                },
                {
                    "#% chance to recover #% of ({PoolStatMatchers})",
                    BaseAdd, Values[0].AsPercentage * Values[1].PercentOf(Reference.AsStat), Reference.AsPoolStat.Gain
                },
                // charges
                {
                    "#% chance to gain a power, frenzy or endurance charge",
                    BaseAdd, Value / 3,
                    Charge.Power.ChanceToGain, Charge.Frenzy.ChanceToGain, Charge.Endurance.ChanceToGain
                },
                {
                    "(?<!chance to |when you )gain (an?|1) ({ChargeTypeMatchers})",
                    BaseAdd, 100, Reference.AsChargeType.ChanceToGain
                },
                {
                    "supported skills grant (an?|1) ({ChargeTypeMatchers})",
                    BaseAdd, 100, Reference.AsChargeType.ChanceToGain
                },
                {
                    "(?<!chance to |when you )gain a power or frenzy charge",
                    BaseAdd, 50, Charge.Power.ChanceToGain, Charge.Frenzy.ChanceToGain
                },
                {
                    "(?<!chance to |when you )gain an endurance, frenzy or power charge",
                    BaseAdd, 100/3.0,
                    Charge.Endurance.ChanceToGain, Charge.Frenzy.ChanceToGain, Charge.Power.ChanceToGain
                },
                // skills
                {
                    @"\+# seconds? to ({SkillMatchers}) cooldown",
                    BaseAdd, Value, Stat.Cooldown, With(Reference.AsSkill)
                },
                { "base duration is # seconds", BaseSet, Value, Stat.Duration },
                { @"\+# seconds? to base duration", BaseAdd, Value, Stat.Duration },
                { "base secondary duration is # seconds", BaseSet, Value, Stat.SecondaryDuration },
                {
                    "#% increased duration(?! of)",
                    PercentIncrease, Value, ApplyOnce(Stat.Duration, Stat.SecondaryDuration)
                },
                {
                    "#% reduced duration(?! of)", PercentReduce, Value, ApplyOnce(Stat.Duration, Stat.SecondaryDuration)
                },
                { "skills cost no mana", TotalOverride, 0, Mana.Cost },
                { "you can cast an additional brand", BaseAdd, 1, Skills[Keyword.Brand].CombinedInstances },
                { "repeat an additional time", BaseAdd, 1, Stat.SkillRepeats },
                { "repeat # additional times", BaseAdd, Value, Stat.SkillRepeats },
                // traps, mines, totems
                { "trap lasts # seconds", BaseSet, Value, Stat.Trap.Duration },
                { "mine lasts # seconds", BaseSet, Value, Stat.Mine.Duration },
                { "totem lasts # seconds", BaseSet, Value, Stat.Totem.Duration },
                {
                    "(detonating mines|mine detonation) is instant",
                    TotalOverride, 0, Stat.BaseCastTime, With(Skills.DetonateMines)
                },
                {
                    "(a )?base mine detonation time (of|is) # seconds",
                    TotalOverride, Value, Stat.BaseCastTime, With(Skills.DetonateMines)
                },
                {
                    @"(attack|supported) skills have \+# to maximum number of summoned ballista totems",
                    BaseAdd, Value, Totems.CombinedInstances.Maximum, With(Keyword.From(GameModel.Skills.Keyword.Ballista))
                },
                {
                    @"\+# to maximum number of summoned ballista totems",
                    BaseAdd, Value, Totems.CombinedInstances.Maximum, With(Keyword.From(GameModel.Skills.Keyword.Ballista))
                },
                // minions
                { "can summon up to # golem at a time", BaseSet, Value, Golems.CombinedInstances.Maximum },
                { "maximum # summoned golem", BaseSet, Value, Golems.CombinedInstances.Maximum },
                {
                    @"\+# seconds? to summon skeleton cooldown",
                    BaseAdd, Value, Stat.Cooldown, With(Skills.SummonSkeletons)
                },
                // buffs
                {
                    "(?<!while |chance to )you have ({BuffMatchers})",
                    TotalOverride, 1, Reference.AsBuff.NotAsBuffOn(Self)
                },
                {
                    "(?<!while |chance to )(gain|grants?) ({BuffMatchers})",
                    TotalOverride, 1, Reference.AsBuff.On(Self)
                },
                { "you can have one additional curse", BaseAdd, 1, Buff.CurseLimit },
                { "an additional curse can be applied to you", BaseAdd, 1, Buff.CurseLimit },
                { "enemies can have # additional curse", BaseAdd, Value, Buff.CurseLimit.For(OpponentsOfSelf) },
                { "(you|supported skills) can apply an additional curse", BaseAdd, 1, Buff.CurseLimit.For(OpponentsOfSelf) },
                { "unaffected by curses", PercentLess, 100, Buffs(targets: Self).With(Keyword.Curse).Effect },
                {
                    "unaffected by ({SkillMatchers})",
                    PercentLess, 100, Reference.AsSkill.Buff.EffectOn(Self).For(Entity.Any)
                },
                { "immun(e|ity) to curses", TotalOverride, 0, Buffs(targets: Self).With(Keyword.Curse).On },
                {
                    // Stat isn't useful but has to be parsed successfully because it's part of a replaced stat
                    "removes curses", BaseAdd, 0, Buff.CurseLimit, Not(Condition.True)
                },
                {
                    "monsters are hexproof",
                    TotalOverride, 0, Buffs(Self, OpponentsOfSelf).With(Keyword.Curse).On, Flag.IgnoreHexproof.IsTrue.Not
                },
                {
                    "you and nearby allies have onslaught",
                    TotalOverride, 1, Buff.Onslaught.On(Self), Buff.Onslaught.On(Ally)
                },
                { "gain elemental conflux", TotalOverride, 1, Buff.Conflux.Elemental.On(Self) },
                { "creates consecrated ground", TotalOverride, 1, Ground.Consecrated.On(Self) },
                { "(?<!chance to )impale enemies", TotalOverride, 100, Buff.Impale.Chance },
                { "(?<!chance to )intimidate enemies", TotalOverride, 100, Buff.Intimidate.Chance },
                { "({BuffMatchers}) lasts # seconds", BaseSet, Value, Reference.AsBuff.Duration },
                {
                    "supported auras do not affect you",
                    TotalOverride, 0, Skills.ModifierSourceSkill.Buff.EffectOn(Self)
                },
                { "totems cannot gain ({BuffMatchers})", TotalOverride, 0, Reference.AsBuff.On(Entity.Totem) },
                { "maximum # ({BuffMatchers}) per enemy", TotalOverride, Value, Reference.AsBuff.StackCount.For(OpponentsOfSelf).Maximum },
                // flags
                // ailments
                { "causes bleeding", TotalOverride, 100, Ailment.Bleed.Chance },
                { "bleed is applied", TotalOverride, 100, Ailment.Bleed.Chance },
                { "always poison", TotalOverride, 100, Ailment.Poison.Chance },
                { "always ({AilmentMatchers}) enemies", TotalOverride, 100, Reference.AsAilment.Chance },
                {
                    "({AilmentMatchers}) nearby enemies",
                    TotalOverride, 100, Reference.AsAilment.Chance, OpponentsOfSelf.IsNearby
                },
                { "cannot cause bleeding", TotalOverride, 0, Ailment.Bleed.Chance },
                { "cannot ignite", TotalOverride, 0, Ailment.Ignite.Chance },
                { "cannot (apply|inflict) shock", TotalOverride, 0, Ailment.Shock.Chance },
                { "cannot inflict elemental ailments", TotalOverride, 0, Ailment.Elemental.Select(s => s.Chance) },
                {
                    "(you )?can (afflict|inflict) an additional ignite on an enemy",
                    BaseAdd, 1, Ailment.Ignite.InstancesOn(OpponentsOfSelf).Maximum
                },
                {
                    "(you are )?(immune|immunity) to ({AilmentMatchers})",
                    TotalOverride, 100, Reference.AsAilment.Avoidance
                },
                {
                    "(immune|immunity) to ({AilmentMatchers}) and ({AilmentMatchers})",
                    TotalOverride, 100, References[0].AsAilment.Avoidance, References[1].AsAilment.Avoidance
                },
                { "cannot be ({AilmentMatchers})", TotalOverride, 100, Reference.AsAilment.Avoidance },
                {
                    "cannot be ({AilmentMatchers}) or ({AilmentMatchers})",
                    TotalOverride, 100, References[0].AsAilment.Avoidance, References[1].AsAilment.Avoidance
                },
                {
                    "(immune to|cannot be affected by|immunity to) elemental ailments",
                    TotalOverride, 100, Ailment.Elemental.Select(a => a.Avoidance)
                },
                { "you are immune to ailments", TotalOverride, 100, AllAilments.Select(a => a.Avoidance) },
                {
                    "poison you inflict with critical strikes deals #% more damage",
                    PercentMore, Value, AnyDamageType.DamageMultiplierWithCrits.With(Ailment.Poison)
                },
                { "removes? ({AilmentMatchers})", TotalOverride, 100, Reference.AsAilment.ChanceToRemove },
                {
                    "removes? ({AilmentMatchers}) and ({AilmentMatchers})",
                    TotalOverride, 100, References[0].AsAilment.ChanceToRemove, References[1].AsAilment.ChanceToRemove
                },
                { "removes? (ignite and )?burning", TotalOverride, 100, Ailment.Ignite.ChanceToRemove },
                {
                    "({AilmentMatchers}) you inflict deals? damage #% faster",
                    PercentIncrease, Value, Reference.AsAilment.TickRateModifier
                },
                {
                    "({AilmentMatchers}) inflicted by this skill deals damage #% faster",
                    PercentIncrease, Value, Reference.AsAilment.TickRateModifier
                },
                {
                    "damaging ailments inflicted with supported skills deal damage #% faster",
                    PercentIncrease, Value, Ailment.Ignite.TickRateModifier, Ailment.Bleed.TickRateModifier, Ailment.Poison.TickRateModifier
                },
                // stun
                { "#% increased stun threshold reduction on enemies", PercentReduce, Value, Effect.Stun.Threshold.For(OpponentsOfSelf) },
                { "(you )?cannot be stunned", TotalOverride, 100, Effect.Stun.Avoidance },
                { "additional #% chance to be stunned", BaseAdd, Value, Effect.Stun.Chance.For(OpponentsOfSelf) },
                // knockback
                { "knocks back enemies", TotalOverride, 100, Effect.Knockback.Chance },
                { "knocks enemies back", TotalOverride, 100, Effect.Knockback.Chance },
                { "knockback(?! distance)", TotalOverride, 100, Effect.Knockback.Chance },
                {
                    "adds knockback to melee attacks",
                    TotalOverride, 100, Effect.Knockback.Chance, Condition.WithPart(Keyword.Melee)
                },
                // flasks
                { "(?<!chance to |when you )gain a flask charge", BaseAdd, 100, Flask.ChanceToGainCharge },
                { "recharges # charges?", BaseAdd, Value * 100, Flask.ChanceToGainCharge },
                { "flasks gain # charges?", BaseAdd, Value * 100, Flask.ChanceToGainCharge },
                { "gain # charges?", BaseAdd, Value * 100, Flask.ChanceToGainCharge },
                { "instant recovery", BaseSet, 100, Flask.InstantRecovery },
                // item quantity/quality
                // range and area of effect
                // other
            };
    }
}