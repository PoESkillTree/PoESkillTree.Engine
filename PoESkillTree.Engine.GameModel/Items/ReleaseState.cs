using System.Runtime.Serialization;

namespace PoESkillTree.Engine.GameModel.Items
{
    public enum ReleaseState
    {
        [EnumMember(Value = "unreleased")]
        Unreleased,
        [EnumMember(Value = "released")]
        Released,
        [EnumMember(Value = "legacy")]
        Legacy,
        [EnumMember(Value = "unique_only")]
        UniqueOnly,
    }
}