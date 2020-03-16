using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Converters
{
    public class PassiveTreeColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(Color).IsAssignableFrom(objectType);
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader?.Value is string colour)
            {
                colour = colour.Replace("#", string.Empty).Replace(" ", string.Empty);

                if (colour.Length == 3)
                {
                    colour = Regex.Replace(colour, "([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])", "$1$1$2$2$3$3");
                }

                var rgb = colour.Split(',').Select(x => byte.TryParse(x, out var result) ? result : byte.MaxValue).ToArray();
                if (rgb.Length == 3)
                {
                    return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
                }

                if (colour.Length == 6)
                {
                    colour = $"FF{colour}";
                }

                if (int.TryParse(colour, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var hexColour))
                {
                    return Color.FromArgb(hexColour);
                }
            }

            return Color.Empty;
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException($"{nameof(CanWrite)} should be false (is {CanWrite}). There is no need for write converter.");
    }
}
