using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PoESkillTree.Engine.Computation.Builders.Conditions;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Builders.Values;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Builders.Entities
{
    public class ModifierSourceOpponentEntityBuilder : IHostileEntityBuilder
    {
        private readonly IStatFactory _statFactory;

        public ModifierSourceOpponentEntityBuilder(IStatFactory statFactory) =>
            _statFactory = statFactory;

        public IReadOnlyCollection<Entity> Build(Entity modifierSourceEntity) => modifierSourceEntity.Opponents().ToList();

        public ValueBuilder CountNearby => CreateValue();
        public IConditionBuilder IsNearby => CreateCondition();
        public ValueBuilder CountRareOrUniqueNearby => CreateValue();
        public ValueBuilder Distance => CreateValue();
        public IConditionBuilder IsRare => CreateCondition();
        public IConditionBuilder IsUnique => CreateCondition();
        public IConditionBuilder IsRareOrUnique => IsRare.Or(IsUnique);
        public IConditionBuilder IsMoving => CreateCondition();

        private ValueBuilder CreateValue([CallerMemberName] string identity = "") =>
            new ValueBuilder(new ValueBuilderImpl(ps => BuildValue(ps, identity), _ => ps => BuildValue(ps, identity)));

        private IValue BuildValue(BuildParameters ps, string identity)
        {
            if (ps.ModifierSourceEntity == Entity.Enemy)
            {
                return BuildValue(ps, $"Opponent.{identity}", typeof(uint), 0);
            }
            else
            {
                ps = new BuildParameters(ps.ModifierSource, Entity.Enemy, ps.ModifierForm);
                return BuildValue(ps, identity, typeof(uint), 0);
            }
        }

        private IValue BuildValue(BuildParameters ps, string identity, Type dataType, double defaultValue) =>
            StatBuilderUtils.FromIdentity(_statFactory, identity, dataType, ExplicitRegistrationTypes.UserSpecifiedValue(defaultValue))
                .Value.Build(ps);

        private IConditionBuilder CreateCondition([CallerMemberName] string identity = "") =>
            new ValueConditionBuilder(ps => BuildConditionalValue(ps, identity));

        private IValue BuildConditionalValue(BuildParameters ps, string identity)
        {
            if (ps.ModifierSourceEntity == Entity.Enemy)
            {
                return BuildCondition(ps, $"Opponent.{identity}").Value;
            }
            else
            {
                ps = new BuildParameters(ps.ModifierSource, Entity.Enemy, ps.ModifierForm);
                return BuildCondition(ps, identity).Value;
            }
        }

        private ConditionBuilderResult BuildCondition(BuildParameters ps, string identity) =>
            StatBuilderUtils.ConditionFromIdentity(_statFactory, identity, ExplicitRegistrationTypes.UserSpecifiedValue(false))
                .Build(ps);
    }
}