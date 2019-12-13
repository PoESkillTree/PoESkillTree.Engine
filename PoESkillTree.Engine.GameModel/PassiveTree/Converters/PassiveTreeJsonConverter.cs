using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoESkillTree.Engine.GameModel.PassiveTree.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Converters
{
    public class PassiveTreeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(JsonPassiveTree);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var passiveTree = new JsonPassiveTree();

            if (jObject.GetValue("nodes") is JToken nodes && nodes.Type == JTokenType.Array)
            {
                passiveTree.PassiveNodes = nodes.ToObject<List<JsonPassiveNode>>().ToDictionary(i => i.Id, i => i);
                jObject.Remove("nodes");
            }

            if (jObject.GetValue("skillSprites") is JToken spritesToken)
            {
                // This piece should only run when the older style of skill sprites are used (only active and inactive in the dictionary).
                if (spritesToken.ToObject<Dictionary<string, List<JsonPassiveTreeOldSkillSprite>>>() is var sprites && sprites is { Count: 2 })
                {
                    passiveTree.SkillSprites = new Dictionary<string, List<JsonPassiveTreeSkillSprite>>();
                    foreach (var i in sprites)
                    {
                        //i.Key == "active", "inactive"
                        var key = $"{i.Key.First().ToString().ToUpper()}{i.Key.Substring(1)}";
                        foreach (var j in i.Value)
                        {
                            if (!passiveTree.SkillSprites.ContainsKey($"normal{key}"))
                            {
                                passiveTree.SkillSprites.Add($"normal{key}", new List<JsonPassiveTreeSkillSprite>());
                            }
                            passiveTree.SkillSprites[$"normal{key}"].Add(new JsonPassiveTreeSkillSprite() { FileName = j.FileName, Coords = j.Coords ?? new Dictionary<string, JsonPassiveTree2DArt>() });

                            if (!passiveTree.SkillSprites.ContainsKey($"notable{key}"))
                            {
                                passiveTree.SkillSprites.Add($"notable{key}", new List<JsonPassiveTreeSkillSprite>());
                            }
                            passiveTree.SkillSprites[$"notable{key}"].Add(new JsonPassiveTreeSkillSprite() { FileName = j.FileName, Coords = j.NotableCoords ?? new Dictionary<string, JsonPassiveTree2DArt>() });

                            if (!passiveTree.SkillSprites.ContainsKey($"keystone{key}"))
                            {
                                passiveTree.SkillSprites.Add($"keystone{key}", new List<JsonPassiveTreeSkillSprite>());
                            }
                            passiveTree.SkillSprites[$"keystone{key}"].Add(new JsonPassiveTreeSkillSprite() { FileName = j.FileName, Coords = j.KeystoneCoords ?? new Dictionary<string, JsonPassiveTree2DArt>() });
                        }
                    }
                    jObject.Remove("skillSprites");
                }
            }

            serializer.Populate(jObject.CreateReader(), passiveTree);

            // The PassiveNodeInIds are always the Id of the "Current Node" instead of the Id of the "In Node"
            foreach (var passiveNode in passiveTree.PassiveNodes.Values)
            {
                passiveNode.PassiveNodeInIds = new HashSet<ushort>();
            }

            // Hydrate Extra Images
            foreach (var characterClass in passiveTree.ExtraImages.Keys)
            {
                // Set the Maxium Zoom Level to be the Zoom Level of the Extra Images
                passiveTree.ExtraImages[characterClass].ZoomLevel = passiveTree.MaxImageZoomLevel;
            }

            // Hydrate Passive Node Groups
            foreach (var passiveNodeGroup in passiveTree.PassiveNodeGroups.Keys)
            {
                // Set the Maxium Zoom Level to be the Zoom Level of the Passive Node Group
                passiveTree.PassiveNodeGroups[passiveNodeGroup].ZoomLevel = passiveTree.MaxImageZoomLevel;
            }

            // Hydrate Passive Nodes
            foreach (var passiveNode in passiveTree.PassiveNodes.Values)
            {
                passiveNode.SkillsPerOrbit = passiveTree.Constants.SkillsPerOrbit;
                passiveNode.OrbitRadii = passiveTree.Constants.OrbitRadii;
                passiveNode.ZoomLevel = passiveTree.MaxImageZoomLevel;

                if (passiveTree.PassiveNodeGroups.ContainsKey(passiveNode.PassiveNodeGroupId))
                {
                    passiveNode.PassiveNodeGroup = passiveTree.PassiveNodeGroups[passiveNode.PassiveNodeGroupId];
                }

                // Populate proper "In Nodes"
                foreach (var passiveNodeOutId in passiveNode.PassiveNodeOutIds)
                {
                    if (passiveTree.PassiveNodes.ContainsKey(passiveNodeOutId))
                    {
                        if (!passiveTree.PassiveNodes[passiveNodeOutId].PassiveNodeInIds.Contains(passiveNode.Id))
                        {
                            passiveTree.PassiveNodes[passiveNodeOutId].PassiveNodeInIds.Add(passiveNode.Id);
                        }
                    }
                }
            }

            return passiveTree;
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException($"{nameof(CanWrite)} should be false (is {CanWrite}). There is no need for write converter.");
    }
}
