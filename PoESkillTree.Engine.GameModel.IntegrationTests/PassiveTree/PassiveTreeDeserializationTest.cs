using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using PoESkillTree.Engine.GameModel.PassiveTree.Base;
using System.IO;

namespace PoESkillTree.Engine.GameModel.PassiveTree
{
    [TestFixture]
    public class PassiveTreeDeserializationTest
    {
        [TestCase("skilltree_3.10.0.new.min.json")]
        [TestCase("skilltree_3.10.0.old.min.json")]
        [TestCase("skilltree_3.8.0.min.json")]
        [TestCase("skilltree_3.15.0.min.json")]
        [TestCase("skilltree_3.16.0.min.json")]
        [TestCase("skilltree_3.17.0.min.json")]
        [TestCase("skilltree_3.17.0_atlas.min.json")]
        public void JsonPassiveTree_Deserialization_Serialization_Deserialization(string fileName)
        {
            var orignalJson = TestUtils.ReadDataFile(fileName);
            Assert.IsNotEmpty(orignalJson);
            
            var settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Error;
            settings.TypeNameHandling = TypeNameHandling.All;
            var deserialized1 = JsonConvert.DeserializeObject<JsonPassiveTree>(orignalJson, settings);

            var serialized1 = JsonConvert.SerializeObject(deserialized1);
            Assert.IsNotEmpty(serialized1);

            var deserialized2 = JsonConvert.DeserializeObject<JsonPassiveTree>(serialized1);
            var serialized2 = JsonConvert.SerializeObject(deserialized2);
            Assert.IsNotEmpty(serialized2);

            serialized1.Should().BeEquivalentTo(serialized2);
        }
    }
}