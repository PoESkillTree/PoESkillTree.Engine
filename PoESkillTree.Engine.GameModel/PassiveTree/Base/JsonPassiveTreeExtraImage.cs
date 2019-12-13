using Newtonsoft.Json;
using System.Numerics;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreeExtraImage : JsonPassiveTreePosition
    {
        [JsonProperty("image")]
        public string Image { get; set; } = string.Empty;
    }
}
