using PoESkillTree.Engine.Computation.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Effects;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Builders.Effects
{
    internal class EffectBuilders : StatBuildersBase, IEffectBuilders
    {
        public EffectBuilders(IStatFactory statFactory) : base(statFactory)
        {
            Stun = new StunEffectBuilder(statFactory);
            Knockback = new KnockbackEffectBuilder(statFactory);
            Ailment = new AilmentBuilders(statFactory);
            Ground = new GroundEffectBuilders(statFactory);
        }

        public IStunEffectBuilder Stun { get; }
        public IKnockbackEffectBuilder Knockback { get; }
        public IAilmentBuilders Ailment { get; }
        public IGroundEffectBuilders Ground { get; }
        public IStatBuilder ExpirationModifier => FromIdentity(typeof(int));
    }
}