using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveNode
    {
        [JsonProperty("id")]
        public ushort Id { get; set; } = 0;

        [JsonProperty("icon")]
        public string Icon { get; set; } = string.Empty;

        [JsonProperty("dn")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("ascendancyName")]
        public string AscendancyName { get; set; } = string.Empty;

        [JsonProperty("not")]
        public bool IsNotable { get; set; } = false;

        [JsonProperty("ks")]
        public bool IsKeystone { get; set; } = false;

        [JsonProperty("m")]
        public bool IsMastery { get; set; } = false;

        [JsonProperty("isJewelSocket")]
        public bool IsJewelSocket { get; set; } = false;

        [JsonProperty("isBlighted")]
        public bool IsBlighted { get; set; } = false;

        [JsonProperty("isAscendancyStart")]
        public bool IsAscendancyStart { get; set; } = false;

        [JsonProperty("isMultipleChoice")]
        public bool IsMultipleChoice { get; set; } = false;

        [JsonProperty("isMultipleChoiceOption")]
        public bool IsMultipleChoiceOption { get; set; } = false;

        [JsonProperty("sa")]
        public int Strenth { get; set; } = 0;

        [JsonProperty("da")]
        public int Dexterity { get; set; } = 0;

        [JsonProperty("ia")]
        public int Intelligence { get; set; } = 0;

        [JsonProperty("passivePointsGranted")]
        public int PassivePointsGranted { get; set; } = 0;

        [JsonProperty("spc")]
        public CharacterClass[] StartingCharacterClasses { get; set; } = new CharacterClass[0];

        [JsonProperty("sd")]
        public string[] StatDescriptions { get; set; } = new string[0];

        [JsonProperty("reminderText")]
        public string[] ReminderText { get; set; } = new string[0];

        [JsonProperty("g")]
        public ushort PassiveNodeGroupId { get; set; } = 0;

        [JsonProperty("o")]
        public int OrbitRadiiIndex { get; set; } = 0;

        [JsonProperty("oidx")]
        public int SkillsPerOrbitIndex { get; set; } = 0;

        [JsonProperty("out")]
        public HashSet<ushort> PassiveNodeOutIds { get; set; } = new HashSet<ushort>();

        [JsonProperty("in")]
        public HashSet<ushort> PassiveNodeInIds { get; set; } = new HashSet<ushort>();

        #region Assigned Properties
        [JsonIgnore]
        public float[] SkillsPerOrbit { get; set; } = new float[] { 1f, 6f, 12f, 12f, 40f };

        [JsonIgnore]
        public float[] OrbitRadii { get; set; } = new float[] { 0f, 82f, 162f, 335f, 493f };

        [JsonIgnore]
        public JsonPassiveNodeGroup? PassiveNodeGroup { get; set; } = null;
        #endregion

        #region Calculated Properties
        [JsonIgnore]
        public bool IsSmall => !IsKeystone && !IsNotable && !IsMastery && !IsJewelSocket;

        [JsonIgnore]
        public bool IsAscendancyNode => !string.IsNullOrWhiteSpace(AscendancyName);

        [JsonIgnore]
        public CharacterClass? StartingCharacterClass => StartingCharacterClasses.Length > 0 ? StartingCharacterClasses[0] as CharacterClass? : null;

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
                        _passiveNodeType = PassiveNodeType.JewelSocket;
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
        public double Arc => 2 * Math.PI * SkillsPerOrbitIndex / SkillsPerOrbit[OrbitRadiiIndex];

        [JsonIgnore]
        private Vector2? _position = null;

        [JsonIgnore]
        public Vector2 Position
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
                        _position = PassiveNodeGroup.Position - new Vector2(OrbitRadii[OrbitRadiiIndex] * (float)Math.Sin(-Arc), OrbitRadii[OrbitRadiiIndex] * (float)Math.Cos(-Arc));
                    }
                }

                return _position.Value;
            }
        }
        #endregion
    }
}
