using Newtonsoft.Json;
using System.Drawing;
using System.Linq;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Options
{
    public class JsonAscendancyClassOption
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonProperty("flavourText")]
        public string FlavourText { get; set; } = string.Empty;

        [JsonProperty("flavourTextRect")]
        public string FlavourTextBoundsString { get; set; } = string.Empty;

        [JsonProperty("flavourTextColour")]
        public string FlavourTextColourString { get; set; } = string.Empty;

        #region Calcuated Properties
        [JsonIgnore]
        private RectangleF? _flavourTextBounds = null;

        [JsonIgnore]
        public RectangleF FlavourTextBounds
        {
            get
            {
                if (!_flavourTextBounds.HasValue)
                {
                    var rect = FlavourTextBoundsString.Split(',').Select(x => float.TryParse(x, out var result) ? result : 0f).ToArray();
                    if (rect.Length == 4)
                    {
                        _flavourTextBounds = new RectangleF(rect[0], rect[1], rect[2], rect[3]);
                    }
                    else
                    {
                        _flavourTextBounds = Rectangle.Empty;
                    }
                }

                return _flavourTextBounds.Value;
            }
        }

        [JsonIgnore]
        private Color? _flavourTextColour = null;

        [JsonIgnore]
        public Color FlavourTextColour
        {
            get
            {
                if (!_flavourTextColour.HasValue)
                {
                    var colour = FlavourTextColourString.Split(',').Select(x => byte.TryParse(x, out var result) ? result : byte.MaxValue).ToArray();
                    if (colour.Length == 3)
                    {
                        _flavourTextColour = Color.FromArgb(colour[0], colour[1], colour[2]);
                    }
                    else
                    {
                        _flavourTextColour = Color.White;
                    }
                }

                return _flavourTextColour.Value;
            }
        }
        #endregion
    }
}
