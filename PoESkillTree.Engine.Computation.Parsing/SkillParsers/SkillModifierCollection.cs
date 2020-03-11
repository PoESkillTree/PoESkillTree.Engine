using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class SkillModifierCollection : ModifierCollection
    {
        private readonly IConditionBuilder _isMainSkill;

        public SkillModifierCollection(
            IBuilderFactories builderFactories, IConditionBuilder isMainSkill, ModifierSource.Local localModifierSource,
            Entity modifierSourceEntity)
            : base(builderFactories, localModifierSource, modifierSourceEntity)
            => _isMainSkill = isMainSkill;

        public void AddLocalForMainSkill(
            IStatBuilder stat, Form form, double value, IConditionBuilder? condition = null)
            => AddLocal(stat, form, value, CombineWithNullableCondition(_isMainSkill, condition));

        public void AddGlobalForMainSkill(
            IStatBuilder stat, Form form, double value, IConditionBuilder? condition = null)
            => AddGlobal(stat, form, value, CombineWithNullableCondition(_isMainSkill, condition));

        public void AddGlobalForMainSkill(
            IStatBuilder stat, Form form, IValueBuilder value, IConditionBuilder? condition = null)
            => AddGlobal(stat, form, value, CombineWithNullableCondition(_isMainSkill, condition));

        private static IConditionBuilder CombineWithNullableCondition(
            IConditionBuilder left, IConditionBuilder? right = null)
            => right is null ? left : left.And(right);
    }
}