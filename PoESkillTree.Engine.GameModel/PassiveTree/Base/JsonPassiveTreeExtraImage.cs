using Newtonsoft.Json;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreeExtraImage : JsonPassiveTreePosition
    {
        [JsonProperty("image")]
        public string Image { get; set; } = string.Empty;
    }
}
