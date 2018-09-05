﻿using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Computation.Common.Data;
using PoESkillTree.Computation.Data.Collections;

namespace PoESkillTree.Computation.Data
{
    /// <summary>
    /// Provides a collection of <see cref="StatReplacerData"/>. Mostly handles keystones, they often contain multiple
    /// stats in a single translation. Besides splitting stats, some parts are also replaced by formulations that can
    /// be parsed without adding a new matcher.
    /// </summary>
    /// <remarks>
    /// Regex patterns here are different from the patterns in matchers. These don't support expansion (no value
    /// placeholders, no references to other matchers) but allow referencing groups by index.
    /// </remarks>
    public class StatReplacers
    {
        public IReadOnlyList<StatReplacerData> Replacers { get; } = new StatReplacerCollection
        {
            {
                // Grand Spectrum: Add an additional stat line that increases the Grand Spectrum counter.
                @"(.+) per grand spectrum",
                "grand spectrum", "$0"
            },
            {
                // Corrupted Energy Cobalt Jewel
                @"(with \d corrupted items equipped): (\d+% of chaos damage does not bypass energy shield), and (\d+% of physical damage bypasses energy shield)",
                "$1 $2", "$1 $3"
            },
            // keystones
            {
                // Acrobatics
                @"(\d+% chance to dodge attack hits)\. (\d+% less armour), (\d+% less energy shield), (\d+% less chance to block .+)",
                "$1", "$2", "$3", "$4"
            },
            {
                // Eldritch Battery
                "(Spend Energy Shield before Mana for Skill Costs) energy shield protects mana instead of life",
                "$1",
                "100% of non-chaos damage is taken from energy shield before mana",
                "-100% of non-chaos damage is taken from energy shield before life"
            },
            {
                // Chaos Inoculation
                "(maximum life becomes 1), (immune to chaos damage)",
                "$1", "$2"
            },
            {
                // Blood Magic
                @"(removes all mana)\. (spend .*)",
                "$1", "$2"
            },
            {
                // Iron Reflexes
                @"(converts all evasion rating to armour)\. (dexterity provides no bonus to evasion rating)",
                "$1", "-1 to dexterity evasion bonus per dexterity"
            },
            {
                // Iron Grip
                "the increase to physical damage from strength applies to projectile attacks as well as melee attacks",
                "1% increased physical projectile attack damage per 5 strength damage bonus ceiled"
            },
            {
                // Vaal Pact
                @"life leeched per second is doubled\. (maximum life leech rate is doubled)\. (life regeneration has no effect)\.",
                "100% more life leeched per second", "$1", "$2"
            },
            {
                // Ancestral Bond
                "(you can't deal damage with skills yourself) (can have up to 1 additional totem summoned at a time)",
                "$1", "$2"
            },
            {
                // Ghost Reaver
                @"(life leech is applied to energy shield instead) (\d+% less energy shield recharge rate)",
                "$1", "$2"
            },
            {
                // Arrow Dancing
                @"(\d+% more chance to evade projectile attacks) (\d+% less chance to evade melee attacks)",
                "$1", "$2"
            },
            {
                // Elemental Overload
                @"(\d+% more elemental damage if you've crit in the past \d+ seconds) (no critical strike multiplier) (no damage multiplier for ailments from critical strikes)",
                "$1", "$2, $3"
            },
            {
                // Avatar of Fire
                @"(\d+% of physical, cold and lightning damage converted to fire damage) (deal no non-fire damage)",
                "$1", "$2"
            },
            {
                // Unwavering Stance
                @"(cannot evade enemy attacks) (cannot be stunned)",
                "$1", "$2"
            },
            {
                // Perfect Agony
                @"(modifiers to critical strike multiplier also apply to damage multiplier for ailments from critical strikes at \d+% of their value) (\d+% less damage with hits)",
                "$1", "$2"
            },
            {
                // Crimson Dance
                @"(you can inflict bleeding on an enemy up to \d+ times) (your bleeding does not deal extra damage while the enemy is moving) (\d+% less damage with bleeding)",
                "$1", "$2", "$3"
            },
            {
                // Resolute Technique
                @"(your hits can't be evaded) (never deal critical strikes)",
                "$1", "$2"
            },
        }.ToList();
    }
}