using NUnit.Framework;
using System;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveNodeTest
    {
        [TestCase(CharacterClass.Scion)]
        [TestCase(CharacterClass.Marauder)]
        [TestCase(CharacterClass.Ranger)]
        [TestCase(CharacterClass.Witch)]
        [TestCase(CharacterClass.Duelist)]
        [TestCase(CharacterClass.Templar)]
        [TestCase(CharacterClass.Shadow)]
        [TestCase(null)]
        public void JsonPassiveNode_StartingCharacterClass(CharacterClass? characterClass)
        {
            var node = new JsonPassiveNode()
            {
                StartingCharacterClasses = characterClass.HasValue ? new[] { characterClass.Value } : new CharacterClass[0]
            };

            Assert.AreEqual(characterClass.HasValue, node.IsRootNode);
            Assert.AreEqual(characterClass, node.StartingCharacterClass);
        }

        [TestCase("", ExpectedResult = false)]
        [TestCase("testing", ExpectedResult = true)]
        public bool JsonPassiveNode_IsAscendancyNode(string ascendancyName)
        {
            var node = new JsonPassiveNode
            {
                AscendancyName = ascendancyName
            };

            return node.IsAscendancyNode;
        }

        [TestCase(PassiveNodeType.Keystone, ExpectedResult = PassiveNodeType.Keystone)]
        [TestCase(PassiveNodeType.Mastery, ExpectedResult = PassiveNodeType.Mastery)]
        [TestCase(PassiveNodeType.Notable, ExpectedResult = PassiveNodeType.Notable)]
        [TestCase(PassiveNodeType.JewelSocket, ExpectedResult = PassiveNodeType.JewelSocket)]
        [TestCase(PassiveNodeType.Small, ExpectedResult = PassiveNodeType.Small)]
        [TestCase(PassiveNodeType.Keystone | PassiveNodeType.Mastery | PassiveNodeType.Notable | PassiveNodeType.JewelSocket | PassiveNodeType.Small, ExpectedResult = PassiveNodeType.Small)]
        [TestCase(42, ExpectedResult = PassiveNodeType.Small)]
        public PassiveNodeType JsonPassiveNode_PassiveNodeType(PassiveNodeType passiveNodeType)
        {
            var node = new JsonPassiveNode
            {
                IsKeystone = passiveNodeType == PassiveNodeType.Keystone,
                IsMastery = passiveNodeType == PassiveNodeType.Mastery,
                IsNotable = passiveNodeType == PassiveNodeType.Notable,
                IsJewelSocket = passiveNodeType == PassiveNodeType.JewelSocket,
            };

            return node.PassiveNodeType;
        }

        [TestCase(0, new[] { 1f })]
        [TestCase(0, new[] { 1f })]
        [TestCase(0, new[] { 1f })]
        [TestCase(0, new[] { 1f })]
        [TestCase(1, new[] { 1f, 2f })]
        [TestCase(1, new[] { 1f, 2f })]
        [TestCase(1, new[] { 1f, 2f })]
        [TestCase(1, new[] { 1f, 2f })]
        public void JsonPassiveNode_Arc(int skillsPerOrbitIndex, float[] skillsPerOrbit)
        {
            var node = new JsonPassiveNode
            {
                SkillsPerOrbitIndex = skillsPerOrbitIndex,
                SkillsPerOrbit = skillsPerOrbit,
            };

            Assert.AreEqual(2 * Math.PI * skillsPerOrbitIndex / skillsPerOrbit[skillsPerOrbitIndex], node.Arc);
        }

        [TestCase(0, 0, 0.3835f, 0, new[] { 0f }, 0, new[] { 1f })]
        [TestCase(0, 1, 0.3835f, 0, new[] { 0f }, 0, new[] { 1f })]
        [TestCase(1, 0, 0.3835f, 0, new[] { 0f }, 0, new[] { 1f })]
        [TestCase(1, 1, 0.3835f, 0, new[] { 0f }, 0, new[] { 1f })]
        [TestCase(500, 250, 0.3835f, 1, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        [TestCase(250, 500, 0.3835f, 1, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        [TestCase(500, 500, 0.3835f, 1, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        [TestCase(250, 250, 0.3835f, 1, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        [TestCase(250, 250, 0.3835f, 0, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        public void JsonPassiveNode_Position(float x, float y, float zoomLevel, int orbitRadiiIndex, float[] orbitRadii, int skillsPerOrbitIndex, float[] skillsPerOrbit)
        {
            var group = new JsonPassiveNodeGroup
            {
                OriginalX = x,
                OriginalY = y,
                ZoomLevel = zoomLevel,
            };

            var node = new JsonPassiveNode
            {
                PassiveNodeGroup = group,
                OrbitRadiiIndex = orbitRadiiIndex,
                OrbitRadii = orbitRadii,
                SkillsPerOrbitIndex = skillsPerOrbitIndex,
                SkillsPerOrbit = skillsPerOrbit,
                ZoomLevel = zoomLevel,
            };

            Assert.AreEqual(group.Position.X - orbitRadii[orbitRadiiIndex] * zoomLevel * (float)Math.Sin(-2 * Math.PI * skillsPerOrbitIndex / skillsPerOrbit[skillsPerOrbitIndex]), node.Position.X);
            Assert.AreEqual(group.Position.Y - orbitRadii[orbitRadiiIndex] * zoomLevel * (float)Math.Cos(-2 * Math.PI * skillsPerOrbitIndex / skillsPerOrbit[skillsPerOrbitIndex]), node.Position.Y);
        }

        [TestCase(0, 0, 0.3835f, 0, new[] { 0f }, 0, new[] { 1f })]
        [TestCase(0, 1, 0.3835f, 0, new[] { 0f }, 0, new[] { 1f })]
        [TestCase(1, 0, 0.3835f, 0, new[] { 0f }, 0, new[] { 1f })]
        [TestCase(1, 1, 0.3835f, 0, new[] { 0f }, 0, new[] { 1f })]
        [TestCase(500, 250, 0.3835f, 1, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        [TestCase(250, 500, 0.3835f, 1, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        [TestCase(500, 500, 0.3835f, 1, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        [TestCase(250, 250, 0.3835f, 1, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        [TestCase(250, 250, 0.3835f, 0, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        public void JsonPassiveNode_Convert(float _x, float _y, float zoomLevel, int orbitRadiiIndex, float[] orbitRadii, int skillsPerOrbitIndex, float[] skillsPerOrbit)
        {
            var group = new JsonPassiveNodeGroup
            {
                OriginalX = _x,
                OriginalY = _y,
                ZoomLevel = zoomLevel,
            };

            var node = new JsonPassiveNode
            {
                PassiveNodeGroup = group,
                OrbitRadiiIndex = orbitRadiiIndex,
                OrbitRadii = orbitRadii,
                SkillsPerOrbitIndex = skillsPerOrbitIndex,
                SkillsPerOrbit = skillsPerOrbit,
                ZoomLevel = zoomLevel,
            };

            var nodeDefinition = PassiveNodeDefinition.Convert(node);
            nodeDefinition.Position.Deconstruct(out double x, out double y);
            Assert.AreEqual(nodeDefinition.Position.X, x);
            Assert.AreEqual(nodeDefinition.Position.Y, y);
            Assert.AreEqual(node.Position.X, x);
            Assert.AreEqual(node.Position.Y, y);
        }

        [TestCase(0, 0, 0.3835f, 0, new[] { 0f }, 0, new[] { 1f })]
        [TestCase(0, 1, 0.3835f, 0, new[] { 0f }, 0, new[] { 1f })]
        [TestCase(1, 0, 0.3835f, 0, new[] { 0f }, 0, new[] { 1f })]
        [TestCase(1, 1, 0.3835f, 0, new[] { 0f }, 0, new[] { 1f })]
        [TestCase(500, 250, 0.3835f, 1, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        [TestCase(250, 500, 0.3835f, 1, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        [TestCase(500, 500, 0.3835f, 1, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        [TestCase(250, 250, 0.3835f, 1, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        [TestCase(250, 250, 0.3835f, 0, new[] { 0f, 82f }, 1, new[] { 1f, 2f })]
        public void JsonPassiveNode_ClearPosition(float x, float y, float zoomLevel, int orbitRadiiIndex, float[] orbitRadii, int skillsPerOrbitIndex, float[] skillsPerOrbit)
        {
            var group = new JsonPassiveNodeGroup
            {
                OriginalX = x,
                OriginalY = y,
                ZoomLevel = zoomLevel,
            };

            var node = new JsonPassiveNode
            {
                PassiveNodeGroup = group,
                OrbitRadiiIndex = orbitRadiiIndex,
                OrbitRadii = orbitRadii,
                SkillsPerOrbitIndex = skillsPerOrbitIndex,
                SkillsPerOrbit = skillsPerOrbit,
                ZoomLevel = zoomLevel,
            };

            Assert.AreEqual(group.Position.X - orbitRadii[orbitRadiiIndex] * zoomLevel * (float)Math.Sin(-2 * Math.PI * skillsPerOrbitIndex / skillsPerOrbit[skillsPerOrbitIndex]), node.Position.X);
            Assert.AreEqual(group.Position.Y - orbitRadii[orbitRadiiIndex] * zoomLevel * (float)Math.Cos(-2 * Math.PI * skillsPerOrbitIndex / skillsPerOrbit[skillsPerOrbitIndex]), node.Position.Y);

            var newGroup = new JsonPassiveNodeGroup
            {
                OriginalX = x * 10,
                OriginalY = y * 10,
                ZoomLevel = zoomLevel,
            };

            node.PassiveNodeGroup = newGroup;
            node.ClearPositionCache();

            Assert.AreEqual(newGroup.Position.X - orbitRadii[orbitRadiiIndex] * zoomLevel * (float)Math.Sin(-2 * Math.PI * skillsPerOrbitIndex / skillsPerOrbit[skillsPerOrbitIndex]), node.Position.X);
            Assert.AreEqual(newGroup.Position.Y - orbitRadii[orbitRadiiIndex] * zoomLevel * (float)Math.Cos(-2 * Math.PI * skillsPerOrbitIndex / skillsPerOrbit[skillsPerOrbitIndex]), node.Position.Y);
        }
    }
}
