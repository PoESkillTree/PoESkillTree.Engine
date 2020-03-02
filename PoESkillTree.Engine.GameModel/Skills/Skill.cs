using System;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.Utils;

namespace PoESkillTree.Engine.GameModel.Skills
{
    public class Skill : ValueObject
    {
        private Skill(string id, int level, int quality, ItemSlot itemSlot, int socketIndex, int skillIndex, Gem? gem, bool isEnabled)
        {
            if (level < 1)
                throw new ArgumentOutOfRangeException(nameof(level), level, "Level must be >= 1");
            (Id, Level, Quality, ItemSlot, SocketIndex, SkillIndex, Gem, IsEnabled) =
                (id, level, quality, itemSlot, socketIndex, skillIndex, gem, isEnabled);
        }

        public static readonly Skill Default = new Skill("PlayerMelee", 1, 0, ItemSlot.Unequipable, -1, 0, null, true);

        public static Skill FromGem(Gem gem, int skillIndex, bool isEnabled)
            => new Skill(gem.SkillId, gem.Level, gem.Quality, gem.ItemSlot, gem.SocketIndex, skillIndex, gem, isEnabled);

        public static Skill FromItem(string id, int level, int quality, ItemSlot itemSlot, int skillIndex, bool isEnabled)
            => new Skill(id, level, quality, itemSlot, -1, skillIndex, null, isEnabled);

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
        /// The skill's source gem. Null for item-inherent skills.
        /// </summary>
        public Gem? Gem { get; }

        public bool IsEnabled { get; }

        protected override object ToTuple()
            => (Id, Level, Quality, ItemSlot, SocketIndex, SkillIndex, Gem, IsEnabled);
    }
}