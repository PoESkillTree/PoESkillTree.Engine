using System.Collections.Generic;

namespace PoESkillTree.Engine.GameModel.PassiveTree
{
    public class PassiveNodeDefinition : IDefinition<ushort>
    {
        public PassiveNodeDefinition(
            ushort id, PassiveNodeType type, string name, bool isAscendancyNode, bool costsPassivePoint,
            NodePosition position, IReadOnlyList<string> modifiers)
            => (Id, Type, Name, IsAscendancyNode, CostsPassivePoint, Position, Modifiers) =
                (id, type, name, isAscendancyNode, costsPassivePoint, position, modifiers);

        public ushort Id { get; }

        public PassiveNodeType Type { get; }
        public string Name { get; }
        public bool IsAscendancyNode { get; }
        public bool CostsPassivePoint { get; }

        public NodePosition Position { get; }

        public IReadOnlyList<string> Modifiers { get; }
    }
}