using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveNode : JsonPassiveTreePosition
    {
        [JsonProperty("id")]
        public ushort Id { get; set; } = 0;

        [JsonProperty("skill")]
        public ushort Skill { get; set; } = 0;

        [JsonProperty("icon")]
        public string Icon { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("ascendancyName", NullValueHandling = NullValueHandling.Ignore)]
        public string? AscendancyName { get; set; }

        [JsonProperty("isNotable", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsNotable { get; set; } = false;

        [JsonProperty("isKeystone", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsKeystone { get; set; } = false;

        [JsonProperty("isMastery", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsMastery { get; set; } = false;

        [JsonProperty("isJewelSocket", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsJewelSocket { get; set; } = false;

        [JsonProperty("isBlighted", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsBlighted { get; set; } = false;

        [JsonProperty("isProxy", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsProxy { get; set; } = false;

        [JsonIgnore]
        private bool? isAscendancyStart;

        [JsonProperty("isAscendancyStart", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsAscendancyStart
        {
            get
            {
                isAscendancyStart ??= Name == AscendancyName;
                return isAscendancyStart.Value;
            }
            set => isAscendancyStart = value;
        }

        [JsonProperty("isMultipleChoice", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsMultipleChoice { get; set; } = false;

        [JsonProperty("isMultipleChoiceOption", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsMultipleChoiceOption { get; set; } = false;

        [JsonProperty("grantedStrength", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Strength { get; set; } = 0;

        [JsonProperty("grantedDexterity", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Dexterity { get; set; } = 0;

        [JsonProperty("grantedIntelligence", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Intelligence { get; set; } = 0;

        [JsonProperty("grantedPassivePoints", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int PassivePointsGranted { get; set; } = 0;

        [JsonProperty("classStartIndex", NullValueHandling = NullValueHandling.Ignore)]
        public CharacterClass? StartingCharacterClass { get; set; }

        [JsonProperty("stats")]
        public string[] StatDescriptions { get; set; } = new string[0];

        [JsonProperty("reminderText")]
        public string[] ReminderText { get; set; } = new string[0];

        [JsonProperty("group", NullValueHandling = NullValueHandling.Ignore)]
        public ushort? PassiveNodeGroupId { get; set; }

        [JsonProperty("orbit")]
        public int OrbitRadiiIndex { get; set; } = 0;

        [JsonProperty("orbitIndex")]
        public int SkillsPerOrbitIndex { get; set; } = 0;

        [JsonProperty("recipe", NullValueHandling = NullValueHandling.Ignore)]
        public string[]? Recipe { get; set; }

        [JsonProperty("expansionJewel", NullValueHandling = NullValueHandling.Ignore)]
        public ExpansionJewel? ExpansionJewel { get; set; }

        [JsonProperty("out")]
        public HashSet<ushort> OutPassiveNodeIds { get; } = new HashSet<ushort>();

        [JsonProperty("in")]
        public HashSet<ushort> InPassiveNodeIds { get; } = new HashSet<ushort>();

        #region Assigned Properties
        [JsonIgnore]
        public bool IsSkilled { get; set; } = false;

        [JsonIgnore]
        public float[] SkillsPerOrbit { get; set; } = new float[] { 1f, 6f, 12f, 12f, 40f };

        [JsonIgnore]
        public float[] OrbitRadii { get; set; } = new float[] { 0f, 82f, 162f, 335f, 493f };

        [JsonIgnore]
        public JsonPassiveNodeGroup? PassiveNodeGroup { get; set; } = null;
        #endregion

        #region Calculated Properties
        [JsonIgnore]
        public bool IsSmall => PassiveNodeType == PassiveNodeType.Small;

        [JsonIgnore]
        public bool IsAscendancyNode => !string.IsNullOrWhiteSpace(AscendancyName);

        [JsonIgnore]
        public bool IsRootNode => StartingCharacterClass is { };

        [JsonIgnore]
        private PassiveNodeType? _passiveNodeType = null;

        [JsonIgnore]
        public PassiveNodeType PassiveNodeType
        {
            get
            {
                if (!_passiveNodeType.HasValue)
                {
                    if (IsKeystone)
                    {
                        _passiveNodeType = PassiveNodeType.Keystone;
                    }
                    else if (IsNotable)
                    {
                        _passiveNodeType = PassiveNodeType.Notable;
                    }
                    else if (IsMastery)
                    {
                        _passiveNodeType = PassiveNodeType.Mastery;
                    }
                    else if (IsJewelSocket)
                    {
                        _passiveNodeType = ExpansionJewel is null ? PassiveNodeType.JewelSocket : PassiveNodeType.ExpansionJewelSocket;
                    }
                    else
                    {
                        _passiveNodeType = PassiveNodeType.Small;
                    }
                }

                return _passiveNodeType.Value;
            }
            set => _passiveNodeType = value;
        }

        [JsonIgnore]
        private double? _arc = null;

        [JsonIgnore]
        public double Arc => _arc ??= 2 * Math.PI * SkillsPerOrbitIndex / SkillsPerOrbit[SkillsPerOrbitIndex];

        [JsonIgnore]
        public override Vector2 Position
        {
            get
            {
                if (!_position.HasValue)
                {
                    if (PassiveNodeGroup is null)
                    {
                        _position = Vector2.Zero;
                    }
                    else
                    {
                        var orbitRadius = OrbitRadii[OrbitRadiiIndex] * ZoomLevel;
                        _position = PassiveNodeGroup.Position - new Vector2(orbitRadius * (float)Math.Sin(-Arc), orbitRadius * (float)Math.Cos(-Arc));
                    }
                }

                return _position.Value;
            }
        }
        #endregion

        #region Legacy Parsing Purposes
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE1006 // Naming Styles
        [JsonProperty("dn")]
        private string __dn { set { Name = value; } }

        [JsonProperty("not")]
        private bool __not { set { IsNotable = value; } }

        [JsonProperty("ks")]
        private bool __ks { set { IsKeystone = value; } }

        [JsonProperty("m")]
        private bool __m { set { IsMastery = value; } }

        [JsonProperty("sa")]
        private int __sa { set { Strength = value; } }

        [JsonProperty("da")]
        private int __da { set { Dexterity = value; } }

        [JsonProperty("ia")]
        private int __ia { set { Intelligence = value; } }

        [JsonProperty("passivePointsGranted")]
        private int __passivePointsGranted { set { PassivePointsGranted = value; } }

        [JsonProperty("spc")]
        private CharacterClass[] __spc { set { StartingCharacterClass = value?.Length > 0 ? value[0] as CharacterClass? : null; } }

        [JsonProperty("sd")]
        private string[]? __sd { set { StatDescriptions = value ?? new string[0]; } }

        [JsonProperty("g")]
        private ushort? __g { set { PassiveNodeGroupId = value; } }

        [JsonProperty("o")]
        private int __o { set { OrbitRadiiIndex = value; } }

        [JsonProperty("oidx")]
        private int __oidx { set { SkillsPerOrbitIndex = value; } }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0051 // Remove unused private members
        #endregion
    }

    public partial class ExpansionJewel
    {
        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("proxy")]
        private ushort ProxyPassiveNodeId { get; set; }

        [JsonProperty("parent", NullValueHandling = NullValueHandling.Ignore)]
        public ushort? ParentPassiveNodeId { get; set; }
    }
}
