using System.Collections.Generic;
using PoESkillTree.Engine.GameModel.PassiveTree;

namespace PoESkillTree.Engine.Computation.Parsing.JewelParsers
{
    public interface ITransformationJewelParser
    {
        bool IsTransformationJewelModifier(string jewelModifier);

        IEnumerable<TransformedNodeModifier> ApplyTransformation(
            string jewelModifier, IEnumerable<PassiveNodeDefinition> nodesInRadius);
    }
}