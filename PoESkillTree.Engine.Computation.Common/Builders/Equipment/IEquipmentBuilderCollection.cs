using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.GameModel.Items;

namespace PoESkillTree.Engine.Computation.Common.Builders.Equipment
{
    /// <summary>
    /// Represents a collection of equipment slots.
    /// </summary>
    public interface IEquipmentBuilderCollection : IBuilderCollection<IEquipmentBuilder>
    {
        /// <summary>
        /// Gets the equipment of the given slot.
        /// </summary>
        IEquipmentBuilder this[ItemSlot slot] { get; }
    }

    public static class EquipmentBuilderCollectionExtensions
    {
        public static IConditionBuilder IsAnyFlaskActive(this IEquipmentBuilderCollection @this)
            => @this.Flasks().Select(s => s.HasItem).Aggregate((l, r) => l.Or(r));

        public static IEnumerable<IEquipmentBuilder> Flasks(this IEquipmentBuilderCollection @this)
            => ItemSlotExtensions.Flasks.Select(s => @this[s]);
    }
}