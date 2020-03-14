using Newtonsoft.Json;
using PoESkillTree.Engine.GameModel.PassiveTree.Converters;
using System.Collections.Generic;
using System.Linq;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    [JsonConverter(typeof(PassiveNodeGroupJsonConverter))]
    public class JsonPassiveNodeGroup : JsonPassiveTreePosition
    {
        [JsonProperty("orbits")]
        public List<ushort> OccupiedOrbits { get; private set; } = new List<ushort>();

        [JsonProperty("nodes")]
        public HashSet<ushort> PassiveNodeIds { get; private set; } = new HashSet<ushort>();

        [JsonProperty("isProxy", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsProxy { get; set; }

        #region Legacy Parsing Purposes
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE1006 // Naming Styles
        [JsonProperty("oo")]
        private Dictionary<ushort, bool> __oo
        {
            set
            {
                OccupiedOrbits.AddRange(from orbit in value select orbit.Key);
            }
        }

        [JsonProperty("n")]
        private HashSet<ushort> __n { set { PassiveNodeIds = value; } }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0051 // Remove unused private members
        #endregion
    }
}
