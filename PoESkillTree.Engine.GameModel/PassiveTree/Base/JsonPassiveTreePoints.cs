using Newtonsoft.Json;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreePoints
    {
        [JsonProperty("ascendancyPoints")]
        public int AscendancyPoints { get; set; } = 8;

        [JsonProperty("totalPoints")]
        public int TotalPoints { get; set; } = 123;
    }
}
