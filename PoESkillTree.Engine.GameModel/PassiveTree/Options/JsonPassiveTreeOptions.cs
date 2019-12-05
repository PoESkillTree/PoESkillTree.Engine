using Newtonsoft.Json;
using PoESkillTree.Engine.GameModel.Items;
using System.Collections.Generic;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Options
{
    public class JsonPassiveTreeOptions
    {
        [JsonProperty("ascClasses")]
        public Dictionary<CharacterClass, JsonCharacterAscendancyClassOption> CharacterAscendancyClasses { get; set; } = new Dictionary<CharacterClass, JsonCharacterAscendancyClassOption>();

        [JsonProperty("zoomLevels")]
        public float[] ZoomLevels { get; set; } = new float[] { 0.1246f, 0.2109f, 0.2972f, 0.3835f };

        [JsonProperty("height")]
        public float Height { get; set; } = 767;

        [JsonProperty("startClass")]
        public CharacterClass StartClass { get; set; } = CharacterClass.Witch;

        [JsonProperty("fullScreen")]
        public bool FullScreen { get; set; } = false;

        [JsonProperty("version")]
        public string Version { get; set; } = string.Empty;

        [JsonProperty("realm")]
        public Realm Realm { get; set; } = Realm.PC;

        [JsonProperty("build")]
        public object? Build { get; set; } = null;

        [JsonProperty("circles")]
        public Dictionary<JewelRadius, JsonJewelHighlight> JewelHighlight { get; set; } = new Dictionary<JewelRadius, JsonJewelHighlight>();
    }
}
