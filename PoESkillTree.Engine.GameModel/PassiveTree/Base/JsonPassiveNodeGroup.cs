using Newtonsoft.Json;
using PoESkillTree.Engine.GameModel.PassiveTree.Converters;
using System.Collections.Generic;
using System.Numerics;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    [JsonConverter(typeof(PassiveNodeGroupJsonConverter))]
    public class JsonPassiveNodeGroup
    {
        [JsonProperty("x")]
        public float X { get; set; } = 0f;

        [JsonProperty("y")]
        public float Y { get; set; } = 0f;

        [JsonProperty("oo")]
        public Dictionary<ushort, bool> OccupiedOrbits { get; set; } = new Dictionary<ushort, bool>();

        [JsonProperty("n")]
        public HashSet<ushort> PassiveNodeIds { get; set; } = new HashSet<ushort>();

        #region Calculated Properties 
        [JsonIgnore]
        private Vector2? _position = null;

        [JsonIgnore]
        public Vector2 Position
        {
            get
            {
                if (!_position.HasValue)
                {
                    _position = new Vector2(X, Y);
                }

                return _position.Value;
            }
        }
        #endregion
    }
}
