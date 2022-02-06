using EnumsNET;
using MoreLinq.Extensions;
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
        [JsonProperty("tree")]
        public string Tree { get; set; } = "Default";

        [JsonProperty("classes")]
        public List<JsonPassiveTreeCharacterClass> CharacterClasses { get; private set; } = new List<JsonPassiveTreeCharacterClass>();

        [JsonProperty("groups")]
        public Dictionary<ushort, JsonPassiveNodeGroup> PassiveNodeGroups { get; private set; } = new Dictionary<ushort, JsonPassiveNodeGroup>();

        [JsonProperty("root")]
        public JsonPassiveNode Root { get; set; } = new JsonPassiveNode();
        public bool ShouldSerializeRoot() => false;

        [JsonIgnore]
        public Dictionary<ushort, JsonPassiveNode>? _passiveNodes = null;

        [JsonIgnore]
        public Dictionary<ushort, JsonPassiveNode> PassiveNodes
        {
            get
            {
                if (_passiveNodes == null)
                {
                    _passiveNodes = new Dictionary<ushort, JsonPassiveNode>();
                    if (__nodes.Count > 0)
                    {
                        var maxLength = __nodes.Max(x => x.Key.Length);
                        foreach (var node in __nodes.OrderBy(x => x.Key.PadLeft(maxLength, '0')))
                        {
                            if (ushort.TryParse(node.Key, out var id))
                            {
                                if (node.Value.Skill == 0)
                                {
                                    node.Value.Skill = id;
                                }
                                node.Value.Id = id;
                                _passiveNodes[id] = node.Value;
                            }
                        }
                    }
                }

                return _passiveNodes;
            }
            set
            {
                _passiveNodes = value;
                __nodes = _passiveNodes.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value);
            }
        }

        [JsonProperty("nodes")]
        public Dictionary<string, JsonPassiveNode> __nodes = new Dictionary<string, JsonPassiveNode>();

        [JsonProperty("min_x")]
        public float MinX { get; set; }

        [JsonProperty("min_y")]
        public float MinY { get; set; }

        [JsonProperty("max_x")]
        public float MaxX { get; set; }

        [JsonProperty("max_y")]
        public float MaxY { get; set; }

        [JsonProperty("assets")]
        public Dictionary<string, Dictionary<string, string>> Assets { get; private set; } = new Dictionary<string, Dictionary<string, string>>();

        [JsonProperty("constants")]
        public JsonPassiveTreeConstants Constants { get; set; } = new JsonPassiveTreeConstants();

        [JsonProperty("imageZoomLevels")]
        public float[] ImageZoomLevels { get; set; } = new float[] { 0.1246f, 0.2109f, 0.2972f, 0.3835f };

        [JsonProperty("skillSprites")]
        public Dictionary<string, List<JsonPassiveTreeSkillSprite>> SkillSprites { get; private set; } = new Dictionary<string, List<JsonPassiveTreeSkillSprite>>();

        [JsonIgnore]
        public Dictionary<CharacterClass, JsonPassiveTreeExtraImage> ExtraImages => _extraImages.ToDictionary(k => (CharacterClass)k.Key, k => k.Value);

        [JsonProperty("extraImages")]
        private Dictionary<int, JsonPassiveTreeExtraImage> _extraImages = new Dictionary<int, JsonPassiveTreeExtraImage>();

        [JsonProperty("jewelSlots")]
        public List<ushort> JewelSocketPassiveNodeIds { get; private set; } = new List<ushort>();

        [JsonProperty("points", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonPassiveTreePoints? Points { get; set; } = null;

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
        public bool ShouldSerializeImageRoot() => false;
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
