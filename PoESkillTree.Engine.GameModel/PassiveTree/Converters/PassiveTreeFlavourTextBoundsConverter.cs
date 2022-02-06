using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Converters
{
    public class PassiveTreeFlavourTextBoundsConverter : PassiveTreeRectangleFConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var jObject = new JObject();
            if (value is RectangleF rectangle)
            {
                jObject.Add("x", (int)rectangle.X);
                jObject.Add("y", (int)rectangle.Y);
                jObject.Add("width", (int)rectangle.Width);
                jObject.Add("height", (int)rectangle.Height);
            }

            jObject.WriteTo(writer);
        }
    }
}
