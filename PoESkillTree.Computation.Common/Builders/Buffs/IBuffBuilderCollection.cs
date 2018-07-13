using System.Linq;
using PoESkillTree.Computation.Common.Builders.Entities;
using PoESkillTree.Computation.Common.Builders.Skills;
using PoESkillTree.Computation.Common.Builders.Stats;

namespace PoESkillTree.Computation.Common.Builders.Buffs
{
    /// <summary>
    /// Represents a collection of buffs.
    /// </summary>
    public interface IBuffBuilderCollection : IBuilderCollection
    {
        /// <summary>
        /// Gets a stat representing the effect modifier that is applied to all buffs in this collection.
        /// </summary>
        IStatBuilder Effect { get; }

        /// <summary>
        /// Adds <paramref name="stat"/> to the effects of all skills in this collection. Modifiers with stats built
        /// from the returned stat will apply their value to <paramref name="stat"/> for each buff in this
        /// collection, each application affected by the skill's effect increase.
        /// <para>E.g. "Auras you Cast grant 3% increased Attack and Cast Speed to you and Allies"</para>
        /// </summary>
        IStatBuilder AddStat(IStatBuilder stat);

        /// <summary>
        /// Returns a flag stat indicating whether activating buffs in this collection will also activate them on
        /// the given entity.
        /// </summary>
        IFlagStatBuilder ApplyToEntity(IEntityBuilder target);

        /// <summary>
        /// Returns a new collection that includes all buffs in this collection that originate from any skill
        /// with the keyword <paramref name="keyword"/>.
        /// </summary>
        IBuffBuilderCollection With(IKeywordBuilder keyword);

        /// <summary>
        /// Returns a new collection that includes all buffs in this collection except those that originate from any 
        /// skill with the keyword <paramref name="keyword"/>.
        /// </summary>
        IBuffBuilderCollection Without(IKeywordBuilder keyword);
    }


    public static class BuffBuilderCollectionExtensions
    {
        public static IBuffBuilderCollection With(
            this IBuffBuilderCollection @this, params IKeywordBuilder[] keywords) =>
            keywords.Aggregate(@this, (c, k) => c.With(k));
    }
}