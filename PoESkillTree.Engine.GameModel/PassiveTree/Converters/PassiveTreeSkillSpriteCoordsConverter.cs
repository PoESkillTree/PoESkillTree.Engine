using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Linq;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Converters
{
    public class PassiveTreeSkillSpriteCoordsConverter : PassiveTreeRectangleFConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var jObject = new JObject();
            if (value is RectangleF rectangle)
            {
                jObject.Add("x", (int)rectangle.X);
                jObject.Add("y", (int)rectangle.Y);
                jObject.Add("w", (int)rectangle.Width);
                jObject.Add("h", (int)rectangle.Height);
            }

            jObject.WriteTo(writer);
        }
    }
}
