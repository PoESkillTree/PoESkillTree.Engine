using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Linq;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Converters
{
    public class PassiveTreeFlavourTextBoundsConverter : PassiveTreeRectangleFConverter
    {
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
