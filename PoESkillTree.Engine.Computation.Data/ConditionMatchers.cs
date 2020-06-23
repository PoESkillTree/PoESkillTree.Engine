using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Equipment;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Base;
using PoESkillTree.Engine.Computation.Data.Collections;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Items;

namespace PoESkillTree.Engine.Computation.Data
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="IStatMatchers"/> implementation matching stat parts specifying conditions.
    /// </summary>
    public class ConditionMatchers : StatMatchersBase
    {
        private readonly IModifierBuilder _modifierBuilder;

        public ConditionMatchers(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories)
        {
            _modifierBuilder = modifierBuilder;
        }

        protected override IReadOnlyList<MatcherData> CreateCollection() =>
            new ConditionMatcherCollection(_modifierBuilder)
            {
                // actions
                // - generic
                { @"if you('ve|\\u2019ve|’ve| have) ({ActionMatchers})( an enemy)? recently", Reference.AsAction.Recently },
                { @"if you haven('|\\u2019|’)t ({ActionMatchers}) recently", Not(Reference.AsAction.Recently) },
                { @"if you('|\\u2019|’)ve ({ActionMatchers}) in the past # seconds", Reference.AsAction.InPastXSeconds(Value) },
                { "for # seconds on ({ActionMatchers})", Reference.AsAction.InPastXSeconds(Value) },
                { "on ({ActionMatchers}) for # seconds", Reference.AsAction.InPastXSeconds(Value) },
                {
                    "for # seconds on ({KeywordMatchers}) ({ActionMatchers})",
                    And(Condition.WithPart(References[0].AsKeyword), References[1].AsAction.InPastXSeconds(Value))
                },
                {
                    "(gain )?for # seconds when you ({ActionMatchers}) a rare or unique enemy",
                    And(OpponentsOfSelf.IsRareOrUnique, Reference.AsAction.InPastXSeconds(Value))
                },
                {
                    "(gain )?for # seconds when you ({ActionMatchers}) a unique enemy",
                    And(OpponentsOfSelf.IsUnique, Reference.AsAction.InPastXSeconds(Value))
                },
                {
                    "(gain )?for # seconds when you ({ActionMatchers}) an enemy",
                    Reference.AsAction.InPastXSeconds(Value)
                },
                { "when you ({ActionMatchers}) an enemy, for # seconds", Reference.AsAction.InPastXSeconds(Value) },
                { "for # seconds when ({ActionMatchers})", Reference.AsAction.By(MainOpponentOfSelf).InPastXSeconds(Value) },
                {
                    "on ({ActionMatchers}) a rare or unique enemy, lasting # seconds",
                    And(OpponentsOfSelf.IsRareOrUnique, Reference.AsAction.InPastXSeconds(Value))
                },
                // - kill
                {
                    "if you've killed a maimed enemy recently",
                    And(Kill.Recently, Buff.Maim.IsOn(MainOpponentOfSelf))
                },
                {
                    "if you've killed a bleeding enemy recently",
                    And(Kill.Recently, Ailment.Bleed.IsOn(MainOpponentOfSelf))
                },
                {
                    "if you've killed a cursed enemy recently",
                    And(Kill.Recently, Buffs(targets: OpponentsOfSelf).With(Keyword.Curse).Any())
                },
                {
                    "if you or your totems have killed recently",
                    Or(Kill.Recently, Kill.By(Entity.Totem).Recently)
                },
                {
                    "if you or your minions have killed recently",
                    Or(Kill.Recently, Kill.By(Entity.Minion).Recently)
                },
                { "if you've killed at least # enemies recently", Kill.CountRecently >= Value },
                // - hit
                { "if you('ve| have) hit them recently", Hit.Recently },
                { "if you('ve| have) been hit recently", Hit.By(MainOpponentOfSelf).Recently },
                { "if you haven't been hit recently", Not(Hit.By(MainOpponentOfSelf).Recently) },
                { "if you were damaged by a hit recently", Hit.By(MainOpponentOfSelf).Recently },
                { "if you've taken no damage from hits recently", Not(Hit.By(MainOpponentOfSelf).Recently) },
                { "if you weren't damaged by a hit recently", Not(Hit.By(MainOpponentOfSelf).Recently) },
                {
                    "if you've hit a cursed enemy recently",
                    And(Hit.Recently, Buffs(targets: OpponentsOfSelf).With(Keyword.Curse).Any())
                },
                // - critical strike
                { 
                    "if you've dealt a critical strike with a two handed melee weapon recently",
                    And(MainHandAttackWith(Tags.TwoHandWeapon), Not(MainHand.Has(Tags.Ranged)), CriticalStrike.Recently)
                },
                { "if you've crit in the past # seconds", CriticalStrike.InPastXSeconds(Value) },
                { "if you've dealt a crit in the past # seconds", CriticalStrike.InPastXSeconds(Value) },
                // - block
                { "if you've blocked damage from a unique enemy recently", And(Block.Recently, OpponentsOfSelf.IsUnique) },
                {
                    "if you've blocked damage from a unique enemy in the past # seconds",
                    And(Block.InPastXSeconds(Value), OpponentsOfSelf.IsUnique)
                },
                // - other
                {
                    "if you've stunned an enemy with a two handed melee weapon recently",
                    And(MainHand.Has(Tags.TwoHandWeapon), Not(MainHand.Has(Tags.Ranged)), With(Keyword.Attack), Effect.Stun.InflictionAction.Recently)
                },
                { "if you've taken a savage hit recently", Action.SavageHit.By(MainOpponentOfSelf).Recently },
                { "if you've spent # total mana recently", Action.SpendMana(Value).Recently },
                {
                    "for # seconds after spending( a total of)? # mana",
                    Action.SpendMana(Values[1]).InPastXSeconds(Values[0])
                },
                { "if a minion has (been killed|died) recently", Action.Die.By(Entity.Minion).Recently },
                { "while focussed", Action.Focus.Recently },
                { "for # seconds when you focus", Action.Focus.InPastXSeconds(Value) },
                // damage
                { "attacks have", Condition.With(DamageSource.Attack) },
                { "with attacks", Condition.With(DamageSource.Attack) },
                { "for attacks", Condition.With(DamageSource.Attack) },
                { "for spells", Condition.With(DamageSource.Spell) },
                { "(your )?spells have( a)?", Condition.With(DamageSource.Spell) },
                // - by item tag
                { "with axes", AttackWithSkills(Tags.Axe) },
                { "to axe attacks", AttackWithSkills(Tags.Axe) },
                { "axe attacks deal", AttackWith(Tags.Axe) },
                { "with axes (and|or) swords", AttackWithSkillsEither(Tags.Axe, Tags.Sword) },
                { "attacks with axes or swords", AttackWithSkillsEither(Tags.Axe, Tags.Sword) },
                { "axe or sword attacks deal", AttackWithEither(Tags.Axe, Tags.Sword) },
                { "with (a bow|bows)", AttackWithSkills(Tags.Bow) },
                { "with arrow hits", AttackWithSkills(Tags.Bow) },
                { "bow", AttackWithSkills(Tags.Bow) },
                { "with claws", AttackWithSkills(Tags.Claw) },
                { "claw", AttackWithSkills(Tags.Claw) },
                { "to claw attacks", AttackWithSkills(Tags.Claw) },
                { "claw attacks deal", AttackWith(Tags.Claw) },
                {
                    "with (a claw|claws) (and|or) daggers?",
                    (Or(MainHandAttackWithSkills(Tags.Claw), MainHandAttackWithSkills(Tags.Dagger)),
                        Or(OffHandAttackWithSkills(Tags.Claw), OffHandAttackWithSkills(Tags.Dagger)))
                },
                { "with daggers", AttackWithSkills(Tags.Dagger) },
                { "to dagger attacks", AttackWithSkills(Tags.Dagger) },
                { "dagger attacks deal", AttackWith(Tags.Dagger) },
                { "with maces", AttackWithSkills(Tags.Mace) },
                { "to mace attacks", AttackWithSkills(Tags.Mace) },
                { "with maces (and|or) sceptres", AttackWithSkillsEither(Tags.Mace, Tags.Sceptre) },
                { "to mace (and|or) sceptre attacks", AttackWithSkillsEither(Tags.Mace, Tags.Sceptre) },
                { "mace or sceptre attacks deal", AttackWithEither(Tags.Mace, Tags.Sceptre) },
                {
                    "with (a mace|maces), sceptres? or (staff|staves)",
                    (Or(MainHandAttackWithSkills(Tags.Mace), MainHandAttackWithSkills(Tags.Sceptre), MainHandAttackWithSkills(Tags.Staff)),
                        Or(OffHandAttackWithSkills(Tags.Mace), OffHandAttackWithSkills(Tags.Sceptre), OffHandAttackWithSkills(Tags.Staff)))
                },
                {
                    "mace, sceptre or staff attacks deal",
                    (Or(MainHandAttackWith(Tags.Mace), MainHandAttackWith(Tags.Sceptre), MainHandAttackWith(Tags.Staff)),
                        Or(OffHandAttackWith(Tags.Mace), OffHandAttackWith(Tags.Sceptre), OffHandAttackWith(Tags.Staff)))
                },
                { "with (a staff|staves)", AttackWithSkills(Tags.Staff) },
                { "to staff attacks", AttackWithSkills(Tags.Staff) },
                { "staff attacks deal", AttackWith(Tags.Staff) },
                { "with swords", AttackWithSkills(Tags.Sword) },
                { "to sword attacks", AttackWithSkills(Tags.Sword) },
                { "sword attacks deal", AttackWith(Tags.Sword) },
                { "with wands", AttackWithSkills(Tags.Wand) },
                { "wand", AttackWithSkills(Tags.Wand) },
                { "to wand attacks", AttackWithSkills(Tags.Wand) },
                { "wand attacks deal", AttackWith(Tags.Wand) },
                { "with ranged weapons", AttackWithSkills(Tags.Ranged) },
                { "with melee weapons", And(Condition.WithSkills(DamageSource.Attack), Not(MainHand.Has(Tags.Ranged))) },
                { "attacks with melee weapons deal", And(Condition.With(DamageSource.Attack), Not(MainHand.Has(Tags.Ranged))) },
                { "with one handed weapons", AttackWithSkills(Tags.OneHandWeapon) },
                { "attacks with one handed weapons deal", AttackWith(Tags.OneHandWeapon) },
                {
                    "with one handed melee weapons",
                    (And(MainHandAttackWithSkills(Tags.OneHandWeapon), Not(MainHand.Has(Tags.Ranged))),
                        And(OffHandAttackWithSkills(Tags.OneHandWeapon), Not(OffHand.Has(Tags.Ranged))))
                },
                {
                    "attacks with one handed melee weapons deal",
                    (And(MainHandAttackWith(Tags.OneHandWeapon), Not(MainHand.Has(Tags.Ranged))),
                        And(MainHandAttackWith(Tags.OneHandWeapon), Not(OffHand.Has(Tags.Ranged))))
                },
                { "with two handed weapons", AttackWithSkills(Tags.TwoHandWeapon) },
                { "attacks with two handed weapons deal", AttackWith(Tags.TwoHandWeapon) },
                { "with two handed melee weapons", And(MainHandAttackWithSkills(Tags.TwoHandWeapon), Not(MainHand.Has(Tags.Ranged))) },
                { "attacks with two handed melee weapons deal", And(MainHandAttackWith(Tags.TwoHandWeapon), Not(MainHand.Has(Tags.Ranged))) },
                { "with weapons", AttackWithSkills(Tags.Weapon) },
                { "(?<!this )weapon", AttackWithSkills(Tags.Weapon) },
                { "with unarmed attacks", And(MainHandAttackWithSkills(), Not(MainHand.HasItem)) },
                // - by item slot
                { "with the main-hand weapon", MainHandAttack },
                { "with main hand", MainHandAttack },
                { "with off hand", OffHandAttack },
                {
                    "(attacks |hits )?with this weapon( deal| have)?",
                    (ModifierSourceIs(ItemSlot.MainHand).And(MainHandAttack), ModifierSourceIs(ItemSlot.OffHand).And(OffHandAttack))
                },
                {
                    "against this weapon's hits",
                    (ModifierSourceIs(ItemSlot.MainHand).And(MainHandAttackWithSkills()),
                        ModifierSourceIs(ItemSlot.OffHand).And(OffHandAttackWithSkills()))
                },
                // - taken
                { "(?<!when you )take", Condition.DamageTaken },
                { "dealt", Condition.True },
                // equipment
                { "while unarmed", Not(MainHand.HasItem) },
                { "while wielding an axe", EitherHandHas(Tags.Axe) },
                { "while wielding an axe or sword", EitherHandHas(Tags.Axe).Or(EitherHandHas(Tags.Sword)) },
                { "while wielding a bow", MainHand.Has(Tags.Bow) },
                { "while wielding a claw", EitherHandHas(Tags.Claw) },
                { "while wielding a claw or dagger", EitherHandHas(Tags.Claw).Or(EitherHandHas(Tags.Dagger)) },
                { "while wielding a dagger", EitherHandHas(Tags.Dagger) },
                { "while wielding a mace or sceptre", Or(EitherHandHas(Tags.Mace), EitherHandHas(Tags.Sceptre)) },
                { "while wielding a mace, sceptre or staff", Or(EitherHandHas(Tags.Mace), EitherHandHas(Tags.Sceptre), EitherHandHas(Tags.Staff)) },
                { "while wielding a mace", EitherHandHas(Tags.Mace) },
                { "while wielding a staff", MainHand.Has(Tags.Staff) },
                { "while wielding a sword", EitherHandHas(Tags.Sword) },
                { "while wielding a wand", EitherHandHas(Tags.Wand) },
                { "while wielding a melee weapon", And(EitherHandHas(Tags.Weapon), Not(MainHand.Has(Tags.Ranged))) },
                { "while wielding a one handed weapon", MainHand.Has(Tags.OneHandWeapon) },
                { "while wielding a two handed weapon", MainHand.Has(Tags.TwoHandWeapon) },
                { "while wielding a two handed melee weapon", MainHand.Has(Tags.TwoHandWeapon).And(Not(MainHand.Has(Tags.Ranged))) },
                { "(if|while) dual wielding", OffHand.Has(Tags.Weapon) },
                { "while holding a shield", OffHand.Has(Tags.Shield) },
                { "while dual wielding or holding a shield", Or(OffHand.Has(Tags.Weapon), OffHand.Has(Tags.Shield)) },
                { "with shields", OffHand.Has(Tags.Shield) },
                {
                    "from equipped shield",
                    And(Condition.BaseValueComesFrom(ItemSlot.OffHand), OffHand.Has(Tags.Shield))
                },
                { "with # corrupted items equipped", Equipment.Count(e => e.Corrupted.IsTrue) >= Value },
                {
                    "while wielding two different weapon types",
                    And(OffHand.Has(Tags.Weapon), MainHand.ItemTags.Value.Eq(OffHand.ItemTags.Value).Not)
                },
                // stats
                // - pool
                { "(when|while) on low ({PoolStatMatchers})", Reference.AsPoolStat.IsLow },
                { "when not on low ({PoolStatMatchers})", Not(Reference.AsPoolStat.IsLow) },
                { "(when|while) on full ({PoolStatMatchers})", Reference.AsPoolStat.IsFull },
                { "while ({PoolStatMatchers}) is full", Reference.AsPoolStat.IsFull },
                { "while they are on full ({PoolStatMatchers})", Reference.AsPoolStat.IsFull },
                { "while not on full ({PoolStatMatchers})", Not(Reference.AsPoolStat.IsFull) },
                { "if you have ({PoolStatMatchers})", Not(Reference.AsPoolStat.IsEmpty) },
                { "while no ({PoolStatMatchers}) is reserved", Reference.AsPoolStat.Reservation.Value <= 0 },
                { "if energy shield recharge has started recently", EnergyShield.Recharge.StartedRecently },
                { "while leeching ({PoolStatMatchers})", Reference.AsPoolStat.Leech.IsActive },
                // - charges
                { "(while|if) you have no ({ChargeTypeMatchers})", Reference.AsChargeType.Amount.Value <= 0 },
                { "(while|if) you have( an?)? ({ChargeTypeMatchers})", Reference.AsChargeType.Amount.Value > 0 },
                {
                    "(while|if) you have at least # ({ChargeTypeMatchers})",
                    Reference.AsChargeType.Amount.Value >= Value
                },
                {
                    "while (at maximum|on full) ({ChargeTypeMatchers})",
                    Reference.AsChargeType.Amount.Value >= Reference.AsChargeType.Amount.Maximum.Value
                },
                { "lose a ({ChargeTypeMatchers}) and", Reference.AsChargeType.Amount.Value > 0 },
                { "if you've lost an? ({ChargeTypeMatchers}) in the past # seconds", Reference.AsChargeType.LoseAction.InPastXSeconds(Value) },
                // - other
                { "if you have at least # ({AttributeStatMatchers})", Reference.AsStat.Value >= Value },
                {
                    "if you have # primordial (jewels|items socketed or equipped)",
                    Stat.PrimordialJewelsSocketed.Value >= Value
                },
                // - on ally
                { "while there is at least one nearby ally", Ally.CountNearby >= 1 },
                { "while there are at least five nearby allies", Ally.CountNearby >= 5 },
                // - on enemy
                { "(against enemies )?that are on low life", Life.For(MainOpponentOfSelf).IsLow },
                { "against enemies on low life", Life.For(MainOpponentOfSelf).IsLow },
                { "(against enemies )?that are on full life", Life.For(MainOpponentOfSelf).IsFull },
                { "against enemies on full life", Life.For(MainOpponentOfSelf).IsFull },
                { "against rare and unique enemies", OpponentsOfSelf.IsRareOrUnique },
                { "against unique enemies", OpponentsOfSelf.IsUnique },
                { "if rare or unique", Self.IsRareOrUnique },
                { "if normal or magic", Not(Self.IsRareOrUnique) },
                { "while there is only one nearby enemy", OpponentsOfSelf.CountNearby.Eq(1) },
                { "if there are at least # nearby enemies", OpponentsOfSelf.CountNearby >= Value },
                { "at close range", OpponentsOfSelf.IsNearby },
                { "to enemies that are near you", OpponentsOfSelf.IsNearby },
                { "against nearby enemies", OpponentsOfSelf.IsNearby },
                { "while there is at most one rare or unique enemy nearby", OpponentsOfSelf.CountRareOrUniqueNearby <= 1 },
                { "while a rare or unique enemy is nearby", OpponentsOfSelf.CountRareOrUniqueNearby >= 1 },
                { "while there are at least two rare or unique enemies nearby", OpponentsOfSelf.CountRareOrUniqueNearby >= 2 },
                // buffs
                { "while you have ({BuffMatchers})", Reference.AsBuff.IsOn(Self) },
                { "while affected by ({SkillMatchers})", Reference.AsSkill.Buff.IsOn(Self) },
                { "while affected by a ({KeywordMatchers}) skill buff", Buffs(Self).With(Reference.AsKeyword).Any() },
                { "while affected by a non-vaal ({KeywordMatchers}) skill", Buffs(Self).Without(Keyword.Vaal).With(Reference.AsKeyword).Any() },
                { "while affected by a herald", Buffs(Self).With(Keyword.Herald).Any() },
                { "while you are affected by a herald", Buffs(Self).With(Keyword.Herald).Any() },
                { "during onslaught", Buff.Onslaught.IsOn(Self) },
                { "while phasing", Buff.Phasing.IsOn(Self) },
                { "while elusive", Buff.Elusive.IsOn(Self) },
                { "if you've ({BuffMatchers}) an enemy recently,?", Reference.AsBuff.InflictionAction.Recently },
                { "enemies you taunt( deal)?", And(For(MainOpponentOfSelf), Buff.Taunt.IsOn(Self, MainOpponentOfSelf)) },
                {
                    "enemies taunted by your warcries",
                    And(For(MainOpponentOfSelf),
                        Buff.Taunt.IsOn(Self, MainOpponentOfSelf),
                        Skills[Keyword.Warcry].Cast.InPastXSeconds(Buff.Taunt.Duration.Value))
                },
                { "enemies ({BuffMatchers}) by you", And(For(MainOpponentOfSelf), Reference.AsBuff.IsOn(Self, MainOpponentOfSelf)) },
                { "enemies you curse( have)?", And(For(MainOpponentOfSelf), Buffs(Self, MainOpponentOfSelf).With(Keyword.Curse).Any()) },
                { "({BuffMatchers}) enemies", And(For(MainOpponentOfSelf), Reference.AsBuff.IsOn(Self, MainOpponentOfSelf)) },
                { "you inflict on ({BuffMatchers}) enemies", And(For(MainOpponentOfSelf), Reference.AsBuff.IsOn(Self, MainOpponentOfSelf)) },
                { "(against|from) ({BuffMatchers}) enemies", Buff.Blind.IsOn(MainOpponentOfSelf) },
                { "(against|from) cursed enemies", Buffs(targets: MainOpponentOfSelf).With(Keyword.Curse).Any() },
                {
                    "you and allies affected by (your aura skills|auras from your skills) (have|deal)",
                    Or(For(Self), And(For(Ally), Buffs(targets: Ally).With(Keyword.Aura).Any()))
                },
                {
                    "you and allies affected by your placed banners",
                    Or(For(Self), And(For(Ally), Flag.IsBannerPlanted, Buffs(targets: Ally).With(Keyword.Banner).Any()))
                },
                // ailments
                { "while( you are)? ({AilmentMatchers})", Reference.AsAilment.IsOn(Self) },
                { "(against|from) ({AilmentMatchers}) enemies", Reference.AsAilment.IsOn(MainOpponentOfSelf) },
                {
                    "(against|from) ({AilmentMatchers}) or ({AilmentMatchers}) enemies",
                    Or(References[0].AsAilment.IsOn(MainOpponentOfSelf), References[1].AsAilment.IsOn(MainOpponentOfSelf))
                },
                {
                    "against frozen, shocked or ignited enemies",
                    Or(Ailment.Freeze.IsOn(MainOpponentOfSelf), Ailment.Shock.IsOn(MainOpponentOfSelf), Ailment.Ignite.IsOn(MainOpponentOfSelf))
                },
                { "which are ({AilmentMatchers})", Reference.AsAilment.IsOn(MainOpponentOfSelf) },
                { "you inflict on ({AilmentMatchers}) enemies", Reference.AsAilment.IsOn(MainOpponentOfSelf) },
                {
                    "against enemies( that are)? affected by elemental ailments",
                    Ailment.Elemental.Any(a => a.IsOn(MainOpponentOfSelf))
                },
                {
                    "against enemies( that are)? affected by no elemental ailments",
                    Not(Ailment.Elemental.Any(a => a.IsOn(MainOpponentOfSelf)))
                },
                { "enemies ({AilmentMatchers}) by (supported skills|you)( have)?", Reference.AsAilment.IsOn(MainOpponentOfSelf) },
                { "against enemies affected by ailments", Ailment.All.Any(a => a.IsOn(MainOpponentOfSelf)) },
                { "if you've been ({AilmentMatchers}) recently", Reference.AsAilment.InflictionAction.By(MainOpponentOfSelf).Recently },
                // ground effects
                { "while on consecrated ground", Ground.Consecrated.IsOn(Self) },
                // skills
                // - by keyword
                { "vaal( skill)?", With(Keyword.Vaal) },
                { "non-vaal skills deal", Not(With(Keyword.Vaal)) },
                { "bow skills (have|deal)", With(Keyword.Bow) },
                { "with bow skills", And(MainHand.Has(Tags.Bow), With(Keyword.Bow)) },
                { "spell skills (have|deal)", With(Keyword.Spell) },
                { "attack skills (have|deal)", With(Keyword.Attack) },
                { "non-curse aura skills have", And(With(Keyword.Aura), Not(With(Keyword.Curse))) },
                { "spells cast by totems (have|deal)", And(With(Keyword.Spell), With(Keyword.Totem)) },
                { "(with|of|for|from) ({KeywordMatchers}) skills", With(Reference.AsKeyword) },
                { "({DamageTypeMatchers}) skills (have|deal)", With(Reference.AsDamageType) },
                { "(supported )?({KeywordMatchers}) skills (have|deal)", With(Reference.AsKeyword) },
                { "(supported )?({KeywordMatchers}) spells (have|deal)", And(With(Reference.AsKeyword), With(Keyword.Spell)) },
                {
                    "({KeywordMatchers}) ({KeywordMatchers}) skills (have|deal)",
                    And(With(References[0].AsKeyword), With(References[1].AsKeyword))
                },
                {
                    "(with|of|for|from) ({KeywordMatchers}) ({KeywordMatchers}) skills",
                    And(With(References[0].AsKeyword), With(References[1].AsKeyword))
                },
                { "caused by melee hits", Condition.WithPart(Keyword.Melee) },
                { "with melee damage", Condition.WithPart(Keyword.Melee) },
                { "if triggered", Condition.WithPart(Keyword.Triggered) },
                // - by damage type
                { "with elemental skills", ElementalDamageTypes.Select(With).Aggregate((l, r) => l.Or(r)) },
                { "with ({DamageTypeMatchers}) skills", With(Reference.AsDamageType) },
                // - by single skill
                { "({SkillMatchers})('s)?", With(Reference.AsSkill) },
                { "({SkillMatchers}) (fires|has a|have a|has|deals|gain)", With(Reference.AsSkill) },
                { "dealt by ({SkillMatchers})", With(Reference.AsSkill) },
                {
                    "({SkillMatchers}) and ({SkillMatchers})",
                    Or(With(References[0].AsSkill), With(References[1].AsSkill))
                },
                {
                    "({SkillMatchers}) and ({SkillMatchers}) deal",
                    Or(With(References[0].AsSkill), With(References[1].AsSkill))
                },
                { "while you have an? ({SkillMatchers})", Reference.AsSkill.Instances.Value > 0 },
                {
                    "skills supported by (intensify|unleash) have",
                    Condition.True // the stats with this condition only affect anything when supported by the support anyway
                },
                // - cast recently/in past x seconds
                { "if you've used a ({KeywordMatchers}) skill recently", Skills[Reference.AsKeyword].Cast.Recently },
                { "if you haven't used a ({KeywordMatchers}) skill recently", Not(Skills[Reference.AsKeyword].Cast.Recently) },
                { "if you've cast a spell recently,?", Skills[Keyword.Spell].Cast.Recently },
                { "if you've attacked recently,?", Skills[Keyword.Attack].Cast.Recently },
                { "if (you've|you have) used a minion skill recently", Minions.Cast.Recently },
                { "if you've used a warcry recently", Skills[Keyword.Warcry].Cast.Recently },
                { "if you've warcried recently", Skills[Keyword.Warcry].Cast.Recently },
                { "when you warcry, for # seconds", Skills[Keyword.Warcry].Cast.InPastXSeconds(Value) },
                { @"if you('ve|\\u2019ve|’ve) cursed an enemy recently", Skills[Keyword.Curse].Cast.Recently },
                { "if you've warcried in the past # seconds", Skills[Keyword.Warcry].Cast.InPastXSeconds(Value) },
                {
                    "if you've used a ({DamageTypeMatchers}) skill in the past # seconds",
                    Skills[Reference.AsDamageType].Cast.InPastXSeconds(Value)
                },
                { "if you summoned a golem in the past # seconds", Golems.Cast.InPastXSeconds(Value) },
                // - by skill part
                {
                    "(beams?|final wave|shockwaves?|cone|aftershock|explosion) (has a|deals?|will have)",
                    Stat.MainSkillPart.Value.Eq(1)
                },
                { "if consuming a corpse", Stat.MainSkillPart.Value.Eq(1) }, // Bodyswap
                { "if using your life", Stat.MainSkillPart.Value.Eq(0) }, // Dark Pact
                { "shard", Stat.MainSkillPart.Value.Eq(1) }, // Spectral Shield Throw
                { "at maximum charge distance", Stat.MainSkillPart.Value.Eq(1) }, // Shield Charge
                // - socketed
                { "socketed (skills|gems)", Condition.MainSkillHasModifierSourceItemSlot },
                { "socketed (skills|gems) (deal|have)", Condition.MainSkillHasModifierSourceItemSlot },
                { "socketed attacks have", And(Condition.With(DamageSource.Attack), Condition.MainSkillHasModifierSourceItemSlot) },
                { "socketed spells have", And(Condition.With(DamageSource.Spell), Condition.MainSkillHasModifierSourceItemSlot) },
                // - other
                { "to enemies they're attached to", Flag.IsBrandAttachedToEnemy },
                { "to branded enemy", Flag.IsBrandAttachedToEnemy },
                {
                    "while you're in a blood bladestorm",
                    And(Flag.InBloodStance, Condition.Unique("Are you in a Bladestorm?"))
                },
                {
                    "sand bladestorms grant to you",
                    And(Flag.InSandStance, Condition.Unique("Are you in a Bladestorm?"))
                },
                {
                    "enemies maimed by this skill",
                    And(Condition.ModifierSourceIs(new ModifierSource.Local.Skill("BloodSandArmour")),
                        Buff.Maim.IsOn(MainOpponentOfSelf), Flag.InBloodStance, For(MainOpponentOfSelf))
                },
                // traps and mines
                { "with traps", With(Keyword.Trap) },
                { "skills used by traps have", With(Keyword.Trap) },
                { "with mines", With(Keyword.Mine) },
                { "skills used by mines (deal|have)", With(Keyword.Mine) },
                { "when used by mines", With(Keyword.Mine) },
                { "traps and mines (deal|have a)", Or(With(Keyword.Trap), With(Keyword.Mine)) },
                { "for throwing traps", With(Keyword.Trap) },
                { "if you detonated (mines|a mine) recently", Skills.DetonateMines.Cast.Recently },
                { "if you've placed a mine or thrown a trap recently", Or(Traps.Cast.Recently, Mines.Cast.Recently) },
                { "if you've thrown a trap or mine recently", Or(Traps.Cast.Recently, Mines.Cast.Recently) },
                // totems
                { "^totems'?", For(Entity.Totem) },
                { "totems (gain|have)", For(Entity.Totem) },
                { "you and your totems", Or(For(Self), For(Entity.Totem)) },
                { "(spells cast|attacks used|skills used) by totems (have a|have)", With(Keyword.Totem) },
                { "of totem skills that cast an aura", And(With(Keyword.Totem), With(Keyword.Aura)) },
                { "while you have a totem", Totems.CombinedInstances.Value > 0 },
                { "if you've summoned a totem recently", Totems.Cast.Recently },
                { "if you haven't summoned a totem in the past # seconds", Totems.Cast.InPastXSeconds(Value) },
                // minions
                { "minions", For(Entity.Minion) },
                { "minions (deal|have a|have|gain)", For(Entity.Minion) },
                { "supported skills have minion", For(Entity.Minion) },
                { "minions from supported skills deal", For(Entity.Minion) },
                { "you and your minions have", For(Entity.Minion).Or(For(Self)) },
                { "golems", And(For(Entity.Minion), With(Keyword.Golem)) },
                { "golems have", And(For(Entity.Minion), With(Keyword.Golem)) },
                { "summoned golems( are)?", And(For(Entity.Minion), With(Keyword.Golem)) },
                { "spectres (have|deal)", And(For(Entity.Minion), With(Skills.RaiseSpectre)) },
                { "raised zombies (have|deal)", And(For(Entity.Minion), With(Skills.RaiseZombie)) },
                { "(summoned )?skeletons (have|deal)", And(For(Entity.Minion), WithSkeletonSkills) },
                { 
                    "raised spectres, raised zombies, and summoned skeletons have",
                    And(For(Entity.Minion),
                        Or(With(Skills.RaiseSpectre), With(Skills.RaiseZombie), With(Skills.SummonSkeletons), With(Skills.VaalSummonSkeletons)))
                },
                { "while you have a summoned golem", Golems.CombinedInstances.Value > 0 },
                // flasks
                { "while using a flask", Equipment.IsAnyFlaskActive() },
                { "during any flask effect", Equipment.IsAnyFlaskActive() },
                { "while under no flask effects", Not(Equipment.IsAnyFlaskActive()) },
                {
                    "during effect of any mana flask",
                    Equipment.Flasks().Select(f => f.Has(Tags.ManaFlask)).Aggregate((l, r) => Or(l, r))
                },
                {
                    "during effect of any life or mana flask",
                    Equipment.Flasks()
                        .Select(f => Or(f.Has(Tags.LifeFlask), f.Has(Tags.ManaFlask)))
                        .Aggregate((l, r) => Or(l, r))
                },
                // - mods on flasks are only added when the flask item is enabled
                { "during (flask )?effect", Condition.True },
                // jewel thresholds
                {
                    "with( at least)? # ({AttributeStatMatchers}) in radius",
                    PassiveTree.TotalInModifierSourceJewelRadius(Reference.AsStat) >= Value
                },
                {
                    "with # total ({AttributeStatMatchers}) and ({AttributeStatMatchers}) in radius",
                    (PassiveTree.TotalInModifierSourceJewelRadius(References[0].AsStat)
                     + PassiveTree.TotalInModifierSourceJewelRadius(References[1].AsStat))
                    >= Value
                },
                // passive tree
                { "marauder:", PassiveTree.ConnectsToClass(CharacterClass.Marauder).IsTrue },
                { "duelist:", PassiveTree.ConnectsToClass(CharacterClass.Duelist).IsTrue },
                { "ranger:", PassiveTree.ConnectsToClass(CharacterClass.Ranger).IsTrue },
                { "shadow:", PassiveTree.ConnectsToClass(CharacterClass.Shadow).IsTrue },
                { "witch:", PassiveTree.ConnectsToClass(CharacterClass.Witch).IsTrue },
                { "templar:", PassiveTree.ConnectsToClass(CharacterClass.Templar).IsTrue },
                { "scion:", PassiveTree.ConnectsToClass(CharacterClass.Scion).IsTrue },
                // stance
                { "(while )?in blood stance", Flag.InBloodStance },
                { "(while )?in sand stance", Flag.InSandStance },
                { "if you(’|')ve changed stance recently", Condition.Unique("Stance.ChangedRecently") },
                // enemy
                { "enemies have", For(OpponentsOfSelf) },
                { "to normal or magic enemies", And(For(OpponentsOfSelf), Not(OpponentsOfSelf.IsRareOrUnique)) },
                { "to rare enemies", And(For(OpponentsOfSelf), Not(OpponentsOfSelf.IsRare)) },
                { "to unique enemies", And(For(OpponentsOfSelf), Not(OpponentsOfSelf.IsUnique)) },
                // other
                { "nearby allies( have| deal)?", For(Ally) },
                { "against targets they pierce", Projectile.PierceCount.Value >= 1 },
                { "while stationary", Flag.AlwaysStationary },
                { "while moving", Flag.AlwaysMoving },
                { "exerted attacks deal", And(Condition.With(DamageSource.Attack), Stat.Warcry.AttackAreExerted.IsTrue) },
                { "exerted attacks have", And(Condition.WithAttacks, Stat.Warcry.AttackAreExerted.IsTrue) },
                // unique
                { "against burning enemies", Or(Ailment.Ignite.IsOn(MainOpponentOfSelf), Condition.Unique("Is the Enemy Burning?")) },
                { "while( you are)? burning", Or(Ailment.Ignite.IsOn(Self), Condition.Unique("Are you Burning?")) },
                { "while leeching", Condition.Unique("Leech.IsActive") },
                {
                    "if you've killed an enemy affected by your damage over time recently",
                    Condition.Unique("Have you recently killed an Enemy affected by your Damage over Time?")
                },
                { "while you have energy shield", Condition.Unique("Do you have Energy Shield?") },
                {
                    "if you've taken fire damage from a hit recently",
                    Condition.Unique("Have you recently taken Fire Damage from a Hit?")
                },
                { "while you have at least one nearby ally", Condition.Unique("Is any ally nearby?") },
                { "while channelling", Condition.Unique("Are you currently channeling?") },
                { "if you've been channelling for at least 1 second", Condition.Unique("Have you been Channelling for at least 1 second?") },
                { "while you are not losing rage", Condition.Unique("Are you currently losing rage?") },
                { "during soul gain prevention", Condition.Unique("SoulGainPrevention") },
                { "to your deathmarked enemy", Condition.Unique("Is the enemy Deathmarked?") },
                {
                    "if you dealt a critical strike with a herald skill recently",
                    Condition.Unique("Did you deal a Critical Strike with a Herald Skill Recently?")
                },
                {
                    "you inflict with critical strikes",
                    Condition.Unique("Should modifiers requiring Ailments to be inflicted with critical strikes apply?")
                },
                { "if a non-vaal guard buff was lost recently", Condition.Unique("Have you lost a non-vaal guard buff recently?") },
                {
                    "for 6 seconds on melee hit with a mace, sceptre or staff",
                    And(Or(MainHand.Has(Tags.Mace), MainHand.Has(Tags.Sceptre), MainHand.Has(Tags.Staff),
                        OffHand.Has(Tags.Mace), OffHand.Has(Tags.Sceptre), OffHand.Has(Tags.Staff)),
                        Condition.Unique("Did you hit an enemy with a melee attack in the past 6 seconds?"))
                },
                { "if a warcry sacrificed rage recently", Condition.Unique("Warcry.SacrificedRageRecently") },
                // support gem mod clarifications. Irrelevant for parsing.
                {
                    "((a|for|with|from) )?supported (skill|spell|attack skill|attack)s?'?( (have|deal))?",
                    Condition.True
                },
                { "of supported curse skills", Condition.True },
            };
    }
}