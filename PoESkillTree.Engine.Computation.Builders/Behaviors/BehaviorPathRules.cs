using System.Linq;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Utils;
using PoESkillTree.Engine.Utils.Extensions;

namespace PoESkillTree.Engine.Computation.Builders.Behaviors
{
    public static class BehaviorPathRules
    {
        public static IBehaviorPathRule All => new AllPathsRule();

        public static IBehaviorPathRule NonConversion => new NonConversionPathsRule();

        public static IBehaviorPathRule ConversionWithSpecificSource(IStat conversionSource)
            => new ConversionPathsWithSpecificSourceRule(conversionSource);
    }

    internal class AllPathsRule : SimpleBehaviorPathRule
    {
        public override bool AffectsPath(PathDefinition path)
            => true;
    }

    internal class NonConversionPathsRule : SimpleBehaviorPathRule
    {
        public override bool AffectsPath(PathDefinition path)
            => path.ConversionStats.IsEmpty();
    }

    internal abstract class SimpleBehaviorPathRule : ValueObject, IBehaviorPathRule
    {
        public abstract bool AffectsPath(PathDefinition path);

        protected override object ToTuple()
            => "";
    }

    internal class ConversionPathsWithSpecificSourceRule : ValueObject, IBehaviorPathRule
    {
        private readonly IStat _conversionSource;

        public ConversionPathsWithSpecificSourceRule(IStat conversionSource)
            => _conversionSource = conversionSource;

        public bool AffectsPath(PathDefinition path)
            => _conversionSource.Equals(path.ConversionStats.FirstOrDefault());

        protected override object ToTuple()
            => _conversionSource;
    }
}