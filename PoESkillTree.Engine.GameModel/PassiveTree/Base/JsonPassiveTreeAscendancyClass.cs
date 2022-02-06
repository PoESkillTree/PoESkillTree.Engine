using Newtonsoft.Json;
using PoESkillTree.Engine.GameModel.PassiveTree.Converters;
using System.ComponentModel;
using System.Drawing;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreeAscendancyClass
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("flavourText", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue("")]
        public string FlavourText { get; set; } = string.Empty;

        [JsonProperty("flavourTextRect", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(PassiveTreeFlavourTextBoundsConverter))]
        public RectangleF FlavourTextBounds { get; set; } = RectangleF.Empty;

        [JsonProperty("flavourTextColour", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonConverter(typeof(PassiveTreeColorConverter))]
        public Color FlavourTextColour { get; set; } = Color.Empty;

        #region Legacy Parsing Purposes
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE1006 // Naming Styles
        [JsonProperty("displayName")]
        private string __displayName { set { Name = value; } }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0051 // Remove unused private members
        #endregion
    }
}
