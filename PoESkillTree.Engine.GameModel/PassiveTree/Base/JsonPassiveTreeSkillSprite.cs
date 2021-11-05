﻿using Newtonsoft.Json;
using PoESkillTree.Engine.GameModel.PassiveTree.Converters;
using System.Collections.Generic;
using System.Drawing;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreeSkillSprite
    {
        [JsonProperty("filename")]
        public string FileName { get; set; } = string.Empty;

        [JsonProperty("coords", ItemConverterType = typeof(PassiveTreeSkillSpriteCoordsConverter))]
        public Dictionary<string, RectangleF> Coords { get; set; } = new Dictionary<string, RectangleF>();
    }

    public class JsonPassiveTreeOldSkillSprite : JsonPassiveTreeSkillSprite
    {
        [JsonProperty("notableCoords", ItemConverterType = typeof(PassiveTreeSkillSpriteCoordsConverter))]
        public Dictionary<string, RectangleF> NotableCoords { get; private set; } = new Dictionary<string, RectangleF>();

        [JsonProperty("keystoneCoords", ItemConverterType = typeof(PassiveTreeSkillSpriteCoordsConverter))]
        public Dictionary<string, RectangleF> KeystoneCoords { get; private set; } = new Dictionary<string, RectangleF>();
    }
}
