using Newtonsoft.Json;
using System.Numerics;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreePosition
    {
        [JsonProperty("x")]
        public virtual float OriginalX { get; set; } = 0f;

        [JsonProperty("y")]
        public virtual float OriginalY { get; set; } = 0f;

        [JsonIgnore]
        public virtual float ZoomLevel { get; set; } = 1f;

        [JsonIgnore]
        protected Vector2? _position = null;

        [JsonIgnore]
        public virtual Vector2 Position
        {
            get
            {
                if (!_position.HasValue)
                {
                    _position = new Vector2(OriginalX * ZoomLevel, OriginalY * ZoomLevel);
                }

                return _position.Value;
            }
        }

        public void ClearPositionCache() => _position = null;
    }
}
