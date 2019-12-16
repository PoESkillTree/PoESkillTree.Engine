using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Builders.Values;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Entities;
using PoESkillTree.Engine.Computation.Common.Builders.Resolving;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.Computation.Common.Parsing;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.PassiveTree;

namespace PoESkillTree.Engine.Computation.Builders.Stats
{
    public class PassiveTreeBuilders : StatBuildersBase, IPassiveTreeBuilders
    {
        private readonly PassiveTreeDefinition _tree;

        public PassiveTreeBuilders(IStatFactory statFactory, PassiveTreeDefinition tree) : base(statFactory)
        {
            _tree = tree;
        }

        public IStatBuilder NodeAllocated(ushort nodeId)
            => FromIdentity($"{nodeId}.Allocated", typeof(bool));

        public IStatBuilder NodeEffectiveness(ushort nodeId)
            => FromIdentity($"{nodeId}.Effectiveness", typeof(bool));

        public IStatBuilder NodeSkillPointSpent(ushort nodeId)
            => FromIdentity($"{nodeId}.SkillPointSpent", typeof(bool));

        public IStatBuilder ConnectsToClass(CharacterClass characterClass)
            => FromIdentity($"{characterClass}.TreeConnectedTo", typeof(bool));

        public ValueBuilder AllocatedNodeInModifierSourceJewelRadiusCount =>
            new ValueBuilder(new ValueBuilderImpl(
                ps => BuildInModifierSourceJewelRadiusValue(ps, _ => new ValueBuilder(new ValueBuilderImpl(1)), v => v),
                c => AllocatedNodeInModifierSourceJewelRadiusCount));

        public ValueBuilder TotalInModifierSourceJewelRadius(IStatBuilder stat)
            => new ValueBuilder(new ValueBuilderImpl(
                ps => BuildInModifierSourceJewelRadiusValue(ps, stat, _ => new Constant(true)),
                c => TotalInModifierSourceJewelRadius(stat.Resolve(c))));

        public ValueBuilder AllocatedInModifierSourceJewelRadius(IStatBuilder stat)
            => new ValueBuilder(new ValueBuilderImpl(
                ps => BuildInModifierSourceJewelRadiusValue(ps, stat, v => v),
                c => AllocatedInModifierSourceJewelRadius(stat.Resolve(c))));

        public ValueBuilder UnallocatedInModifierSourceJewelRadius(IStatBuilder stat)
            => new ValueBuilder(new ValueBuilderImpl(
                ps => BuildInModifierSourceJewelRadiusValue(ps, stat,
                    v => new ConditionalValue(c => !v.Calculate(c).IsTrue(), $"!{v}")),
                c => UnallocatedInModifierSourceJewelRadius(stat.Resolve(c))));

        private IValue BuildInModifierSourceJewelRadiusValue(
            BuildParameters parameters, IStatBuilder stat, Func<IValue, IValue> condition) =>
            BuildInModifierSourceJewelRadiusValue(parameters, d => stat.AsPassiveNodePropertyFor(d.Id).Value, condition);

        private IValue BuildInModifierSourceJewelRadiusValue(
            BuildParameters parameters, Func<PassiveNodeDefinition, ValueBuilder> value, Func<IValue, IValue> condition)
        {
            return GetNodesInRadius(parameters)
                .Select(GetValue)
                .Aggregate((l, r) => l + r)
                .Build(parameters);

            ValueBuilder GetValue(PassiveNodeDefinition d)
                => value(d).If(condition(NodeAllocated(d.Id).Value.Build(parameters)));
        }

        public IStatBuilder MultipliedAttributeForNodesInModifierSourceJewelRadius(
            IStatBuilder sourceAttribute, IStatBuilder targetAttribute)
            => new StatBuilder(StatFactory,
                new MultipliedAttributeForEachNodeInRadiusStatBuilder(GetNodesInRadius, sourceAttribute, targetAttribute));

        public IStatBuilder ModifyNodeEffectivenessInModifierSourceJewelRadius(
            bool onlyIfSkilled, params PassiveNodeType[] affectedNodeTypes)
        {
            return new StatBuilder(StatFactory,
                new FunctionalCompositeCoreStatBuilder(GetStatBuilders));

            IEnumerable<ICoreStatBuilder> GetStatBuilders(BuildParameters parameters)
                => GetNodesInRadius(parameters)
                    .Where(d => affectedNodeTypes.Contains(d.Type))
                    .Select(GetStatBuilder)
                    .Select(b => new StatBuilderAdapter(b));

            IStatBuilder GetStatBuilder(PassiveNodeDefinition node)
                => onlyIfSkilled
                    ? NodeEffectiveness(node.Id).WithCondition(NodeAllocated(node.Id).IsSet)
                    : NodeEffectiveness(node.Id);
        }

        public IStatBuilder ConnectJewelToNodesInModifierSourceJewelRadius
            => new StatBuilder(StatFactory,
                new FunctionalCompositeCoreStatBuilder(GetConnectionsToNodesInModifierSourceJewelRadius));

        private IEnumerable<ICoreStatBuilder> GetConnectionsToNodesInModifierSourceJewelRadius(
            BuildParameters parameters)
        {
            var jewelNodeId = GetJewelSource(parameters).PassiveNodeId;
            return GetNodesInRadius(parameters)
                .Select(d => FromIdentity($"{jewelNodeId}.ConnectsTo({d.Id})", typeof(bool), 
                    ExplicitRegistrationTypes.PassiveTreeConnection(jewelNodeId, d.Id)))
                .Select(b => new StatBuilderAdapter(b));
        }

        private IEnumerable<PassiveNodeDefinition> GetNodesInRadius(BuildParameters parameters)
        {
            var modifierSource = GetJewelSource(parameters);
            return _tree.GetNodesInRadius(modifierSource.PassiveNodeId, modifierSource.Radius);
        }

        private static ModifierSource.Local.Jewel GetJewelSource(BuildParameters parameters)
        {
            ModifierSource? modifierSource = parameters.ModifierSource;
            if (modifierSource is ModifierSource.Global globalSource)
                modifierSource = globalSource.LocalSource;
            if (modifierSource is ModifierSource.Local.Jewel jewelSource)
                return jewelSource;
            throw new ParseException(
                "IPassiveTreeBuilders.*InModifierSourceJewelRadius can only be used with a source of type ModifierSource.Local.Jewel");
        }

        private class FunctionalCompositeCoreStatBuilder : ICoreStatBuilder
        {
            private readonly Func<BuildParameters, IEnumerable<ICoreStatBuilder>> _getComponents;

            public FunctionalCompositeCoreStatBuilder(Func<BuildParameters, IEnumerable<ICoreStatBuilder>> getComponents)
            {
                _getComponents = getComponents;
            }

            public ICoreStatBuilder Resolve(ResolveContext context)
                => new FunctionalCompositeCoreStatBuilder(ps => _getComponents(ps).Select(b => b.Resolve(context)));

            public ICoreStatBuilder WithEntity(IEntityBuilder entityBuilder)
                => new FunctionalCompositeCoreStatBuilder(
                    ps => _getComponents(ps).Select(b => b.WithEntity(entityBuilder)));

            public IEnumerable<StatBuilderResult> Build(BuildParameters parameters)
                => _getComponents(parameters).SelectMany(b => b.Build(parameters));
        }
    }
}