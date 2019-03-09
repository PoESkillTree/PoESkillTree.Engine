﻿using System.Collections.Generic;
using PoESkillTree.Computation.Common.Builders.Actions;
using PoESkillTree.Computation.Common.Builders.Effects;
using PoESkillTree.Computation.Common.Data;
using PoESkillTree.Computation.Data.Base;
using PoESkillTree.Computation.Data.Collections;

namespace PoESkillTree.Computation.Data
{
    /// <summary>
    /// <see cref="IReferencedMatchers"/> implementation for <see cref="IActionBuilder"/>s.
    /// </summary>
    public class ActionMatchers : ReferencedMatchersBase<IActionBuilder>
    {
        private IActionBuilders Action { get; }
        private IEffectBuilders Effect { get; }

        public ActionMatchers(IActionBuilders actionBuilders, IEffectBuilders effect)
        {
            Action = actionBuilders;
            Effect = effect;
        }

        protected override IReadOnlyList<ReferencedMatcherData> CreateCollection() =>
            new ReferencedMatcherCollection<IActionBuilder>
            {
                { "kill(ed)?", Action.Kill },
                { "killing", Action.Kill },
                { "dealing a killing blow", Action.Kill },
                { "block(ed)?", Action.Block },
                { "hits?", Action.Hit },
                { "(dealt a )?critical strike", Action.CriticalStrike },
                { "non-critical strike", Action.NonCriticalStrike },
                { "stun", Effect.Stun.InflictionAction },
                { "shocked", Effect.Ailment.Shock.InflictionAction },
            }; // Add
    }
}