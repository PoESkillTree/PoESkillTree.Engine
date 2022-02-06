using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PoESkillTree.Engine.GameModel.PassiveTree.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Converters
{
    public class PassiveTreeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(JsonPassiveTree);

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var passiveTree = new JsonPassiveTree();

            if (jObject.GetValue("nodes") is JToken nodesToken)
            {
                if (nodesToken.Type == JTokenType.Array)
                {
                    var nodes = new Dictionary<ushort, JsonPassiveNode>();
                    foreach (var item in nodesToken.ToObject<List<JsonPassiveNode>>() ?? new List<JsonPassiveNode>())
                    {
                        item.Skill = item.Id;
                        nodes.Add(item.Id, item);
                    }
                    jObject.Remove("nodes");
                    passiveTree.PassiveNodes = nodes;
                    passiveTree.__nodes.Add("root", passiveTree.Root);
                }
                else
                {
                    var nodes = nodesToken.ToObject<Dictionary<string, JsonPassiveNode>>() ?? new Dictionary<string, JsonPassiveNode>();
                    if (nodes.FirstOrDefault(kvp => kvp.Key == "root") is { Value: JsonPassiveNode root }) {
                        passiveTree.Root = root;
                    }
                }
            }

            if (jObject.GetValue("skillSprites") is JToken spritesToken)
            {
                // This piece should only run when the older style of skill sprites are used (only active and inactive in the dictionary).
                if (spritesToken.ToObject<Dictionary<string, List<JsonPassiveTreeOldSkillSprite>>>() is var sprites && sprites is { Count: 2 })
                {
                    passiveTree.SkillSprites.Clear();
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
                            passiveTree.SkillSprites[$"normal{key}"].Add(new JsonPassiveTreeSkillSprite { FileName = j.FileName, Coords = j.Coords ?? new Dictionary<string, RectangleF>() });

                            if (!passiveTree.SkillSprites.ContainsKey($"notable{key}"))
                            {
                                passiveTree.SkillSprites.Add($"notable{key}", new List<JsonPassiveTreeSkillSprite>());
                            }
                            passiveTree.SkillSprites[$"notable{key}"].Add(new JsonPassiveTreeSkillSprite { FileName = j.FileName, Coords = j.NotableCoords ?? new Dictionary<string, RectangleF>() });

                            if (!passiveTree.SkillSprites.ContainsKey($"keystone{key}"))
                            {
                                passiveTree.SkillSprites.Add($"keystone{key}", new List<JsonPassiveTreeSkillSprite>());
                            }
                            passiveTree.SkillSprites[$"keystone{key}"].Add(new JsonPassiveTreeSkillSprite { FileName = j.FileName, Coords = j.KeystoneCoords ?? new Dictionary<string, RectangleF>() });
                        }
                    }
                    jObject.Remove("skillSprites");
                }
            }

            serializer.Populate(jObject.CreateReader(), passiveTree);

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
            var angles = passiveTree.Constants.OrbitAngles;
            foreach (var passiveNode in passiveTree.PassiveNodes.Values)
            {
                passiveNode.OrbitAngles = angles;
                passiveNode.OrbitRadii = passiveTree.Constants.OrbitRadii;
                passiveNode.ZoomLevel = passiveTree.MaxImageZoomLevel;

                if (passiveNode.PassiveNodeGroupId.HasValue && passiveTree.PassiveNodeGroups.ContainsKey(passiveNode.PassiveNodeGroupId.Value))
                {
                    passiveNode.PassiveNodeGroup = passiveTree.PassiveNodeGroups[passiveNode.PassiveNodeGroupId.Value];
                    passiveNode.PassiveNodeGroup.PassiveNodes[passiveNode.Id] = passiveNode;
                }

                if (passiveNode.InPassiveNodeIds.Contains(passiveNode.Id))
                {
                    passiveNode.InPassiveNodeIds.Remove(passiveNode.Id);
                }

                // Populate proper "In Nodes"
                foreach (var passiveNodeOutId in passiveNode.OutPassiveNodeIds)
                {
                    if (passiveTree.PassiveNodes.ContainsKey(passiveNodeOutId))
                    {
                        passiveNode.NeighborPassiveNodes[passiveNodeOutId] = passiveTree.PassiveNodes[passiveNodeOutId];

                        if (!passiveTree.PassiveNodes[passiveNodeOutId].InPassiveNodeIds.Contains(passiveNode.Id))
                        {
                            passiveTree.PassiveNodes[passiveNodeOutId].InPassiveNodeIds.Add(passiveNode.Id);
                            passiveTree.PassiveNodes[passiveNodeOutId].NeighborPassiveNodes[passiveNode.Id] = passiveNode;
                        }
                    }
                }
            }

            return passiveTree;
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException($"{nameof(CanWrite)} should be false (is {CanWrite}). There is no need for write converter.");
    }
}
