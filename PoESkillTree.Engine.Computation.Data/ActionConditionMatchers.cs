using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Base;
using PoESkillTree.Engine.Computation.Data.Collections;
using PoESkillTree.Engine.GameModel.Items;

namespace PoESkillTree.Engine.Computation.Data
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="IStatMatchers"/> implementation matching stat parts specifying
    /// <see cref="Common.Builders.Actions.IActionBuilder.On"/> conditions.
    /// These must be applied after all other conditions.
    /// </summary>
    public class ActionConditionMatchers : StatMatchersBase
    {
        private readonly IModifierBuilder _modifierBuilder;

        public ActionConditionMatchers(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories)
        {
            _modifierBuilder = modifierBuilder;
        }

        protected override IReadOnlyList<MatcherData> CreateCollection() =>
            new ConditionMatcherCollection(_modifierBuilder)
            {
                // generic
                { "on ({ActionMatchers})", Reference.AsAction.On },
                {
                    "on ({KeywordMatchers}) ({ActionMatchers})",
                    And(Condition.WithPart(References[0].AsKeyword), References[1].AsAction.On)
                },
                { "when you ({ActionMatchers}) an enemy", Reference.AsAction.On },
                { "when you ({ActionMatchers}) a rare enemy", And(OpponentsOfSelf.IsRare, Reference.AsAction.On) },
                { "when you ({ActionMatchers}) a unique enemy", And(OpponentsOfSelf.IsUnique, Reference.AsAction.On) },
                {
                    "when you ({ActionMatchers}) a rare or unique enemy",
                    And(OpponentsOfSelf.IsRareOrUnique, Reference.AsAction.On)
                },
                {
                    "when projectile ({ActionMatchers}) a rare or unique enemy",
                    And(OpponentsOfSelf.IsRareOrUnique, Condition.WithPart(Keyword.Projectile), Reference.AsAction.On)
                },
                {
                    "(when you|on) ({ActionMatchers}) a ({AilmentMatchers}) enemy",
                    And(References[1].AsAilment.IsOn(MainOpponentOfSelf), References[0].AsAction.On)
                },
                {
                    "(when you|on) ({ActionMatchers}) a cursed enemy",
                    And(Buffs(targets: OpponentsOfSelf).With(Keyword.Curse).Any(), References[0].AsAction.On)
                },
                // kill
                { "if you or your totems kill an enemy", Or(Kill.On, Kill.By(Entity.Totem).On) },
                { "affecting enemies you kill", Kill.On },
                // hit
                { "when hit", Hit.By(OpponentsOfSelf).On },
                { "when you are hit( by an enemy)?", Hit.By(OpponentsOfSelf).On },
                { "with hits", Hit.On },
                { "for each enemy hit by (your )?attacks", And(With(Keyword.Attack), Hit.On) },
                { "for each enemy hit by (your )?spells", And(With(Keyword.Spell), Hit.On) },
                {
                    "when you or your totems hit an enemy with a spell",
                    And(With(Keyword.Spell), Hit.On.Or(Hit.By(Entity.Totem).On))
                },
                {
                    "for each blinded enemy hit by this weapon",
                    (And(ModifierSourceIs(ItemSlot.MainHand), MainHandAttack, Hit.On, Buff.Blind.IsOn(MainOpponentOfSelf)),
                        And(ModifierSourceIs(ItemSlot.OffHand), OffHandAttack, Hit.On, Buff.Blind.IsOn(MainOpponentOfSelf)))
                },
                { "on hit,? no more than once every (second|# seconds)", Hit.On },
                { "on melee hit, no more than once every # seconds", And(Condition.WithPart(Keyword.Melee), Hit.On) },
                { "on hit with spell damage", And(Condition.With(DamageSource.Spell), Hit.On) },
                // critical strike
                { "critical strikes have a", CriticalStrike.On },
                { "(when you deal|if you get) a critical strike", CriticalStrike.On },
                { "your critical strikes", CriticalStrike.On },
                { "when you take a critical strike", CriticalStrike.By(OpponentsOfSelf).On },
                // skill cast
                { "when you place a totem", Totems.Cast.On },
                { "when you summon a totem", Totems.Cast.On },
                { "when summoned", Totems.Cast.On },
                { "when you( use a)? warcry", Skills[Keyword.Warcry].Cast.On },
                { "when you use a skill", Skills.AllSkills.Cast.On },
                { "when you use a ({DamageTypeMatchers}) skill", Skills[Reference.AsDamageType].Cast.On },
                {
                    "when you use an elemental skill",
                    ElementalDamageTypes.Select(dt => Skills[dt].Cast.On).Aggregate((l, r) => l.Or(r))
                },
                // block
                { "when they block", Block.On },
                { "when you block", Block.On },
                { "when you block attack damage", Block.Attack.On },
                { "when you block spell damage", Block.Spell.On },
                // buffs
                { "when you ({BuffMatchers}) an enemy", Reference.AsBuff.InflictionAction.On },
                // stun
                {
                    "when you stun an enemy with a melee hit",
                    And(Condition.WithPart(Keyword.Melee), Effect.Stun.InflictionAction.On)
                },
                { "when you stun", Effect.Stun.InflictionAction.On },
                // other
                { "after spending( a total of)? # mana", Action.SpendMana(Value).On },
                { "when you spend mana", Action.SpendMana(ValueFactory.Create(1)).On },
                { "when you focus", Action.Focus.On },
                { "when you gain a ({ChargeTypeMatchers})", Reference.AsChargeType.GainAction.On },
                { "every # seconds?", Action.EveryXSeconds(Values[0]).On },
                { "(every|each) second", Action.EveryXSeconds(ValueFactory.Create(1)).On },
                // leftover, meaningless words
                { "you gain", Condition.True },
                { "you", Condition.True },
                { "grants", Condition.True },
                // unique
                {
                    "when your trap is triggered by an enemy",
                    Action.Unique("When a Trap is triggered by an Enemy").On
                },
                {
                    "when (your|a) mine is detonated targeting an enemy",
                    Action.Unique("When a Mine is detonated targeting an Enemy").On
                },
                { "on use", Action.Unique("When you use the Flask").On },
                { "when you use a flask", Action.Unique("When you use any Flask").On },
                { "when you use a life flask", Action.Unique("When you use any Life Flask").On },
                { "if this skill hits any enemies", Action.Unique("When your active skill hits any enemies").On },
                { "when you gain Adrenaline", Action.Unique("When you gain Adrenaline").On },
                { "after channelling for # seconds?", Action.Unique("PeriodOfChannelling").On },
                { "every second, consume a nearby corpse to", Action.Unique("When you consume a corpse, every second").On },
            }; // add
    }
}