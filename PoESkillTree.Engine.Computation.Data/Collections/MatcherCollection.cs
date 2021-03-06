using System.Collections;
using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common.Builders.Modifiers;
using PoESkillTree.Engine.Computation.Common.Data;

namespace PoESkillTree.Engine.Computation.Data.Collections
{
    /// <summary>
    /// Collection of <see cref="MatcherData"/> that allows collection initialization syntax for adding entries.
    /// See <see cref="StatReplacerData"/> for documentation of the Add methods' parameters.
    /// <para>Subclasses provide Add that create <see cref="IModifierBuilder"/> instances from their parameters.</para>
    /// </summary>
    public abstract class MatcherCollection : IReadOnlyList<MatcherData>
    {
        /// <summary>
        /// An empty <see cref="IModifierBuilder"/> to build others from.
        /// </summary>
        protected IModifierBuilder ModifierBuilder { get; }

        private readonly List<MatcherData> _matchers = new List<MatcherData>();

        protected MatcherCollection(IModifierBuilder modifierBuilder)
            => ModifierBuilder = modifierBuilder;

        protected void Add(string regex, IModifierBuilder modifierBuilder, string matchSubstitution = "")
            => _matchers.Add(new MatcherData(regex, modifierBuilder.Build(), matchSubstitution));

        public IEnumerator<MatcherData> GetEnumerator() => _matchers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _matchers.Count;

        public MatcherData this[int index] => _matchers[index];
    }
}