using System.Runtime.Serialization;

namespace PoESkillTree.Engine.GameModel
{
    public enum Realm
    {
        [EnumMember(Value = "pc")]
        PC,
        [EnumMember(Value = "xbox")]
        Xbox,
        [EnumMember(Value = "sony")]
        Sony,
    }
}
