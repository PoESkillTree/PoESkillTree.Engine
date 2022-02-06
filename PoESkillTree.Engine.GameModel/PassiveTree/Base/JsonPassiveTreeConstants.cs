using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveTreeConstants
    {
        [JsonProperty("classes")]
        public Dictionary<string, CharacterClass> Classes { get; set; } = new Dictionary<string, CharacterClass>();

        [JsonProperty("characterAttributes")]
        public Dictionary<string, int> CharacterAttributes { get; set; } = new Dictionary<string, int>();

        [JsonProperty("PSSCentreInnerRadius")]
        public int PSSCentreInnerRadius { get; set; } = 130;

        [JsonProperty("skillsPerOrbit")]
        public int[] SkillsPerOrbit { get; private set; } = new int[] { 1, 6, 12, 12, 40 };

        [JsonProperty("orbitRadii")]
        public int[] OrbitRadii { get; private set; } = new int[] { 0, 82, 162, 335, 493 };

        [JsonIgnore]
        public Dictionary<int, float[]> OrbitAngles
        {
            get
            {
                var angles = new Dictionary<int, float[]>();

                for (var orbit = 0; orbit < SkillsPerOrbit.Length; orbit++)
                {
                    var skillsPerOrbit = SkillsPerOrbit[orbit];
                    angles[orbit] = skillsPerOrbit switch
                    {
                        16 => new float[] { 0f, 30f, 45f, 60f, 90f, 120f, 135f, 150f, 180f, 210f, 225f, 240f, 270f, 300f, 315f, 330f },
                        40 => new float[] { 0f, 10f, 20f, 30f, 40f, 45f, 50f, 60f, 70f, 80f, 90f, 100f, 110f, 120f, 130f, 135f, 140f, 150f, 160f, 170f, 180f, 190f, 200f, 210f, 220f, 225f, 230f, 240f, 250f, 260f, 270f, 280f, 290f, 300f, 310f, 315f, 320f, 330f, 340f, 350f },
                        _ => Enumerable.Range(0, skillsPerOrbit).Select(x => 360f * x / skillsPerOrbit).ToArray()
                    };
                }

                var radians = new Dictionary<int, float[]>();
                foreach (var (orbit, degrees) in angles)
                {
                    radians[orbit] = degrees.Select(x => x * (MathF.PI / 180f)).ToArray();
                }

                return radians;
            }
        }
    }
}
