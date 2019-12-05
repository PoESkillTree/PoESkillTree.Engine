using Newtonsoft.Json;
using System.Numerics;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreeExtraImage
    {
        [JsonProperty("x")]
        public float X { get; set; } = 0;

        [JsonProperty("y")]
        public float Y { get; set; } = 0;

        [JsonProperty("image")]
        public string Image { get; set; } = string.Empty;

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
