﻿using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;
using System.Collections.Generic;
using PoESkillTree.Computation.Parsing.Builders;
using PoESkillTree.Computation.Parsing.Builders.Charges;
using PoESkillTree.Computation.Parsing.Data;

namespace PoESkillTree.Computation.Data
{
    public class ChargeTypeMatchers : ReferencedMatchersBase<IChargeTypeBuilder>
    {
        private IChargeTypeBuilders Charge { get; }

        public ChargeTypeMatchers(IChargeTypeBuilders chargeTypeBuilders)
        {
            Charge = chargeTypeBuilders;
        }

        protected override IEnumerable<ReferencedMatcherData<IChargeTypeBuilder>>
            CreateCollection() =>
            new ReferencedMatcherCollection<IChargeTypeBuilder>
            {
                { "endurance charges?", Charge.Endurance },
                { "power charges?", Charge.Power },
                { "endurance charges?", Charge.Endurance },
            };
    }
}