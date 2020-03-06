using System;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Skills;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Parsing;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Builders.Conditions
{
    public class ConditionBuilders : IConditionBuilders
    {
        private readonly IStatFactory _statFactory;

        public ConditionBuilders(IStatFactory statFactory) => _statFactory = statFactory;

        public IConditionBuilder With(IKeywordBuilder keyword) => ValueConditionBuilder.Create(
            (ps, k) => _statFactory.MainSkillHasKeyword(ps.ModifierSourceEntity, k.Build(ps)), keyword);

        public IConditionBuilder WithPart(IKeywordBuilder keyword) => ValueConditionBuilder.Create(
            (ps, k) => _statFactory.MainSkillPartHasKeyword(ps.ModifierSourceEntity, k.Build(ps)), keyword);

        public IConditionBuilder With(ISkillBuilder skill)
        {
            var activeSkillIdStat = new StatBuilder(_statFactory, new LeafCoreStatBuilder(_statFactory.MainSkillId));
            return new StatConvertingConditionBuilder(IfIsDamageStat(d => d.WithSkills))
                .And(activeSkillIdStat.Value.Eq(skill.SkillId));
        }

        public IConditionBuilder AttackWith(AttackDamageHand hand) =>
            With(DamageSource.Attack)
                .And(new StatConvertingConditionBuilder(IfIsDamageStat(d => d.With(hand))));

        public IConditionBuilder With(DamageSource damageSource) =>
            new StatConvertingConditionBuilder(IfIsDamageStat(
                d => d.WithSkills(damageSource),
                _ => throw new ParseException(
                    $"IConditionBuilders.{nameof(With)} only works with damage related stats")));

        private static StatConverter IfIsDamageStat(Func<IDamageRelatedStatBuilder, IStatBuilder> then) =>
            IfIsDamageStat(then, Funcs.Identity);

        private static StatConverter IfIsDamageStat(
            Func<IDamageRelatedStatBuilder, IStatBuilder> then, Func<IStatBuilder, IStatBuilder> otherwise) =>
            s => s is IDamageRelatedStatBuilder d ? then(d) : otherwise(s);

        public IConditionBuilder DamageTaken =>
            new StatConvertingConditionBuilder(s => s is IDamageStatBuilder d
                ? d.Taken
                : throw new ParseException($"IConditionBuilders.{nameof(DamageTaken)} only works with damage stats"));

        public IConditionBuilder For(IEntityBuilder entity) =>
            new StatConvertingConditionBuilder(s => s.For(entity));

        public IConditionBuilder ModifierSourceIs(ModifierSource modifierSource)
            => new ValueConditionBuilder(
                ps => new Constant(ps.ModifierSource.CanonicalSource.Equals(modifierSource.CanonicalSource)));

        public IConditionBuilder MainSkillHasModifierSourceItemSlot => new ValueConditionBuilder(BuildMainSkillHasModifierSourceItemSlot);

        private IValue BuildMainSkillHasModifierSourceItemSlot(BuildParameters parameters)
        {
            var mainSkillItemSlotValue = new StatValue(_statFactory.MainSkillItemSlot(parameters.ModifierSourceEntity));
            var modifierSourceItemSlot = GetItemSlot(parameters.ModifierSource);
            return new ConditionalValue(c => mainSkillItemSlotValue.Calculate(c) == (NodeValue) (int) modifierSourceItemSlot,
                $"{mainSkillItemSlotValue} == {modifierSourceItemSlot}");
        }

        private static ItemSlot GetItemSlot(ModifierSource source) =>
            source.GetLocalSource() is ModifierSource.Local.Item itemSource
                ? itemSource.Slot
                : throw new ParseException($"ModifierSource must be Item, {source} given");

        public IConditionBuilder BaseValueComesFrom(ItemSlot slot)
        {
            var modifierSource = new ModifierSource.Local.Item(slot);
            return new StatConvertingConditionBuilder(ConvertStat);

            IStatBuilder ConvertStat(IStatBuilder stat) =>
                new StatBuilder(_statFactory,
                    new StatBuilderWithModifierSource(new StatBuilderAdapter(stat), modifierSource));
        }

        public IConditionBuilder Unique(string name)
            => StatBuilderUtils.ConditionFromIdentity(_statFactory, name,
                ExplicitRegistrationTypes.UserSpecifiedValue(false));

        public IConditionBuilder True => ConstantConditionBuilder.True;
    }
}