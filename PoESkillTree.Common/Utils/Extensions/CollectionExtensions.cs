﻿using System;
using System.Collections.Generic;

namespace PoESkillTree.Common.Utils.Extensions
{
    public static class CollectionExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, 
            TKey key, Func<TKey, TValue> valueFactory)
        {
            if (!dict.TryGetValue(key, out var value))
            {
                value = valueFactory(key);
                dict[key] = value;
            }
            return value;
        }

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair,
            out TKey key, out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }

        /// <summary>
        /// Applies <paramref name="action"/> to <c>dict[key]</c> if <c>dict.ContainsKey(key)</c>.
        /// </summary>
        public static void ApplyIfPresent<TKey, TValue>(this IDictionary<TKey, TValue> dict,
            TKey key, Action<TValue> action)
        {
            if (dict.TryGetValue(key, out var value))
            {
                action(value);
            }
        }
    }
}