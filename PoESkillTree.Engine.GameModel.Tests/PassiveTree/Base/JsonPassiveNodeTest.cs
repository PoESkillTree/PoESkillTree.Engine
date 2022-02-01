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
                StartingCharacterClass = characterClass
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
    }
}
