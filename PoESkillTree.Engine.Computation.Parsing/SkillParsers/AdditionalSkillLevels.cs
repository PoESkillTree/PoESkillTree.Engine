using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Skills;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    public class AdditionalSkillLevels : ValueObject
    {
        private readonly IReadOnlyDictionary<Skill, IValueBuilder> _valueBuilders;
        private readonly IReadOnlyDictionary<Skill, int> _values;

        public AdditionalSkillLevels(IReadOnlyDictionary<Skill, IValueBuilder> valueBuilders, IReadOnlyDictionary<Skill, int> values)
        {
            _valueBuilders = valueBuilders;
            _values = values;
        }

        public static AdditionalSkillLevels Calculate(
            IReadOnlyDictionary<Skill, IValueBuilder> valueBuilders, SkillDefinitions skillDefinitions, 
            Entity modifierSourceEntity, IValueCalculationContext context)
        {
            var values = valueBuilders.ToDictionary(
                p => p.Key,
                p => CalculateAdditionalLevel(skillDefinitions, modifierSourceEntity, context, p.Key, p.Value));
            return new AdditionalSkillLevels(valueBuilders, values);
        }

        private static int CalculateAdditionalLevel(
            SkillDefinitions skillDefinitions, Entity modifierSourceEntity, IValueCalculationContext context, Skill skill, IValueBuilder valueBuilder)
        {
            var gem = skill.Gem;
            if (gem is null)
                return 0;

            var displayName = skillDefinitions.GetSkillById(gem.SkillId).DisplayName;
            var gemModifierSource = new ModifierSource.Local.Gem(gem, displayName);
            var buildParameters = new BuildParameters(new ModifierSource.Global(gemModifierSource), modifierSourceEntity, default);
            return (int) (valueBuilder.Build(buildParameters).Calculate(context).SingleOrNull() ?? 0);
        }

        public IValueBuilder GetAdditionalLevelBuilder(Skill skill) => _valueBuilders[skill];

        public int GetAdditionalLevel(Skill skill) => _values[skill];

        protected override object ToTuple() => WithSequenceEquality(_values.ToList());
    }
}