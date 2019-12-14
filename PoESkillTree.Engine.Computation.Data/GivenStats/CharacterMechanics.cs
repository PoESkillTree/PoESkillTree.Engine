using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Data.Collections;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Data.GivenStats
{
    public class CharacterMechanics : DataDrivenMechanicsBase
    {
        private readonly Lazy<IReadOnlyList<IIntermediateModifier>> _lazyGivenStats;

        public CharacterMechanics(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories, modifierBuilder)
        {
            _lazyGivenStats = new Lazy<IReadOnlyList<IIntermediateModifier>>(
                () => CreateCollection().ToList());
        }

        public override IReadOnlyList<Entity> AffectedEntities { get; } = new[] {GameModel.Entity.Character};

        public override IReadOnlyList<IIntermediateModifier> GivenModifiers => _lazyGivenStats.Value;

        private DataDrivenMechanicCollection CreateCollection()
            => new DataDrivenMechanicCollection(ModifierBuilder, BuilderFactories)
            {
                // flasks
                { PercentMore, Flask.LifeRecovery, Flask.Effect.Value * 100 },
                { PercentMore, Flask.ManaRecovery, Flask.Effect.Value * 100 },
                { PercentMore, Flask.LifeRecovery, Flask.LifeRecoverySpeed.Value * 100 },
                { PercentMore, Flask.ManaRecovery, Flask.ManaRecoverySpeed.Value * 100 },
            };
    }
}