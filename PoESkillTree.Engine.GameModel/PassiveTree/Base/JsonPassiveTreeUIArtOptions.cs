using Newtonsoft.Json;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreeUIArtOptions
    {
        [JsonProperty("largeGroupUsesHalfImage")]
        public bool LargeGroupUsesHalfImage { get; set; } = true;
    }
}
