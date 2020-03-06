using PoESkillTree.Engine.Computation.Common.Builders.Skills;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Common.Builders.Stats
{
    /// <summary>
    /// Factory interface for stats related to gems.
    /// </summary>
    public interface IGemStatBuilders
    {
        IStatBuilder AdditionalActiveLevels(IGemTagBuilder gemTag);
        IStatBuilder AdditionalActiveSpellLevels(IGemTagBuilder gemTag);

        IStatBuilder AdditionalLevelsForModifierSourceItemSlot();
        IStatBuilder AdditionalLevelsForModifierSourceItemSlot(IGemTagBuilder gemTag);
        IStatBuilder AdditionalActiveLevelsForModifierSourceItemSlot();

        IStatBuilder AdditionalLevels(Skill skill);


        IStatBuilder AdditionalQualityForModifierSourceItemSlot { get; }
        IStatBuilder AdditionalSupportQualityForModifierSourceItemSlot { get; }

        IStatBuilder AdditionalQuality(Skill skill);


        IStatBuilder IncreasedReservationForModifierSourceItemSlot { get; }
        IStatBuilder IncreasedReservationForItemSlot(ItemSlot itemSlot);

        IStatBuilder IncreasedNonCurseAuraEffectForModifierSourceItemSlot { get; }
        IStatBuilder IncreasedNonCurseAuraEffectForItemSlot(ItemSlot itemSlot);
    }
}