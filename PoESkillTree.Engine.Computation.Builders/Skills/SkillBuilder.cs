using System;
using System.Runtime.CompilerServices;
using PoESkillTree.Engine.Computation.Builders.Actions;
using PoESkillTree.Engine.Computation.Builders.Buffs;
using PoESkillTree.Engine.Computation.Builders.Entities;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Builders.Values;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Actions;
using PoESkillTree.Engine.Computation.Common.Builders.Buffs;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Skills;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Builders.Skills
{
    public class SkillBuilder : ISkillBuilder
    {
        private readonly IStatFactory _statFactory;
        private readonly ICoreBuilder<SkillDefinition> _coreBuilder;

        public SkillBuilder(IStatFactory statFactory, ICoreBuilder<SkillDefinition> coreBuilder)
        {
            _statFactory = statFactory;
            _coreBuilder = coreBuilder;
        }

        public ISkillBuilder Resolve(ResolveContext context) =>
            new SkillBuilder(_statFactory, _coreBuilder.Resolve(context));

        public IActionBuilder Cast =>
            new ActionBuilder(_statFactory, CoreBuilder.UnaryOperation(_coreBuilder, d => $"{d.Id}.Cast"),
                new ModifierSourceEntityBuilder());

        public IStatBuilder Instances => CreateStatBuilder(typeof(uint));

        public IStatBuilder Reservation => CreateStatBuilder(typeof(uint));

        public IStatBuilder ReservationPool => CreateStatBuilder(typeof(Pool));

        private IStatBuilder CreateStatBuilder(Type dataType, [CallerMemberName] string identitySuffix = null)
            => new StatBuilder(_statFactory, new CoreStatBuilderFromCoreBuilder<SkillDefinition>(_coreBuilder,
                (e, d) => _statFactory.FromIdentity($"{d.Id}.{identitySuffix}", e, dataType)));

        public ValueBuilder SkillId =>
            new ValueBuilder(new ValueBuilderImpl(
                ps => new Constant(_coreBuilder.Build(ps).NumericId),
                c => Resolve(c).SkillId));

        public IBuffBuilder Buff
            => new BuffBuilder(_statFactory, CoreBuilder.UnaryOperation(_coreBuilder, SelectBuffIdentity));

        private static string SelectBuffIdentity(SkillDefinition skillDefinition)
        {
            if (skillDefinition.IsSupport || !skillDefinition.ActiveSkill.ProvidesBuff)
                return "dummy";
            return skillDefinition.Id;
        }

        public SkillDefinition Build(BuildParameters parameters) => _coreBuilder.Build(parameters);
    }
}