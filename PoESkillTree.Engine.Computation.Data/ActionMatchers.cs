using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common.Builders.Actions;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Effects;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Base;
using PoESkillTree.Engine.Computation.Data.Collections;

namespace PoESkillTree.Engine.Computation.Data
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
                { "kill(ed|ing)?", Action.Kill },
                { "dealing a killing blow", Action.Kill },
                { "block(ed)?", Action.Block },
                { "blocked attack damage", Action.Block.Attack },
                { "blocked spell damage", Action.Block.Spell },
                { "hit(s|ting)?", Action.Hit },
                { "hit with your main hand weapon", Action.HitWith(AttackDamageHand.MainHand) },
                { "hit with your off hand weapon", Action.HitWith(AttackDamageHand.OffHand) },
                { "(dealt a )?critical strike", Action.CriticalStrike },
                { "non-critical strike", Action.NonCriticalStrike },
                { "stun(ned)?", Effect.Stun.InflictionAction },
                { "shock(ed)?", Effect.Ailment.Shock.InflictionAction },
                { "chill(ed)?", Effect.Ailment.Chill.InflictionAction },
                { "ignite(d)?", Effect.Ailment.Ignite.InflictionAction },
                { "frozen", Effect.Ailment.Freeze.InflictionAction },
            }; // Add
    }
}