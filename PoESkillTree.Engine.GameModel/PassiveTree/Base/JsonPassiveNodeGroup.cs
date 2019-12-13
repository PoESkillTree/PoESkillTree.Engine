using Newtonsoft.Json;
using PoESkillTree.Engine.GameModel.PassiveTree.Converters;
using System.Collections.Generic;
using System.Numerics;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    [JsonConverter(typeof(PassiveNodeGroupJsonConverter))]
    public class JsonPassiveNodeGroup : JsonPassiveTreePosition
    {
        [JsonProperty("oo")]
        public Dictionary<ushort, bool> OccupiedOrbits { get; set; } = new Dictionary<ushort, bool>();

        [JsonProperty("n")]
        public HashSet<ushort> PassiveNodeIds { get; set; } = new HashSet<ushort>();
    }
}
