using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Equipment;

namespace PoESkillTree.Engine.Computation.Builders.Equipment
{
    public class EquipmentBuilders : IEquipmentBuilders
    {
        private readonly IStatFactory _statFactory;

        public EquipmentBuilders(IStatFactory statFactory) => _statFactory = statFactory;

        public IEquipmentBuilderCollection Equipment => new EquipmentBuilderCollection(_statFactory);
    }
}