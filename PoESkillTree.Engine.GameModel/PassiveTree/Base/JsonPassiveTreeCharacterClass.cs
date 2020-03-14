using Newtonsoft.Json;
using System.Collections.Generic;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreeCharacterClass
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("base_str")]
        public int Strength { get; set; }

        [JsonProperty("base_dex")]
        public int Dexterity { get; set; }

        [JsonProperty("base_int")]
        public int Intelligence { get; set; }

        [JsonProperty("ascendancies")]
        public List<JsonPassiveTreeAscendancyClass> AscendancyClasses { get; set; } = new List<JsonPassiveTreeAscendancyClass>();
    }
}
