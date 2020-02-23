using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Base;
using PoESkillTree.Engine.Computation.Data.Collections;

namespace PoESkillTree.Engine.Computation.Data
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="IStatMatchers"/> implementation matching stat parts specifying converters to the modifier's stats.
    /// </summary>
    public class StatManipulatorMatchers : StatMatchersBase
    {
        private readonly IModifierBuilder _modifierBuilder;

        public StatManipulatorMatchers(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories)
        {
            _modifierBuilder = modifierBuilder;
        }

        protected override IReadOnlyList<MatcherData> CreateCollection() =>
            new StatManipulatorMatcherCollection(_modifierBuilder)
            {
                { "you and nearby allies (deal|have)(?! onslaught)", s => Buff.Aura(s, Self, Ally) },
                { "you and nearby allies(?! deal| have)", s => Buff.Aura(s, Self, Ally) },
                { "you and nearby non-minion allies have a", s => Buff.Aura(s, Self, Entity.Totem) },
                {
                    "auras from your skills grant (?<inner>.*) to you and allies",
                    s => Buffs(Self, Self, Ally).With(Keyword.Aura).Without(Keyword.Curse).AddStat(s), "${inner}"
                },
                {
                    "consecrated ground( you create)? grants (?<inner>.*) to you and allies",
                    s => s.For(Self).WithCondition(Ground.Consecrated.IsOn(Self))
                        .Concat(s.For(Entity.Minion).WithCondition(Ground.Consecrated.IsOn(Entity.Minion)))
                        .Concat(s.For(Entity.Totem).WithCondition(Ground.Consecrated.IsOn(Entity.Totem))),
                    "${inner}"
                },
                {
                    "consecrated ground you create applies (?<inner>.*) to enemies",
                    s => s.For(OpponentOfSelf).WithCondition(Ground.Consecrated.IsOn(OpponentOfSelf)), "${inner}"
                },
                {
                    "every # seconds, gain (?<inner>.*) for # seconds",
                    s => Buff.Temporary(s), "${inner}"
                },
                { "nearby enemies (have|deal)", s => Buff.Aura(s, OpponentOfSelf) },
                { "nearby enemies(?= take)", s => Buff.Aura(s, OpponentOfSelf) },
                { "nearby chilled enemies deal", s => Buff.Aura(s, OpponentOfSelf).WithCondition(Ailment.Chill.IsOn(OpponentOfSelf)) },
                { "nearby hindered enemies deal", s => Buff.Aura(s, OpponentOfSelf).WithCondition(Buff.Hinder.IsOn(OpponentOfSelf)) },
                { "enemies near your totems (have|deal)", s => Buff.Aura(s, OpponentOfSelf).For(Entity.Totem) },
                { "enemies near your totems(?= take)", s => Buff.Aura(s, OpponentOfSelf).For(Entity.Totem) },
                { "each totem applies (?<inner>.*) to enemies near it", s => Buff.Aura(s, OpponentOfSelf).For(Entity.Totem), "${inner} for each totem" },
                { "({BuffMatchers}) grants", Reference.AsBuff.AddStat },
                { "hinder enemies with", Buff.Hinder.AddStat },
                { "during ({SkillMatchers}) for you and allies", Reference.AsSkill.Buff.AddStat },
                { "enemies ({AilmentMatchers}) by supported skills have", s => Reference.AsAilment.AddStat(s).For(OpponentOfSelf) },
                { "enemies ({BuffMatchers}) by supported skills(?= take)", s => Reference.AsBuff.AddStatForSource(s, Self).For(OpponentOfSelf) },
                { "elusive from supported skills also grants (?<inner>.*) for skills supported by nightblade", Buff.Elusive.AddStat, "${inner}" },
                { @"\(AsItemProperty\)", s => s.AsItemProperty },
                { @"\(AsPassiveNodeProperty\)", s => s.AsPassiveNodeProperty },
                { @"\(AsPassiveNodeBaseProperty\)", s => s.AsPassiveNodeBaseProperty },
            };
    }
}