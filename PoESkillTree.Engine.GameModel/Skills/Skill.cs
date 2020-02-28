using System;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.GameModel.Skills
{
    public class Skill : ValueObject
    {
        public static readonly Skill Default = new Skill("PlayerMelee", 1, 0, ItemSlot.Unequipable, 0, null);

        public Skill(string id, int level, int quality, ItemSlot itemSlot, int socketIndex, int? gemGroup, bool isEnabled = true)
            : this(id, level, quality, itemSlot, socketIndex, 0, gemGroup, isEnabled)
        {
            if (level < 1)
                throw new ArgumentOutOfRangeException(nameof(level), level, "Level must be >= 1");
            (Id, Level, Quality, ItemSlot, SocketIndex, GemGroup, IsEnabled) =
                (id, level, quality, itemSlot, socketIndex, gemGroup, isEnabled);
        }

        public Skill(string id, int level, int quality, ItemSlot itemSlot, int socketIndex, int skillIndex, int? gemGroup, bool isEnabled = true)
        {
            if (level < 1)
                throw new ArgumentOutOfRangeException(nameof(level), level, "Level must be >= 1");
            (Id, Level, Quality, ItemSlot, SocketIndex, SkillIndex, GemGroup, IsEnabled) =
                (id, level, quality, itemSlot, socketIndex, skillIndex, gemGroup, isEnabled);
        }

        public string Id { get; }
        public int Level { get; }
        public int Quality { get; }

        public ItemSlot ItemSlot { get; }
        public int SocketIndex { get; }

        /// <summary>
        /// The index of the skill in the same socket. Relevant for gems providing multiple skills, e.g. Vaal gems.
        /// </summary>
        public int SkillIndex { get; }

        /// <summary>
        /// The skill's gem/link group. Null for item-inherent skills.
        /// </summary>
        public int? GemGroup { get; }

        public bool IsEnabled { get; }

        protected override object ToTuple()
            => (Id, Level, Quality, ItemSlot, SocketIndex, SkillIndex, GemGroup, IsEnabled);
    }
}