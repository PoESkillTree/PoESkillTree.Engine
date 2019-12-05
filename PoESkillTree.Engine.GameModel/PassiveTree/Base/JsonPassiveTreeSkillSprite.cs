using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreeSkillSprite
    {
        [JsonProperty("filename")]
        public string FileName { get; set; } = string.Empty;

        [JsonProperty("coords")]
        public Dictionary<string, JsonPassiveTree2DArt> Coords { get; set; } = new Dictionary<string, JsonPassiveTree2DArt>();
    }

    public class JsonPassiveTreeOldSkillSprite : JsonPassiveTreeSkillSprite
    {
        [JsonProperty("notableCoords")]
        public Dictionary<string, JsonPassiveTree2DArt> NotableCoords { get; set; } = new Dictionary<string, JsonPassiveTree2DArt>();

        [JsonProperty("keystoneCoords")]
        public Dictionary<string, JsonPassiveTree2DArt> KeystoneCoords { get; set; } = new Dictionary<string, JsonPassiveTree2DArt>();
    }

    public class JsonPassiveTree2DArt
    {
        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("h")]
        public int Height { get; set; }

        #region Calculated Properties
        [JsonIgnore]
        private Rectangle? _bounds = null;

        [JsonIgnore]
        public Rectangle Bounds
        {
            get
            {
                if (!_bounds.HasValue)
                {
                    _bounds = new Rectangle(X, Y, Width, Height);
                }

                return _bounds.Value;
            }
        }
        #endregion
    }
}
