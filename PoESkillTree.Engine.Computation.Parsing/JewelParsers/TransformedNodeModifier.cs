using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders.Values;

namespace PoESkillTree.Engine.Computation.Parsing.JewelParsers
{
    public class TransformedNodeModifier
    {
        public TransformedNodeModifier(string modifier, IValueBuilder valueMultiplier)
        {
            Modifier = modifier;
            ValueMultiplier = valueMultiplier;
        }

        public string Modifier { get; }

        public IValueBuilder ValueMultiplier { get; }
    }
}