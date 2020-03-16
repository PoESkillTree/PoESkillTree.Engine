using Newtonsoft.Json;
using System.Numerics;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreePosition
    {
        [JsonProperty("x", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float OriginalX { get; set; } = 0f;

        [JsonProperty("y", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public float OriginalY { get; set; } = 0f;

        [JsonIgnore]
        public float ZoomLevel { get; set; } = 1f;

        [JsonIgnore]
        protected Vector2? _position = null;

        [JsonIgnore]
        public virtual Vector2 Position
        {
            get
            {
                if (!_position.HasValue)
                {
                    _position = PositionAtZoomLevel(ZoomLevel);
                }

                return _position.Value;
            }
        }

        public virtual Vector2 PositionAtZoomLevel(float zoomLevel)
        {
            return new Vector2(OriginalX * zoomLevel, OriginalY * zoomLevel);
        }

        public void ClearPositionCache() => _position = null;
    }
}
