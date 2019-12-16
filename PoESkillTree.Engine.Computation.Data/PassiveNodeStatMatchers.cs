using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Base;
using PoESkillTree.Engine.Computation.Data.Collections;
using PoESkillTree.Engine.GameModel.PassiveTree;

namespace PoESkillTree.Engine.Computation.Data
{
    /// <summary>
    /// <see cref="IStatMatchers"/> implementation that matches passive nodes (keystones and notables) by their name.
    /// </summary>
    public class PassiveNodeStatMatchers : StatMatchersBase
    {
        private readonly IModifierBuilder _modifierBuilder;
        private readonly IReadOnlyList<PassiveNodeDefinition> _passives;

        public PassiveNodeStatMatchers(
            IBuilderFactories builderFactories, IModifierBuilder modifierBuilder,
            IReadOnlyList<PassiveNodeDefinition> passives)
            : base(builderFactories)
            => (_modifierBuilder, _passives) = (modifierBuilder, passives);

        protected override IReadOnlyList<MatcherData> CreateCollection()
        {
            var collection = new FormAndStatMatcherCollection(_modifierBuilder, ValueFactory);
            foreach (var node in _passives.Where(d => d.Type == PassiveNodeType.Keystone || d.Type == PassiveNodeType.Notable))
            {
                collection.Add($"(you have )?{node.Name.ToLowerInvariant()}",
                    TotalOverride, 1, PassiveTree.NodeSkilled(node.Id));
            }
            return collection;
        }
    }
}