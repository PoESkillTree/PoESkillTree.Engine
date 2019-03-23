﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using PoESkillTree.GameModel.Items;

namespace PoESkillTree.GameModel.Modifiers
{
    /// <summary>
    /// Determines whether a modifier is local and whether it affects the properties on the item it's on.
    /// Local modifiers that don't affect properties are only relevant for weapons so the stat can be restricted to
    /// the hand the item is in when that restriction is not clear from the text (e.g. "with this weapon"), and
    /// for flasks so they can be applied without being modified by flask effect.
    /// </summary>
    public static class ModifierLocalityTester
    {
        private const string Increase = "#% (increased|reduced) ";
        private const string BaseAdd = @"(\+|-)?#%? to ";
        private const string Chance = @"\+?#% chance to ";
        private static readonly string OptionalChance = $"({Chance})?";

        private static readonly IReadOnlyList<Regex> UnconditionalProperty = new RegexCollection
        {
            "level requirement (increased|reduced) by #",
            BaseAdd + "quality",
            @"(has no|no|#% increased|#% reduced|\+#|-#) (attribute|intelligence|dexterity|strength) requirements?",
        }.ToList();

        private static readonly IReadOnlyList<Regex> WeaponProperty = new RegexCollection
        {
            "adds # to # (physical|fire|cold|lightning|chaos) damage( in (main|off) hand)?",
            "no physical damage",
            Increase + "(physical damage|attack speed|critical strike chance|weapon range)",
        }.ToList();

        private static readonly IReadOnlyList<Regex> WeaponLocal = new RegexCollection
        {
            OptionalChance + "impale enemies on hit with attacks",
            OptionalChance + "(blind enemies|poison|causes? bleeding|maim) on hit",
            OptionalChance + "cause (poison|bleeding) on critical strike",
            "always poison on hit",
            "#% of physical attack damage leched as (life|mana|life and mana)",
            BaseAdd + "accuracy rating",
            "hits can't be evaded",
            "# (life|mana) gained for each enemy hit by attacks",
            "# life and mana gained for each enemy hit",
        }.ToList();

        private static readonly IReadOnlyList<Regex> ArmourProperty = new RegexCollection
        {
            $"({Increase}|{BaseAdd})" +
            "(armour|evasion|energy shield|armour and evasion|armour and energy shield|evasion and energy shield|armour, evasion and energy Shield)",
            "item has no level requirement and energy shield",
        }.ToList();

        private static readonly IReadOnlyList<Regex> ShieldProperty = new RegexCollection
        {
            Chance + "block",
            "cannot block",
            "no chance to block",
        }.ToList();

        private static readonly IReadOnlyList<Regex> FlaskProperty = new RegexCollection
        {
            Increase + "(recovery rate|(life|mana|amount) recovered|effect|duration|charges used)",
            BaseAdd + "maximum charges",
        }.ToList();

        private static readonly IReadOnlyList<Regex> FlaskLocal = new RegexCollection
        {
            Increase + "(charge recovery|recovery when on low life)",
            OptionalChance + "gain a flask charge when you deal a critical strike",
            "gains no charges during effect of any (soul ripper|overflowing chalice) flask",
            "recharges # charges? when you ((deal|take) a critical strike|consume an ignited corpse)",
            "instant recovery(when on low life)?",
            "#% of recovery applied instantly",
            "grants #% of life recovery to minions",
        }.ToList();

        private const string Attribute = "(strength|dexterity|intelligence)";

        private static readonly IReadOnlyList<Regex> PassiveNodeProperty = new RegexCollection
        {
            $@"\+# to {Attribute}",
            $@"\+# to {Attribute} and {Attribute}",
        }.ToList();

        private static readonly Regex NumberRegex = new Regex(@"\d+(\.\d+)?");

        public static bool AffectsProperties(string modifier, Tags itemTags)
        {
            var canonicalModifier = ToCanonicalModifier(modifier);
            if (AnyMatch(UnconditionalProperty, canonicalModifier))
                return true;
            if (itemTags.HasFlag(Tags.Weapon) && AnyMatch(WeaponProperty, canonicalModifier))
                return true;
            if (itemTags.HasFlag(Tags.Armour) && AnyMatch(ArmourProperty, canonicalModifier))
                return true;
            if (itemTags.HasFlag(Tags.Shield) && AnyMatch(ShieldProperty, canonicalModifier))
                return true;
            if (itemTags.HasFlag(Tags.Flask) && AnyMatch(FlaskProperty, canonicalModifier))
                return true;
            return false;
        }

        public static bool IsLocal(string modifier, Tags itemTags)
        {
            if (AffectsProperties(modifier, itemTags))
                return true;

            var canonicalModifier = ToCanonicalModifier(modifier);
            if (itemTags.HasFlag(Tags.Weapon) && AnyMatch(WeaponLocal, canonicalModifier))
                return true;
            if (itemTags.HasFlag(Tags.Flask) && AnyMatch(FlaskLocal, canonicalModifier))
                return true;
            return false;
        }

        public static bool AffectsPassiveNodeProperty(string modifier)
            => AnyMatch(PassiveNodeProperty, ToCanonicalModifier(modifier));

        private static string ToCanonicalModifier(string modifier)
            => NumberRegex.Replace(modifier, "#").ToLowerInvariant();

        private static bool AnyMatch(IEnumerable<Regex> regexes, string modifier)
            => regexes.Any(r => r.IsMatch(modifier));

        private class RegexCollection : IEnumerable<Regex>
        {
            private readonly List<Regex> _regexes = new List<Regex>();

            public void Add([RegexPattern] string pattern)
                => _regexes.Add(new Regex($"^{pattern}$", RegexOptions.Singleline | RegexOptions.IgnoreCase));

            public IEnumerator<Regex> GetEnumerator() => _regexes.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}