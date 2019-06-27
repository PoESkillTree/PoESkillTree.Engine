using PoESkillTree.Engine.Computation.Common.Builders.Charges;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Base;
using PoESkillTree.Engine.Computation.Data.Collections;
using System.Collections.Generic;

namespace PoESkillTree.Engine.Computation.Data
{
    /// <summary>
    /// <see cref="IReferencedMatchers"/> implementation for <see cref="IChargeTypeBuilder"/>s.
    /// </summary>
    public class ChargeTypeMatchers : ReferencedMatchersBase<IChargeTypeBuilder>
    {
        private IChargeTypeBuilders Charge { get; }

        public ChargeTypeMatchers(IChargeTypeBuilders chargeTypeBuilders)
        {
            Charge = chargeTypeBuilders;
        }

        protected override IReadOnlyList<ReferencedMatcherData> CreateCollection() =>
            new ReferencedMatcherCollection<IChargeTypeBuilder>
            {
                { "endurance charges?", Charge.Endurance },
                { "frenzy charges?", Charge.Frenzy },
                { "power charges?", Charge.Power },
                { "rage", Charge.Rage },
                { "ghost shrouds?", Charge.From(ChargeType.GhostShroud) },
                { "intensity", Charge.From(ChargeType.Intensity) },
                { "challenger charges?", Charge.From(ChargeType.Challenger) },
                { "blitz charges?", Charge.From(ChargeType.Blitz) },
            };
    }
}