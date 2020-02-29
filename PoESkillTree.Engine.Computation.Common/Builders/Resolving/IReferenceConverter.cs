using PoESkillTree.Engine.Computation.Common.Builders.Actions;
using PoESkillTree.Engine.Computation.Common.Builders.Buffs;
using PoESkillTree.Engine.Computation.Common.Builders.Charges;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Effects;
using PoESkillTree.Engine.Computation.Common.Builders.Skills;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;

namespace PoESkillTree.Engine.Computation.Common.Builders.Resolving
{
    /// <summary>
    /// Converts objects referenced from other matcher collections to their types.
    /// <para>Because getting the actual referenced objects requires matching the stat line, these methods can not
    /// throw exceptions on invalid casts when first called. Exceptions will only be thrown once the context has been
    /// resolved.</para>
    /// </summary>
    public interface IReferenceConverter
    {
        IDamageTypeBuilder AsDamageType { get; }

        IChargeTypeBuilder AsChargeType { get; }

        IAilmentBuilder AsAilment { get; }

        IKeywordBuilder AsKeyword { get; }

        IActionBuilder AsAction { get; }

        IStatBuilder AsStat { get; }

        IPoolStatBuilder AsPoolStat { get; }

        IBuffBuilder AsBuff { get; }

        ISkillBuilder AsSkill { get; }

        IGemTagBuilder AsGemTag { get; }
    }
}