using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Base;
using PoESkillTree.Engine.Computation.Data.Collections;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Data.GivenStats
{
    public abstract class LevelBasedStats : UsesConditionBuilders, IGivenStats
    {
        protected IModifierBuilder ModifierBuilder { get; }
        protected MonsterBaseStats MonsterBaseStats { get; }
        private readonly Lazy<IReadOnlyList<IIntermediateModifier>> _lazyGivenStats;

        protected LevelBasedStats(
            IBuilderFactories builderFactories, IModifierBuilder modifierBuilder, MonsterBaseStats monsterBaseStats)
            : base(builderFactories)
        {
            ModifierBuilder = modifierBuilder;
            MonsterBaseStats = monsterBaseStats;
            _lazyGivenStats = new Lazy<IReadOnlyList<IIntermediateModifier>>(() => CreateCollection().ToList());
        }

        public abstract IReadOnlyList<Entity> AffectedEntities { get; }

        public IReadOnlyList<string> GivenStatLines { get; } = new string[0];

        public IReadOnlyList<IIntermediateModifier> GivenModifiers => _lazyGivenStats.Value;

        protected abstract GivenStatCollection CreateCollection();

        protected ValueBuilder LevelBased(Func<int, double> selector, string identity)
            => Stat.Level.Value.Select(v => selector((int) v), v => $"{identity}(level: {v})");
    }
}