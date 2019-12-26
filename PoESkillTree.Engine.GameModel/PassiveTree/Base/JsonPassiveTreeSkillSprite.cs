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
        public Dictionary<string, JsonPassiveTree2DArt> Coords { get; } = new Dictionary<string, JsonPassiveTree2DArt>();

        public JsonPassiveTreeSkillSprite(string fileName, Dictionary<string, JsonPassiveTree2DArt>? coords)
        {
            FileName = fileName;
            if (coords is { })
            {
                Coords = coords;
            }
        }
    }

    public class JsonPassiveTreeOldSkillSprite : JsonPassiveTreeSkillSprite
    {
        public JsonPassiveTreeOldSkillSprite(string fileName, Dictionary<string, JsonPassiveTree2DArt>? coords) : base(fileName, coords) { }

        [JsonProperty("notableCoords")]
        public Dictionary<string, JsonPassiveTree2DArt> NotableCoords { get; } = new Dictionary<string, JsonPassiveTree2DArt>();

        [JsonProperty("keystoneCoords")]
        public Dictionary<string, JsonPassiveTree2DArt> KeystoneCoords { get; } = new Dictionary<string, JsonPassiveTree2DArt>();
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
