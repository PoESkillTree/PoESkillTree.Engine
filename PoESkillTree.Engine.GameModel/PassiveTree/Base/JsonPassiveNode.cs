using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace PoESkillTree.Engine.GameModel.PassiveTree.Base
{
    public class JsonPassiveNode : JsonPassiveTreePosition
    {
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ushort Id { get; set; } = 0;
        public bool ShouldSerializeId() => false;

        [JsonProperty("skill", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ushort Skill { get; set; } = 0;

        [JsonProperty("icon", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue("")]
        public string Icon { get; set; } = string.Empty;

        [JsonProperty("inactiveIcon", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? InactiveIcon { get; set; } = null;

        [JsonProperty("activeIcon", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? ActiveIcon { get; set; } = null;

        [JsonProperty("activeEffectImage", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? ActiveEffectImage { get; set; } = null;

        [JsonProperty("masteryEffects")]
        public JsonPassiveNodeMasterEffect[] MasteryEffects { get; set; } = new JsonPassiveNodeMasterEffect[0];
        public bool ShouldSerializeMasteryEffects() => MasteryEffects.Length > 0;

        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue("")]
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
        public bool ShouldSerializeStatDescriptions() => !(PassiveNodeGroupId == 0 && StatDescriptions.Length == 0);

        [JsonProperty("reminderText")]
        public string[] ReminderText { get; set; } = new string[0];
        public bool ShouldSerializeReminderText() => ReminderText.Length > 0;

        [JsonProperty("flavourText")]
        public string[] FlavourText { get; set; } = new string[0];
        public bool ShouldSerializeFlavourText() => FlavourText.Length > 0;

        [JsonProperty("group", NullValueHandling = NullValueHandling.Ignore)]
        public ushort? PassiveNodeGroupId { get; set; }

        [JsonProperty("orbit")]
        public int Orbit { get; set; } = 0;
        public bool ShouldSerializeOrbit() => ShouldSerializeOrbitData();

        [JsonProperty("orbitIndex")]
        public int OrbitIndex { get; set; } = 0;
        public bool ShouldSerializeOrbitIndex() => ShouldSerializeOrbitData();

        [JsonProperty("recipe", NullValueHandling = NullValueHandling.Ignore)]
        public string[]? Recipe { get; set; }

        [JsonProperty("expansionJewel", NullValueHandling = NullValueHandling.Ignore)]
        public JsonExpansionJewelSocket? ExpansionJewelSocket { get; set; }

        [JsonIgnore]
        public HashSet<ushort>? _outPassiveNodeIds = null;

        [JsonIgnore]
        public HashSet<ushort> OutPassiveNodeIds
        {
            get
            {
                if (_outPassiveNodeIds == null)
                {
                    _outPassiveNodeIds = new HashSet<ushort>(__out.Select(x => ushort.Parse(x)));
                }

                return _outPassiveNodeIds;
            }
        }

        [JsonProperty("out")]
        private HashSet<string> __out = new HashSet<string>();
        public bool ShouldSerialize__out() => ShouldSerializeOrbitData();

        [JsonIgnore]
        public HashSet<ushort>? _inPassiveNodeIds = null;

        [JsonIgnore]
        public HashSet<ushort> InPassiveNodeIds
        {
            get
            {
                if (_inPassiveNodeIds == null)
                {
                    _inPassiveNodeIds = new HashSet<ushort>(__in.Select(x => ushort.Parse(x)));
                }

                return _inPassiveNodeIds;
            }
        }

        [JsonProperty("in")]
        private HashSet<string> __in = new HashSet<string>();
        public bool ShouldSerialize__in() => ShouldSerializeOrbitData();

        [JsonIgnore]
        public Dictionary<ushort, JsonPassiveNode> NeighborPassiveNodes { get; private set; } = new Dictionary<ushort, JsonPassiveNode>();

        #region Assigned Properties
        [JsonIgnore]
        public bool IsSkilled { get; set; } = false;

        [JsonIgnore]
        public Dictionary<int, float[]> OrbitAngles { get; set; } = new Dictionary<int, float[]>();

        [JsonIgnore]
        public int[] OrbitRadii { get; set; } = new int[] { 0, 82, 162, 335, 493 };

        [JsonIgnore]
        public JsonPassiveNodeGroup? PassiveNodeGroup { get; set; } = null;

        private bool ShouldSerializeOrbitData() => !(Orbit == 0 && OrbitIndex == 0 && OutPassiveNodeIds.Count == 0 && InPassiveNodeIds.Count == 0 && IsBlighted == false);
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
                        _passiveNodeType = ExpansionJewelSocket is null ? PassiveNodeType.JewelSocket : PassiveNodeType.ExpansionJewelSocket;
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
        private float? _arc = null;

        [JsonIgnore]
        public float Arc => _arc ??= OrbitAngles[Orbit][OrbitIndex];

        [JsonIgnore]
        public override Vector2 Position
        {
            get
            {
                if (!_position.HasValue)
                {
                    _position = PositionAtZoomLevel(ZoomLevel);
                }

                return _position.Value;
            }
        }

        public override Vector2 PositionAtZoomLevel(float zoomLevel)
        {
            if (PassiveNodeGroup is null)
            {
                return Vector2.Zero;
            }

            var orbitRadius = OrbitRadii[Orbit] * zoomLevel;
            return PassiveNodeGroup.Position - new Vector2(orbitRadius * MathF.Sin(-Arc), orbitRadius * MathF.Cos(-Arc));
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
        private int __o { set { Orbit = value; } }

        [JsonProperty("oidx")]
        private int __oidx { set { OrbitIndex = value; } }
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0051 // Remove unused private members
        #endregion
    }

    public class JsonExpansionJewelSocket
    {
        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonIgnore]
        private ushort? _proxyPassiveNodeId = null;

        [JsonIgnore]
        private ushort ProxyPassiveNodeId
        {
            get
            {
                if (_proxyPassiveNodeId == null && !string.IsNullOrWhiteSpace(_proxy))
                {
                    _proxyPassiveNodeId = ushort.Parse(_proxy);
                }

                return _proxyPassiveNodeId ?? ushort.MinValue;
            }
        }

        [JsonProperty("proxy")]
        private string _proxy = string.Empty;

        [JsonIgnore]
        private ushort? _parentPassiveNodeId = null;

        [JsonIgnore]
        private ushort? ParentPassiveNodeId
        {
            get
            {
                if (_parentPassiveNodeId == null && !string.IsNullOrWhiteSpace(_parent))
                {
                    _parentPassiveNodeId = ushort.Parse(_parent);
                }

                return _parentPassiveNodeId;
            }
        }

        [JsonProperty("parent", NullValueHandling = NullValueHandling.Ignore)]
        private string? _parent;
    }
}
