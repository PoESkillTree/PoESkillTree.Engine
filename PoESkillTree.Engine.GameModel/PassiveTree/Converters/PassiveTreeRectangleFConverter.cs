using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Converters
{
    public class PassiveTreeRectangleFConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(RectangleF).IsAssignableFrom(objectType);
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var rect = ((string?)reader.Value)?.Split(',').Select(x => float.TryParse(x, out var result) ? result : 0f).ToArray();
                if (rect?.Length == 4)
                {
                    return new RectangleF(rect[0], rect[1], rect[2], rect[3]);
                }
            }
            else
            {
                var jObject = JObject.Load(reader);
                var (x, y, width, height) = (jObject["x"]?.ToObject<float?>(), jObject["y"]?.ToObject<float?>(), (jObject["w"] ?? jObject["width"])?.ToObject<float?>(), (jObject["h"] ?? jObject["height"])?.ToObject<float?>());
                if (x.HasValue && y.HasValue && width.HasValue && height.HasValue)
                {
                    return new RectangleF(x.Value, y.Value, width.Value, height.Value);
                }
            }

            return RectangleF.Empty;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var jObject = new JObject();
            if (value is RectangleF rectangle)
            {
                jObject.Add("x", rectangle.X);
                jObject.Add("y", rectangle.Y);
                jObject.Add("width", rectangle.Width);
                jObject.Add("height", rectangle.Height);
            }

            jObject.WriteTo(writer);
        }
    }
}
