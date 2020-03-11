using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Data.Collections;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Data.GivenStats
{
    public class EnemyMechanics : DataDrivenMechanicsBase
    {
        private readonly Lazy<IReadOnlyList<IIntermediateModifier>> _lazyGivenStats;

        public EnemyMechanics(IBuilderFactories builderFactories, IModifierBuilder modifierBuilder)
            : base(builderFactories, modifierBuilder)
        {
            _lazyGivenStats = new Lazy<IReadOnlyList<IIntermediateModifier>>(
                () => CreateCollection().ToList());
        }

        public override IReadOnlyList<Entity> AffectedEntities { get; } = new[] {GameModel.Entity.Enemy};

        public override IReadOnlyList<IIntermediateModifier> GivenModifiers => _lazyGivenStats.Value;

        private DataDrivenMechanicCollection CreateCollection()
            => new DataDrivenMechanicCollection(ModifierBuilder, BuilderFactories)
            {
                // stun
                { PercentMore, Effect.Stun.Duration, 100 / Effect.Stun.Recovery.For(Entity.Character).Value - 100 },
            };
    }
}