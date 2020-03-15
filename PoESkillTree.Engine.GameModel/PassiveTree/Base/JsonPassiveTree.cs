using EnumsNET;
using Newtonsoft.Json;
using PoESkillTree.Engine.GameModel.PassiveTree.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    [JsonConverter(typeof(PassiveTreeJsonConverter))]
    public class JsonPassiveTree
    {
        [JsonProperty("classes")]
        public List<JsonPassiveTreeCharacterClass> CharacterClasses { get; } = new List<JsonPassiveTreeCharacterClass>();

        [JsonProperty("groups")]
        public Dictionary<ushort, JsonPassiveNodeGroup> PassiveNodeGroups { get; } = new Dictionary<ushort, JsonPassiveNodeGroup>();

        [JsonProperty("root")]
        public JsonPassiveNode Root { get; set; } = new JsonPassiveNode();

        [JsonProperty("nodes")]
        public Dictionary<ushort, JsonPassiveNode> PassiveNodes { get; } = new Dictionary<ushort, JsonPassiveNode>();

        [JsonProperty("min_x")]
        public float MinX { get; set; }

        [JsonProperty("min_y")]
        public float MinY { get; set; }

        [JsonProperty("max_x")]
        public float MaxX { get; set; }

        [JsonProperty("max_y")]
        public float MaxY { get; set; }

        [JsonProperty("assets")]
        public Dictionary<string, Dictionary<string, string>> Assets { get; } = new Dictionary<string, Dictionary<string, string>>();

        [JsonProperty("constants")]
        public JsonPassiveTreeConstants Constants { get; set; } = new JsonPassiveTreeConstants();

        [JsonProperty("imageZoomLevels")]
        public float[] ImageZoomLevels { get; set; } = new float[] { 0.1246f, 0.2109f, 0.2972f, 0.3835f };

        [JsonProperty("skillSprites")]
        public Dictionary<string, List<JsonPassiveTreeSkillSprite>> SkillSprites { get; } = new Dictionary<string, List<JsonPassiveTreeSkillSprite>>();

        [JsonProperty("extraImages")]
        public Dictionary<CharacterClass, JsonPassiveTreeExtraImage> ExtraImages { get; } = new Dictionary<CharacterClass, JsonPassiveTreeExtraImage>();

        [JsonProperty("jewelSlots")]
        public List<ushort> JewelPassiveNodeIds { get; } = new List<ushort>();

        #region Calculated Properties
        [JsonIgnore]
        public int MaxImageZoomLevelIndex => ImageZoomLevels.Length - 1;

        [JsonIgnore]
        public float MaxImageZoomLevel => MaxImageZoomLevelIndex >= 0 ? ImageZoomLevels[MaxImageZoomLevelIndex] : 1f;

        [JsonIgnore]
        private RectangleF? _bounds = null;

        [JsonIgnore]
        public RectangleF Bounds
        {
            get
            {
                if (!_bounds.HasValue)
                {
                    _bounds = new RectangleF(MinX, MinY, MaxX - MinX, MaxY - MinY);
                }

                return _bounds.Value;
            }
        }

        [JsonIgnore]
        public Uri WebCDN => new Uri(@"http://web.poecdn.com/");

        [JsonIgnore]
        public Uri ImageUri => new Uri(WebCDN, ImageRoot);

        [JsonIgnore]
        public Uri SpriteSheetUri => new Uri(ImageUri, @"build-gen/passive-skill-sprite/");

        [JsonIgnore]
        private string? _imageRoot = null;

        [JsonProperty("imageRoot")]
        public string ImageRoot
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_imageRoot))
                {
                    _imageRoot = @"/image/";
                }

                return _imageRoot;
            }
            set => _imageRoot = $"/{value.TrimStart('/').TrimEnd('/')}/".Replace(WebCDN.AbsoluteUri, string.Empty);
        }
        #endregion

        #region Legacy Parsing Purposes
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE1006 // Naming Styles
        [JsonProperty("characterData")]
        private Dictionary<CharacterClass, JsonPassiveTreeCharacterClass> __characterData
        {
            set
            {
                if (!(value is null))
                {
                    foreach (var (key, character) in value.OrderBy(x => x.Key))
                    {
                        character.Name = Enums.AsString(key);
                        CharacterClasses.Add(character);
                    }
                }
            }
        }

#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0051 // Remove unused private members
        #endregion
    }
}
