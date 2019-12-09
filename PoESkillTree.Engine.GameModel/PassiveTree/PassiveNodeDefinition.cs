﻿using PoESkillTree.Engine.GameModel.PassiveTree.Base;
using System.Collections.Generic;

namespace PoESkillTree.Engine.GameModel.PassiveTree
{
    public class PassiveNodeDefinition : IDefinition<ushort>
    {
        public PassiveNodeDefinition(
            ushort id, PassiveNodeType type, string name, bool isAscendancyNode, bool costsPassivePoint,
            int passivePointsGranted, NodePosition position, IReadOnlyList<string> modifiers)
            => (Id, Type, Name, IsAscendancyNode, CostsPassivePoint, PassivePointsGranted, Position, Modifiers) =
                (id, type, name, isAscendancyNode, costsPassivePoint, passivePointsGranted, position, modifiers);
        
        private PassiveNodeDefinition(JsonPassiveNode passiveNode)
            : this(passiveNode.Id,
                   passiveNode.PassiveNodeType,
                   passiveNode.Name,
                   passiveNode.IsAscendancyNode,
                   !passiveNode.IsRootNode && !passiveNode.IsAscendancyNode && !passiveNode.IsMultipleChoiceOption,
                   passiveNode.PassivePointsGranted,
                   new NodePosition(passiveNode.Position.X, passiveNode.Position.Y),
                   passiveNode.StatDescriptions)
        {

        }

        public static PassiveNodeDefinition Convert(JsonPassiveNode passiveNode) => new PassiveNodeDefinition(passiveNode);

        public ushort Id { get; }

        public PassiveNodeType Type { get; }
        public string Name { get; }
        public bool IsAscendancyNode { get; }
        public bool CostsPassivePoint { get; }
        public int PassivePointsGranted { get; }

        public NodePosition Position { get; }

        public IReadOnlyList<string> Modifiers { get; }
    }
}