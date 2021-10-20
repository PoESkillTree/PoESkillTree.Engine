using Newtonsoft.Json;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveNodeMasterEffect
    {
        [JsonProperty("effect")]
        public ushort Effect { get; set; } = 0;

        [JsonProperty("stats")]
        public string[] StatDescriptions { get; set; } = new string[0];
    }
}