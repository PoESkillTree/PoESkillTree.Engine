using Newtonsoft.Json;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreeCharacterData
    {
        [JsonProperty("base_str")]
        public int Strength { get; set; }

        [JsonProperty("base_dex")]
        public int Dexterity { get; set; }

        [JsonProperty("base_int")]
        public int Intelligence { get; set; }
    }
}
