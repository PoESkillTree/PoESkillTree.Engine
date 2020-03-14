using Newtonsoft.Json;
using PoESkillTree.Engine.GameModel.Items;
using System.Collections.Generic;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreeConstants
    {
        [JsonProperty("classes")]
        public Dictionary<string, CharacterClass> Classes { get; } = new Dictionary<string, CharacterClass>()
        {
            { "StrDexIntClass", CharacterClass.Scion },
            { "StrClass", CharacterClass.Marauder },
            { "DexClass", CharacterClass.Ranger },
            { "IntClass", CharacterClass.Witch },
            { "StrDexClass", CharacterClass.Duelist },
            { "StrIntClass", CharacterClass.Templar },
            { "DexIntClass", CharacterClass.Shadow },
        };

        [JsonProperty("characterAttributes")]
        public Dictionary<string, int> CharacterAttributes { get; } = new Dictionary<string, int>()
        {
            { "Strength", 0 },
            { "Dexterity", 1 },
            { "Intelligence", 2 },
        };

        [JsonProperty("PSSCentreInnerRadius")]
        public int PSSCentreInnerRadius { get; set; } = 130;

        [JsonProperty("skillsPerOrbit")]
        public float[] SkillsPerOrbit { get; } = new float[] { 1f, 6f, 12f, 12f, 40f };

        [JsonProperty("orbitRadii")]
        public float[] OrbitRadii { get; } = new float[] { 0f, 82f, 162f, 335f, 493f };
    }
}
