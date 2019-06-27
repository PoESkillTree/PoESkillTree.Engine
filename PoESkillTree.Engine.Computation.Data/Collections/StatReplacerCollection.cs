using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using PoESkillTree.Engine.Computation.Common.Data;

namespace PoESkillTree.Engine.Computation.Data.Collections
{
    /// <summary>
    /// Collection of <see cref="StatReplacerData"/> that allows collection initialization syntax for adding entries.
    /// See <see cref="StatReplacerData"/> for documentation of <see cref="Add"/>'s parameters.
    /// </summary>
    public class StatReplacerCollection : IEnumerable<StatReplacerData>
    {
        private readonly List<StatReplacerData> _data = new List<StatReplacerData>();

        public IEnumerator<StatReplacerData> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add([RegexPattern] string originalStat, params string[] replacements)
            => _data.Add(new StatReplacerData(originalStat, replacements));
    }
}