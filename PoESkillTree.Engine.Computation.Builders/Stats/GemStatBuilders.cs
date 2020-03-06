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
            AdditionalLevels("", m => GetItemSlot(m).ToString());

        public IStatBuilder AdditionalLevelsForModifierSourceItemSlot(IGemTagBuilder gemTag) =>
            AdditionalLevels("", gemTag, (ps, t) => $"{t}.{GetItemSlot(ps.ModifierSource)}");

        public IStatBuilder AdditionalActiveLevelsForModifierSourceItemSlot() =>
            AdditionalLevels(".ActiveSkill", m => GetItemSlot(m).ToString());

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
            FromIdentity($"Skill.AdditionalLevels.{skill.ItemSlot}.{skill.SocketIndex}.{skill.SkillIndex}", typeof(int),
                ExplicitRegistrationTypes.ChangeInvalidatesSkillParse(skill));


        public IStatBuilder AdditionalQualityForModifierSourceItemSlot =>
            Additional("Quality", m => GetItemSlot(m).ToString());

        public IStatBuilder AdditionalSupportQualityForModifierSourceItemSlot =>
            Additional("Quality.SupportSkill", m => GetItemSlot(m).ToString());

        public IStatBuilder AdditionalQuality(Skill skill) =>
            FromIdentity($"Skill.AdditionalQuality.{skill.ItemSlot}.{skill.SocketIndex}.{skill.SkillIndex}", typeof(int),
                ExplicitRegistrationTypes.ChangeInvalidatesSkillParse(skill));


        private static ItemSlot GetItemSlot(ModifierSource source) =>
            source.GetLocalSource() switch
            {
                ModifierSource.Local.Item itemSource => itemSource.Slot,
                ModifierSource.Local.Gem gemSource => gemSource.SourceGem.ItemSlot,
                _ => throw new ParseException($"ModifierSource must be Item or Gem, {source} given")
            };

        private IStatBuilder Additional(string identityInfix, Func<ModifierSource, string> buildIdentitySuffix)
        {
            var coreBuilder = new StatBuilderWithStatConverter(
                new LeafCoreStatBuilder(e => StatFactory.FromIdentity($"Gem.Additional{identityInfix}", e, typeof(int))),
                (m, s) => StatFactory.CopyWithSuffix(s, buildIdentitySuffix(m), typeof(int)));
            return new StatBuilder(StatFactory, coreBuilder);
        }
    }
}