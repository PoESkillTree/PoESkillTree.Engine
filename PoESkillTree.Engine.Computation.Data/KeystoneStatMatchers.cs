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
    /// <see cref="IStatMatchers"/> implementation that matches keystones by their name.
    /// </summary>
    public class KeystoneStatMatchers : StatMatchersBase
    {
        private readonly IModifierBuilder _modifierBuilder;
        private readonly IReadOnlyList<PassiveNodeDefinition> _passives;

        public KeystoneStatMatchers(
            IBuilderFactories builderFactories, IModifierBuilder modifierBuilder,
            IReadOnlyList<PassiveNodeDefinition> passives)
            : base(builderFactories)
            => (_modifierBuilder, _passives) = (modifierBuilder, passives);

        protected override IReadOnlyList<MatcherData> CreateCollection()
        {
            var collection = new FormAndStatMatcherCollection(_modifierBuilder, ValueFactory);
            foreach (var keystone in _passives.Where(d => d.Type == PassiveNodeType.Keystone))
            {
                collection.Add($"(you have )?{keystone.Name.ToLowerInvariant()}",
                    TotalOverride, 1, PassiveTree.NodeSkilled(keystone.Id));
            }
            return collection;
        }
    }
}