﻿using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.Computation.Common.Builders.Values;
using PoESkillTree.GameModel;
using PoESkillTree.Utils.Extensions;

namespace PoESkillTree.Computation.Common.Builders.Modifiers
{
    /// <summary>
    /// Extension methods on <see cref="IIntermediateModifier"/> that allow merging multiple together and building
    /// one to a list of <see cref="Modifier"/>s.
    /// </summary>
    public static class IntermediateModifierExtensions
    {
        /// <summary>
        /// Merges <paramref name="left"/> with <paramref name="right"/> and returns the result.
        /// </summary>
        /// <remarks>
        /// The converters are merged by creating new converters that successively apply both. The entries of
        /// <paramref name="left"/> are merged with the entries of <paramref name="right"/>.
        /// <para> Two entries are merged by anding the conditions and taking the non-null form, stat and value,
        /// i.e. two merged entries may not both have forms, stats or values.
        /// </para>
        /// <para> Which entries are merged depends on the number of entries in <paramref name="left"/> and
        /// <paramref name="right"/>.
        /// <list type="bullet">
        /// <item>Both have more than one entry: An <see cref="ArgumentException"/> is thrown.</item>
        /// <item>One has no entries: The result has no entries.</item>
        /// <item>Otherwise: The single entry of one <see cref="IIntermediateModifier"/>
        /// is merged with every entry of the other <see cref="IIntermediateModifier"/>, i.e. the result has as much
        /// entries as the other <see cref="IIntermediateModifier"/>.</item>
        /// </list>
        /// </para>
        /// </remarks>
        public static IIntermediateModifier MergeWith(this IIntermediateModifier left, IIntermediateModifier right)
        {
            IStatBuilder ConvertStat(IStatBuilder s) =>
                right.StatConverter(left.StatConverter(s));

            IValueBuilder ConvertValue(IValueBuilder v) =>
                right.ValueConverter(left.ValueConverter(v));

            if (left.Entries.Count > 1 && right.Entries.Count > 1)
            {
                // This simple solution will fail once there are stat lines that parse into multiple stats
                // and have a condition requiring splitting by main hand and off hand.
                throw new ArgumentException("There may only be one IIntermediateModifier with multiple entries");
            }

            IEnumerable<IntermediateModifierEntry> entries;
            if (left.Entries.IsEmpty())
            {
                entries = right.Entries;
            }
            else if (right.Entries.IsEmpty())
            {
                entries = left.Entries;
            }
            else
            {
                entries = left.Entries.SelectMany(l => right.Entries.Select(r => Merge(l, r)));
            }
            return new SimpleIntermediateModifier(entries.ToList(), ConvertStat, ConvertValue);
        }

        private static IntermediateModifierEntry Merge(IntermediateModifierEntry left,
            IntermediateModifierEntry right)
        {
            if (left.Form != null && right.Form != null)
                throw new ArgumentException("Form may only be set once");
            if (left.Stat != null && right.Stat != null)
                throw new ArgumentException("Stat may only be set once");
            if (left.Value != null && right.Value != null)
                throw new ArgumentException("Value may only be set once");

            IConditionBuilder condition;
            if (left.Condition == null)
            {
                condition = right.Condition;
            }
            else if (right.Condition == null)
            {
                condition = left.Condition;
            }
            else
            {
                condition = left.Condition.And(right.Condition);
            }

            return new IntermediateModifierEntry()
                .WithForm(left.Form ?? right.Form)
                .WithStat(left.Stat ?? right.Stat)
                .WithValue(left.Value ?? right.Value)
                .WithCondition(condition);
        }

        /// <summary>
        /// Aggregates the <paramref name="modifiers"/> using <see cref="MergeWith"/> and an empty
        /// <see cref="IIntermediateModifier"/> as seed.
        /// </summary>
        /// <remarks>
        /// The same rules as in <see cref="MergeWith"/> apply, i.e. only one <see cref="IIntermediateModifier"/>
        /// may have more than one entry, entries with form set, entries with stat set and entries with value set.
        /// </remarks>
        public static IIntermediateModifier Aggregate(this IEnumerable<IIntermediateModifier> modifiers)
            => modifiers.Aggregate(SimpleIntermediateModifier.Empty, MergeWith);

        /// <summary>
        /// Builds <paramref name="modifier"/> by creating a <see cref="Modifier"/> from each of its entries.
        /// The <see cref="IIntermediateModifier.StatConverter"/> is applied to each stat and
        /// <see cref="IIntermediateModifier.ValueConverter"/> to each value. Entries with null form, stat or value
        /// are ignored.
        /// </summary>
        public static IReadOnlyList<Modifier> Build(
            this IIntermediateModifier modifier, ModifierSource originalSource, Entity modifierSourceEntity)
            => new IntermediateModifierBuilder(modifier, originalSource, modifierSourceEntity).Build();

        private class IntermediateModifierBuilder
        {
            private readonly IIntermediateModifier _modifier;
            private readonly ModifierSource _originalSource;
            private readonly Entity _modifierSourceEntity;
            private readonly List<Modifier> _modifiers = new List<Modifier>();

            public IntermediateModifierBuilder(
                IIntermediateModifier modifier, ModifierSource originalSource, Entity modifierSourceEntity)
            {
                _modifier = modifier;
                _originalSource = originalSource;
                _modifierSourceEntity = modifierSourceEntity;
            }

            public IReadOnlyList<Modifier> Build()
            {
                foreach (var entry in _modifier.Entries)
                {
                    Build(entry);
                }
                return _modifiers;
            }

            private void Build(IntermediateModifierEntry entry)
            {
                if (entry.Form == null || entry.Stat == null || entry.Value == null)
                    return;

                var (form, formValueConverter) = entry.Form.Build();
                var buildParameters = new BuildParameters(_originalSource, _modifierSourceEntity, form);

                var statBuilder = entry.Stat;
                if (entry.Condition != null)
                {
                    statBuilder = statBuilder.WithCondition(entry.Condition);
                }
                statBuilder = _modifier.StatConverter(statBuilder);
                var statBuilderResults = statBuilder.Build(buildParameters);

                foreach (var (stats, source, statValueConverter) in statBuilderResults)
                {
                    var valueBuilder = formValueConverter(statValueConverter(_modifier.ValueConverter(entry.Value)));
                    var value = valueBuilder.Build(buildParameters);
                    _modifiers.Add(new Modifier(stats, form, value, source));
                }
            }
        }
    }
}