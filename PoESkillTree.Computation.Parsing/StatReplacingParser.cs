﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PoESkillTree.Common.Utils;
using PoESkillTree.Computation.Parsing.Data;

namespace PoESkillTree.Computation.Parsing
{
    /// <inheritdoc />
    /// <summary>
    /// Decorating parser that potentially splits the input stat into multiple using a list of
    /// <see cref="StatReplacerData"/>, passes each of those stats to the decorated parser and outputs all results.
    /// <para>The output remaining is created by joining all stats' remaining outputs that are not only whitespace
    /// with newlines.</para>
    /// <para>Parsing is successful if all stats coulc be parsed successfully.</para>
    /// </summary>
    /// <typeparam name="TResult">Type of the decorated parser's results</typeparam>
    public class StatReplacingParser<TResult> : IParser<IReadOnlyList<TResult>>
    {
        private readonly IParser<TResult> _inner;

        private readonly IReadOnlyList<StatReplacerData> _statReplacerData;

        private readonly RegexCache _regexCache = 
            new RegexCache(RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public StatReplacingParser(IParser<TResult> inner, 
            IReadOnlyList<StatReplacerData> statReplacerData)
        {
            _inner = inner;
            _statReplacerData = statReplacerData;
        }

        public bool TryParse(string stat, out string remaining, out IReadOnlyList<TResult> result)
        {
            var ret = true;
            var results = new List<TResult>();
            var remainings = new List<string>();
            foreach (var subStat in GetReplacements(stat))
            {
                ret &= _inner.TryParse(subStat, out var singleRemaining, out var singleResult);
                results.Add(singleResult);
                if (!string.IsNullOrWhiteSpace(singleRemaining))
                {
                    remainings.Add(singleRemaining);
                }
            }
            result = results;
            remaining = string.Join("\n", remainings);
            return ret;
        }

        private IEnumerable<string> GetReplacements(string stat)
        {
            IEnumerable<IEnumerable<string>> allMatches =
                from data in _statReplacerData
                let match = _regexCache["^" + data.OriginalStatRegex + "$"].Match(stat)
                where match.Success
                select data.Replacements.Select(match.Result);
            return allMatches
                .DefaultIfEmpty(new[] { stat })
                .First();
        }
    }
}