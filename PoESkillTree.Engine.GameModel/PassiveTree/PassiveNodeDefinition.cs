using PoESkillTree.Engine.GameModel.PassiveTree.Base;
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

        private PassiveNodeDefinition(JsonPassiveNode passiveNode)
            : this(passiveNode.Id,
                  passiveNode.PassiveNodeType,
                  passiveNode.Name,
                  passiveNode.IsAscendancyNode,
                  !passiveNode.IsRootNode && !passiveNode.IsAscendancyNode && !passiveNode.IsMultipleChoiceOption,
                  new NodePosition(passiveNode.Position.X, passiveNode.Position.Y),
                  passiveNode.StatDescriptions)
        { }

        public static PassiveNodeDefinition Convert(JsonPassiveNode passiveNode, float zoomLevel = 1f)
        {
            static float? SetZoomLevel(JsonPassiveTreePosition? position, float? _zoomLevel)
            {
                if (position is null || _zoomLevel is null)
                {
                    return null;
                }

                var temp = position.ZoomLevel;
                position.ZoomLevel = _zoomLevel.Value;
                position.ClearPositionCache();
                return temp;
            }

            var tempGroupZoom = SetZoomLevel(passiveNode.PassiveNodeGroup, zoomLevel);
            var tempNodeZoom = SetZoomLevel(passiveNode, zoomLevel);

            var passiveNodeDefinition = new PassiveNodeDefinition(passiveNode);

            SetZoomLevel(passiveNode.PassiveNodeGroup, tempGroupZoom);
            SetZoomLevel(passiveNode, tempNodeZoom);

            return passiveNodeDefinition;
        }

        public ushort Id { get; }

        public PassiveNodeType Type { get; }
        public string Name { get; }
        public bool IsAscendancyNode { get; }
        public bool CostsPassivePoint { get; }

        public NodePosition Position { get; }

        public IReadOnlyList<string> Modifiers { get; }
    }
}