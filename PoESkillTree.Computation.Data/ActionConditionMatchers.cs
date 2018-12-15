﻿using System.Collections.Generic;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Modifiers;
using PoESkillTree.Computation.Common.Builders.Resolving;
using PoESkillTree.Computation.Common.Data;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;

namespace PoESkillTree.Computation.Data
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

        public ActionConditionMatchers(
            IBuilderFactories builderFactories, IMatchContexts matchContexts, IModifierBuilder modifierBuilder)
            : base(builderFactories, matchContexts)
        {
            _modifierBuilder = modifierBuilder;
        }

        protected override IEnumerable<MatcherData> CreateCollection() =>
            new ConditionMatcherCollection(_modifierBuilder)
            {
                // generic
                { "on ({ActionMatchers})", Reference.AsAction.On },
                {
                    "on ({KeywordMatchers}) ({ActionMatchers})",
                    And(Condition.WithPart(References[0].AsKeyword), References[1].AsAction.On)
                },
                { "when you ({ActionMatchers}) an enemy", Reference.AsAction.On },
                {
                    "when you ({ActionMatchers}) a rare or unique enemy",
                    And(Enemy.IsRareOrUnique, Reference.AsAction.On)
                },
                {
                    "when projectile ({ActionMatchers}) a rare or unique enemy",
                    And(Enemy.IsRareOrUnique, Condition.WithPart(Keyword.Projectile), Reference.AsAction.On)
                },
                {
                    "(when you|on) ({ActionMatchers}) a ({AilmentMatchers}) enemy",
                    And(References[1].AsAilment.IsOn(Enemy), References[0].AsAction.On)
                },
                // kill
                { "if you or your totems kill an enemy", Or(Kill.On, Kill.By(Entity.Totem).On) },
                // hit
                { "when hit", Hit.By(Enemy).On },
                { "when you are hit", Hit.By(Enemy).On },
                { "with hits", Hit.On },
                { "for each enemy hit by (your )?attacks", And(With(Keyword.Attack), Hit.On) },
                {
                    "when you or your totems hit an enemy with a spell",
                    And(With(Keyword.Spell), Hit.On.Or(Hit.By(Entity.Totem).On))
                },
                // critical strike
                { "critical strikes have a", CriticalStrike.On },
                { "when you deal a critical strike", CriticalStrike.On },
                { "if you get a critical strike", CriticalStrike.On },
                // skill cast
                { "when you place a totem", Totems.Cast.On },
                { "when you summon a totem", Totems.Cast.On },
                { "when you use a warcry", Skills[Keyword.Warcry].Cast.On },
                // block
                { "when they block", Block.On },
                { "when you block", Block.On },
                // other
                { "when you stun an enemy", Action.Stun.On },
                { "when you stun an enemy with a melee hit", And(Condition.WithPart(Keyword.Melee), Action.Stun.On) },
                { "after spending( a total of)? # mana", Action.SpendMana(Value).On },
                { "when you gain a ({ChargeTypeMatchers})", Reference.AsChargeType.GainAction.On },
                { "you gain", Condition.True }, // may be left over at the end, does nothing
                { "you", Condition.True },
                { "grants", Condition.True },
                // unique
                {
                    "when your trap is triggered by an enemy",
                    Action.Unique("When your Trap is triggered by an Enemy").On
                },
                {
                    "when your mine is detonated targeting an enemy",
                    Action.Unique("When your Mine is detonated targeting an Enemy").On
                },
            }; // add
    }
}