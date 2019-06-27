using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Equipment;
using PoESkillTree.Engine.GameModel.Items;

namespace PoESkillTree.Engine.Computation.Builders.Equipment
{
    public class EquipmentBuilderCollection
        : FixedBuilderCollection<ItemSlot, IEquipmentBuilder>, IEquipmentBuilderCollection
    {
        private static readonly IReadOnlyList<ItemSlot> Keys = Enums.GetValues<ItemSlot>().ToList();

        public EquipmentBuilderCollection(IStatFactory statFactory)
            : base(Keys, s => new EquipmentBuilder(statFactory, s))
        {
        }
    }
}