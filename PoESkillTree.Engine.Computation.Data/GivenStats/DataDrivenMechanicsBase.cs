using System;
using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Base;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Data.GivenStats
{
    public abstract class DataDrivenMechanicsBase : UsesConditionBuilders, IGivenStats
    {
        protected DataDrivenMechanicsBase(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories)
        {
            ModifierBuilder = modifierBuilder;
        }

        protected IModifierBuilder ModifierBuilder { get; }
        protected IMetaStatBuilders MetaStats => BuilderFactories.MetaStatBuilders;

        public abstract IReadOnlyList<Entity> AffectedEntities { get; }

        public IReadOnlyList<string> GivenStatLines { get; } = Array.Empty<string>();

        public abstract IReadOnlyList<IIntermediateModifier> GivenModifiers { get; }

        protected static ValueBuilder DamageTakenMultiplier(IStatBuilder resistance, IStatBuilder damageReduction, IStatBuilder damageTaken)
            => (1 - resistance.Value.AsPercentage) * (1 - damageReduction.Value.AsPercentage) * damageTaken.Value;

        protected IDamageRelatedStatBuilder DamageTaken(DamageType damageType)
            => DamageTypeBuilders.From(damageType).Damage.Taken;

        protected ValueBuilder PhysicalDamageReductionFromArmour(IStatBuilder armour, ValueBuilder physicalDamage)
        {
            var armourValue = ValueFactory.If(armour.ChanceToDouble.Value >= 100)
                .Then(2 * armour.Value)
                .Else(armour.Value);
            return 100 * ValueFactory.If(armourValue.Eq(0))
                .Then(0)
                .Else(armourValue / (armourValue + 10 * physicalDamage.Average));
        }

        protected ValueBuilder ChanceToHitValue(
            IStatBuilder accuracyStat, IStatBuilder evasionStat, IConditionBuilder isBlinded)
        {
            var accuracy = accuracyStat.Value;
            var evasion = evasionStat.Value;
            var blindMultiplier = ValueFactory.If(isBlinded).Then(0.5).Else(1);
            return 100 * blindMultiplier * 1.15 * accuracy /
                   (accuracy + (evasion / 4).Select(d => Math.Pow(d, 0.8), v => $"{v}^0.8"));
        }
    }
}