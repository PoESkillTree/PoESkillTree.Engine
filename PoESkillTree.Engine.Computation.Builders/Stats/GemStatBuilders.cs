using System;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Skills;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Parsing;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Builders.Stats
{
    public class GemStatBuilders : StatBuildersBase, IGemStatBuilders
    {
        public GemStatBuilders(IStatFactory statFactory) : base(statFactory)
        {
        }

        public IStatBuilder AdditionalActiveLevels(IGemTagBuilder gemTag) => AdditionalLevels(".ActiveSkill", gemTag);

        public IStatBuilder AdditionalActiveSpellLevels(IGemTagBuilder gemTag) => AdditionalLevels(".ActiveSkill.spell", gemTag);

        private IStatBuilder AdditionalLevels(string identityInfix, IGemTagBuilder gemTag) =>
            AdditionalLevels(identityInfix, gemTag, (_, i) => i);

        public IStatBuilder AdditionalLevelsForModifierSourceItemSlot() =>
            AdditionalLevels("", GetItemSlot);

        public IStatBuilder AdditionalLevelsForModifierSourceItemSlot(IGemTagBuilder gemTag) =>
            AdditionalLevels("", gemTag, (ps, t) => $"{t}.{GetItemSlot(ps.ModifierSource)}");

        public IStatBuilder AdditionalActiveLevelsForModifierSourceItemSlot() =>
            AdditionalLevels(".ActiveSkill", GetItemSlot);

        private IStatBuilder AdditionalLevels(string identityInfix, IGemTagBuilder gemTag, Func<BuildParameters, string, string> buildIdentitySuffix)
        {
            var coreBuilder = new CoreStatBuilderFromCoreBuilder<string>(
                CoreBuilder.Proxy(gemTag, (ps, b) => buildIdentitySuffix(ps, b.Build(ps))),
                (e, t) => StatFactory.FromIdentity($"Gem.AdditionalLevels{identityInfix}.{t}", e, typeof(int)));
            return new StatBuilder(StatFactory, coreBuilder);
        }

        private IStatBuilder AdditionalLevels(string identityInfix, Func<ModifierSource, string> buildIdentitySuffix) =>
            Additional("Levels" + identityInfix, buildIdentitySuffix);

        public IStatBuilder AdditionalLevels(Skill skill) =>
            FromIdentity($"Skill.AdditionalLevels.{skill.ItemSlot}.{skill.SocketIndex}.{skill.SkillIndex}", typeof(int));


        public IStatBuilder AdditionalQualityForModifierSourceItemSlot =>
            Additional("Quality", GetItemSlot);

        public IStatBuilder AdditionalSupportQualityForModifierSourceItemSlot =>
            Additional("Quality.SupportSkill", GetItemSlot);

        public IStatBuilder AdditionalQuality(Skill skill) =>
            FromIdentity($"Skill.AdditionalQuality.{skill.ItemSlot}.{skill.SocketIndex}.{skill.SkillIndex}", typeof(int));


        public IStatBuilder IncreasedReservationForModifierSourceItemSlot =>
            CreateSourceDependentStat("IncreasedReservation", GetItemSlot);

        public IStatBuilder IncreasedReservationForItemSlot(ItemSlot itemSlot) =>
            FromIdentity($"IncreasedReservation.{itemSlot}", typeof(int));

        public IStatBuilder IncreasedNonCurseAuraEffectForModifierSourceItemSlot =>
            CreateSourceDependentStat("IncreasedNonCurseAuraEffect", GetItemSlot);

        public IStatBuilder IncreasedNonCurseAuraEffectForItemSlot(ItemSlot itemSlot) =>
            FromIdentity($"IncreasedNonCurseAuraEffect.{itemSlot}", typeof(int));


        private static string GetItemSlot(ModifierSource source) =>
            (source.GetLocalSource() switch
            {
                ModifierSource.Local.Item itemSource => itemSource.Slot,
                ModifierSource.Local.Gem gemSource => gemSource.SourceGem.ItemSlot,
                _ => throw new ParseException($"ModifierSource must be Item or Gem, {source} given")
            }).ToString();

        private IStatBuilder Additional(string identityInfix, Func<ModifierSource, string> buildIdentitySuffix) =>
            CreateSourceDependentStat($"Gem.Additional{identityInfix}", buildIdentitySuffix);

        private IStatBuilder CreateSourceDependentStat(string identityPrefix, Func<ModifierSource, string> buildIdentitySuffix)
        {
            var coreBuilder = new StatBuilderWithStatConverter(
                new LeafCoreStatBuilder(e => StatFactory.FromIdentity(identityPrefix, e, typeof(int))),
                (m, s) => StatFactory.CopyWithSuffix(s, buildIdentitySuffix(m), typeof(int)));
            return new StatBuilder(StatFactory, coreBuilder);
        }
    }
}