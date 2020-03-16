using NUnit.Framework;
using System.Collections.Generic;

namespace PoESkillTree.Engine.GameModel.PassiveTree
{
    public class PassiveTreeBuildUrlDataTest
    {
        [TestCase(CharacterClass.Scion, 0, 4, false)]
        [TestCase(CharacterClass.Scion, 0, 4, true)]
        [TestCase(CharacterClass.Shadow, 0, 4, false)]
        [TestCase(CharacterClass.Shadow, 3, 4, false)]
        [TestCase(CharacterClass.Scion, 0, 3, true)]
        public void PassiveTreeBuildUrlData_ConstructorDefault(CharacterClass characterClass, int ascendancyClass, int version, bool fullscreen)
        {
            var passiveNodeIds = new List<ushort>() { 100, 200, 300 };
            
            var expected = new PassiveTreeBuildUrlData(characterClass, ascendancyClass, passiveNodeIds, version, fullscreen);
            Assert.AreEqual(characterClass, expected.CharacterClass);
            Assert.AreEqual(ascendancyClass, expected.AscendancyClass);
            Assert.AreEqual(passiveNodeIds, expected.PassiveNodeIds);
            Assert.AreEqual(version, expected.Version);
            Assert.AreEqual(fullscreen, expected.Fullscreen);
        }

        [TestCase(CharacterClass.Scion, 0, 4, false)]
        [TestCase(CharacterClass.Scion, 0, 4, true)]
        [TestCase(CharacterClass.Shadow, 0, 4, false)]
        [TestCase(CharacterClass.Shadow, 3, 4, false)]
        [TestCase(CharacterClass.Scion, 0, 3, true)]
        public void PassiveTreeBuildUrlData_ConstructorDecode(CharacterClass characterClass, int ascendancyClass, int version, bool fullscreen)
        {
            var passiveNodeIds = new List<ushort>() { 100, 200, 300 };

            var expected = new PassiveTreeBuildUrlData(characterClass, ascendancyClass, passiveNodeIds, version, fullscreen);
            Assert.AreEqual(characterClass, expected.CharacterClass);
            Assert.AreEqual(ascendancyClass, expected.AscendancyClass);
            Assert.AreEqual(passiveNodeIds, expected.PassiveNodeIds);
            Assert.AreEqual(version, expected.Version);
            Assert.AreEqual(fullscreen, expected.Fullscreen);

            var encoded = expected.EncodeUrl();

            var actual = new PassiveTreeBuildUrlData(encoded);
            Assert.AreEqual(expected.CharacterClass, actual.CharacterClass);
            Assert.AreEqual(expected.AscendancyClass, actual.AscendancyClass);
            Assert.AreEqual(expected.PassiveNodeIds, actual.PassiveNodeIds);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.Fullscreen, actual.Fullscreen);
        }

        [Test]
        public void PassiveTreeBuildUrlData_ConstructorDecode_Empty()
        {
            var expected = new PassiveTreeBuildUrlData { };
            var actual = new PassiveTreeBuildUrlData(string.Empty);

            Assert.AreEqual(expected.CharacterClass, actual.CharacterClass);
            Assert.AreEqual(expected.AscendancyClass, actual.AscendancyClass);
            Assert.AreEqual(expected.PassiveNodeIds, actual.PassiveNodeIds);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.Fullscreen, actual.Fullscreen);
        }
    }
}
