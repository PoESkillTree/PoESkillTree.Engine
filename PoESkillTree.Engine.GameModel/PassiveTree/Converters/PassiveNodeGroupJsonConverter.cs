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

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var passiveNodeGroup = new JsonPassiveNodeGroup();

            if (jObject.GetValue("oo") is JToken oo && oo.Type == JTokenType.Object)
            {
                ParseOccupiedOrbitsToken(oo);
                jObject.Remove("oo");
            }

            if (jObject.GetValue("oo") is JToken ooArray && ooArray.Type == JTokenType.Array)
            {
                var array = ooArray.ToObject<List<ushort>>() ?? new List<ushort>();
                if (array.Count == 1)
                {
                    passiveNodeGroup.OccupiedOrbits.Add(0);
                }
                else
                {
                    passiveNodeGroup.OccupiedOrbits.AddRange(from item in array select item);
                }
                jObject.Remove("oo");
            }

            if (jObject.GetValue("orbits") is JToken orbits && orbits.Type == JTokenType.Object)
            {
                ParseOccupiedOrbitsToken(orbits);
                jObject.Remove("orbits");
            }

            void ParseOccupiedOrbitsToken(JToken token)
            {
                passiveNodeGroup.OccupiedOrbits.Clear();
                passiveNodeGroup.OccupiedOrbits.AddRange(from item in token.ToObject<Dictionary<ushort, bool>>() ?? new Dictionary<ushort, bool>() select item.Key);
            }

            serializer.Populate(jObject.CreateReader(), passiveNodeGroup);
            return passiveNodeGroup;
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException($"{nameof(CanWrite)} should be false (is {CanWrite}). There is no need for write converter.");
    }
}
