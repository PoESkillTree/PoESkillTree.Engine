﻿using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using PoESkillTree.Engine.GameModel.PassiveTree.Base;

namespace PoESkillTree.Engine.GameModel.PassiveTree
{
    [TestFixture]
    public class PassiveTreeDeserializationTest
    {
        [TestCase("skilltree_3.10.0.new.min.json")]
        [TestCase("skilltree_3.10.0.old.min.json")]
        [TestCase("skilltree_3.8.0.min.json")]
        public void JsonPassiveTree_Deserialization_Serialization_Deserialization(string fileName)
        {
            var orignalJson = TestUtils.ReadDataFile(fileName);
            Assert.IsNotEmpty(orignalJson);

            var deserialized1 = JsonConvert.DeserializeObject<JsonPassiveTree>(orignalJson);
            var serialized1 = JsonConvert.SerializeObject(deserialized1);
            Assert.IsNotEmpty(serialized1);

            var deserialized2 = JsonConvert.DeserializeObject<JsonPassiveTree>(serialized1);
            var serialized2 = JsonConvert.SerializeObject(deserialized2);
            Assert.IsNotEmpty(serialized2);

            serialized1.Should().BeEquivalentTo(serialized2);
        }
    }
}