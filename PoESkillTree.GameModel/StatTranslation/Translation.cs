﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using MoreLinq;
using PoESkillTree.GameModel.Items;
using PoESkillTree.Utils.Extensions;

namespace PoESkillTree.GameModel.StatTranslation
{
    /// <summary>
    /// Interface for classes that can translate values to a string
    /// </summary>
    public interface ITranslation
    {
        /// <summary>
        /// Translates the given values.
        /// </summary>
        /// <param name="values">the values to translates</param>
        /// <returns>the translated values. Null if the values should not be translated, e.g. the stat should be 
        /// hidden. This is the case for stats without effect or stats that are not meant to be visible to players.
        /// </returns>
        [CanBeNull]
        string Translate(IReadOnlyList<int> values);
    }

    /// <summary>
    /// Encapsulates a JsonStatTranslation and contains the logic for using it to translate values.
    /// </summary>
    public class Translation : ITranslation
    {
        private readonly JsonStatTranslation _jsonTranslation;

        /// <summary>
        /// The stat ids translated by this instance.
        /// </summary>
        public IReadOnlyList<string> Ids => _jsonTranslation.Ids;

        public Translation(JsonStatTranslation jsonTranslation)
        {
            _jsonTranslation = jsonTranslation;
        }

        /// <summary>
        /// Translates the given values. Each value is taken as the value for the id at its index 
        /// (ids as in <see cref="Ids"/>).
        /// </summary>
        /// <param name="values">the values to translates</param>
        /// <returns>the translated values. Null if the values should not be translated, e.g. the stat should be 
        /// hidden. This is the case for stats without effect or stats that are not meant to be visible to players.
        /// </returns>
        /// <exception cref="ArgumentException">if the number of values does not match the number of 
        /// <see cref="Ids"/></exception>
        public string Translate(IReadOnlyList<int> values)
        {
            if (values.Count != Ids.Count)
                throw new ArgumentException("Number of values does not match number of ids");

            if (values.All(v => v == 0))
            {
                // stats with all values being zero (before applying handlers) have no effect
                return null;
            }
            foreach (var entry in _jsonTranslation.English)
            {
                bool match = entry.Condition
                    .EquiZip(values, (c, v) => c == null || (c.Min <= v && v <= c.Max))
                    .All();
                if (match)
                {
                    var formatInputs = new List<string>();
                    for (var i = 0; i < values.Count; i++)
                    {
                        var value = (double) values[i];
                        foreach (var handler in entry.IndexHandlers[i])
                        {
                            value = handler.Apply(value);
                        }
                        formatInputs.Add(entry.Formats[i].Apply(value));
                    }
                    var suffix = _jsonTranslation.IsHidden
                        ? " " + ItemConstants.HiddenStatSuffix
                        : "";
                    return string.Format(CultureInfo.InvariantCulture, entry.FormatString,
                               formatInputs.ToArray<object>()) + suffix;
                }
            }
            return null;
        }

        /// <summary>
        /// Translates the given values.
        /// </summary>
        /// <param name="idValueDict">dictionary of the values to translate. The keys are the ids the values belong to.
        /// Values for ids not handled by this instance are ignored.</param>
        /// <returns>the translated values. Null if the values should not be translated, e.g. the stat should be 
        /// hidden. This is the case for stats without effect or stats that are not meant to be visible to players.
        /// </returns>
        [CanBeNull]
        public string Translate(IReadOnlyDictionary<string, int> idValueDict)
        {
            var values = new int[Ids.Count];
            for (var i = 0; i < Ids.Count; i++)
            {
                values[i] = idValueDict.GetValueOrDefault(Ids[i]);
            }
            return Translate(values);
        }
    }
}