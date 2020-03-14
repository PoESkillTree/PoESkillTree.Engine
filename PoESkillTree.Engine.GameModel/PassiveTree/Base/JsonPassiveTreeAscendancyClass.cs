using Newtonsoft.Json;
using PoESkillTree.Engine.GameModel.PassiveTree.Converters;
using System.Drawing;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreeAscendancyClass
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonProperty("flavourText")]
        public string FlavourText { get; set; } = string.Empty;

        [JsonProperty("flavourTextRect")]
        [JsonConverter(typeof(PassiveTreeRectangleFConverter))]
        public RectangleF FlavourTextBounds { get; set; } = RectangleF.Empty;

        [JsonProperty("flavourTextColour")]
        [JsonConverter(typeof(PassiveTreeColorConverter))]
        public Color FlavourTextColour { get; set; }

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
