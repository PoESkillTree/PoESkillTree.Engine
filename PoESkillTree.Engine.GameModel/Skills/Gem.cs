using System;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.GameModel.Skills
{
    public class Gem : ValueObject
    {
        public Gem(string skillId, int level, int quality, ItemSlot itemSlot, int socketIndex, int group, bool isEnabled)
        {
            if (level < 1)
                throw new ArgumentOutOfRangeException(nameof(level), level, "Level must be >= 1");
            SkillId = skillId;
            Level = level;
            Quality = quality;
            ItemSlot = itemSlot;
            SocketIndex = socketIndex;
            Group = group;
            IsEnabled = isEnabled;
        }

        public string SkillId { get; }
        public int Level { get; }
        public int Quality { get; }

        public ItemSlot ItemSlot { get; }
        public int SocketIndex { get; }

        /// <summary>
        /// The gem's link group.
        /// </summary>
        public int Group { get; }

        public bool IsEnabled { get; }

        protected override object ToTuple()
            => (SkillId, Level, Quality, ItemSlot, SocketIndex, Group, IsEnabled);
    }
}