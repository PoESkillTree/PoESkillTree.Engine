using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoESkillTree.Engine.GameModel.PassiveTree.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Converters
{
    public class PassiveNodeGroupJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(JsonPassiveNodeGroup);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var passiveNodeGroup = new JsonPassiveNodeGroup();

            if (jObject.GetValue("oo") is JToken oo && oo.Type == JTokenType.Array)
            {
                passiveNodeGroup.OccupiedOrbits.Clear();
                foreach (var item in oo.ToObject<List<bool>>().Select((value, index) => (Index: (ushort)index, Value: value)))
                {
                    passiveNodeGroup.OccupiedOrbits.Add(item.Index, item.Value);
                }

                jObject.Remove("oo");
            }

            serializer.Populate(jObject.CreateReader(), passiveNodeGroup);
            return passiveNodeGroup;
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException($"{nameof(CanWrite)} should be false (is {CanWrite}). There is no need for write converter.");
    }
}
