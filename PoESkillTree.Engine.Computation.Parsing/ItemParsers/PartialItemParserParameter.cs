using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Items;

namespace PoESkillTree.Engine.Computation.Parsing.ItemParsers
{
    public class PartialItemParserParameter
    {
        public PartialItemParserParameter(
            Item item, ItemSlot itemSlot, Entity entity, BaseItemDefinition baseItemDefinition,
            ModifierSource.Local localSource, ModifierSource.Global globalSource)
            => (Item, ItemSlot, Entity, BaseItemDefinition, LocalSource, GlobalSource) =
                (item, itemSlot, entity, baseItemDefinition, localSource, globalSource);

        public void Deconstruct(
            out Item item, out ItemSlot itemSlot, out Entity entity, out BaseItemDefinition baseItemDefinition,
            out ModifierSource.Local localSource, out ModifierSource.Global globalSource)
            => (item, itemSlot, entity, baseItemDefinition, localSource, globalSource) =
                (Item, ItemSlot, Entity, BaseItemDefinition, LocalSource, GlobalSource);

        public Item Item { get; }
        public ItemSlot ItemSlot { get; }
        public Entity Entity { get; }
        public BaseItemDefinition BaseItemDefinition { get; }

        public ModifierSource.Local LocalSource { get; }
        public ModifierSource.Global GlobalSource { get; }
    }
}