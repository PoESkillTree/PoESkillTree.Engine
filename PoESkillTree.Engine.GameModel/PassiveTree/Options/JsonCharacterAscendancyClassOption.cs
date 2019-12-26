using Newtonsoft.Json;
using System.Collections.Generic;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Options
{
    public class JsonCharacterAscendancyClassOption
    {
        [JsonProperty("name")]
        public string CharacterName { get; set; } = string.Empty;

        [JsonProperty]
        public Dictionary<int, JsonAscendancyClassOption> AscendancyClasses { get; } = new Dictionary<int, JsonAscendancyClassOption>();
    }

}
