using Newtonsoft.Json;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Options
{
    public class JsonJewelHighlight
    {
        [JsonProperty("level")]
        public float ZoomLevel { get; set; } = float.Epsilon;

        [JsonProperty("width")]
        public int Diameter { get; set; } = 0;

        #region Calculated Properties
        [JsonIgnore]
        public int Radius => Diameter / 2;

        [JsonIgnore]
        public float ZoomLevelDiameter => Diameter / ZoomLevel;

        [JsonIgnore]
        public float ZoomLevelRadius => ZoomLevelDiameter / 2;
        #endregion
    }
}
