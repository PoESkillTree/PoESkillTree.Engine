using Newtonsoft.Json;

namespace PoESkillTree.Engine.GameModel.StatTranslation
{
    // The classes used to deserialize RePoE's stat_translations

#pragma warning disable 8618 // Initialization is done through deserialization

    public class JsonStatTranslation
    {
        [JsonProperty("English")]
        public JsonTranslationEntry[] English { get; set; }

        [JsonProperty("ids")]
        public string[] Ids { get; set; }

        [JsonProperty("hidden")]
        public bool IsHidden { get; set; }
    }

    public class JsonTranslationEntry
    {
        [JsonProperty("condition")]
        public JsonCondition[] Condition { get; set; }

        [JsonProperty("format")]
        public Format[] Formats { get; set; }

        [JsonProperty("index_handlers")]
        public IndexHandler[][] IndexHandlers { get; set; }

        [JsonProperty("string")]
        public string FormatString { get; set; }
    }

    public class JsonCondition
    {
        [JsonProperty("min")]
        public int Min { get; set; } = int.MinValue;

        [JsonProperty("max")]
        public int Max { get; set; } = int.MaxValue;

        [JsonProperty("negated")]
        public bool IsNegated { get; set; }
    }

#pragma warning restore
}