using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Engine.Computation.Common.Data;
using PoESkillTree.Engine.Computation.Data.Collections;

namespace PoESkillTree.Engine.Computation.Data
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
            {
                // Pure Talent Jewel
                @"while your passive skill tree connects to a class' starting location, you gain: (marauder: .*) (duelist: .*) (ranger: .*) (shadow: .*) (witch: .*) (templar: .*) (scion: .*)",
                "$1", "$2", "$3", "$4", "$5", "$6", "$7"
            },
            {
                // Fevered Mind Jewel
                @"notable passive skills in radius are transformed to instead grant: (\d+% .+) and (\d+% .+)",
                "notable passive skills in radius grant nothing", "$1 per notable allocated in radius", "$2 per notable allocated in radius"
            },
            {
                // Flask suffixes
                @"(?<stat1>(immune|immunity) to (?<effect>.+) during flask effect) (?<stat2>removes? (\k<effect>|burning) on use)",
                "${stat1}", "${stat2}"
            },
            {
                // Hinder reduced movement speed
                @"(.* hinder enemies.*), with (\d+% reduced movement speed)",
                "$1", "hinder enemies with $2"
            },
            {
                @"(.* hinder enemies.*), reducing movement speed by (\d+%)",
                "$1", "hinder enemies with $2 reduced movement speed"
            },
            {
                @"(enemies .* are hindered), with (\d+% reduced movement speed)",
                "$1", "hinder enemies with $2"
            },
            {
                @"(every \d+ seconds?, gain a) (.*), up to a maximum of (\d+)",
                "$1 $2", "+$3 to maximum $2"
            },
            {
                @"(every second, consume a nearby corpse to .* mana) (\d+% .* if you haven't consumed a corpse recently)",
                "$1", "$2"
            },
            {
                @"(you can only have one herald) (\d+% .* herald buffs on you) (\d+% .* herald skills) (\d+% .* herald skills) (minions from herald skills deal \d+% more damage) (your aura skills are disabled)",
                "$2", "$3", "$4", "$5"
            },
            {
                @"(.+ while affected by a non-vaal guard skill) (.+ while affected by a non-vaal guard skill) (.+ if a non-vaal guard buff was lost recently)",
                "$1", "$2", "$3"
            },
            // keystones
            {
                // Acrobatics
                @"(\d+% chance to dodge attack hits)\. (\d+% less armour), (\d+% less energy shield), (\d+% less chance to block .+)",
                "$1", "$2", "$3", "$4"
            },
            {
                // Eldritch Battery
                @"(Spend Energy Shield before Mana for Skill Costs) energy shield protects mana instead of life (\d+% less Energy Shield Recharge Rate)",
                "$1",
                "100% of non-chaos damage is taken from energy shield before mana",
                "-100% of non-chaos damage is taken from energy shield before life",
                "$2"
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
                @"(life leeched per second is doubled) (maximum .* is doubled) (life regeneration has no effect)",
                "$1", "$2", "$3"
            },
            {
                // Ancestral Bond
                "(you can't deal damage with skills yourself) (.* of summoned totems)",
                "$1", "$2"
            },
            {
                // Runebinder
                @"(.* of summoned totems)\.? (you can have an additional brand attached to an enemy)",
                "$1", "$2"
            },
            {
                // Ghost Reaver
                @"(leech energy shield instead of life) (maximum .* is doubled) (\d+% less energy shield recharge rate)",
                "$1", "$2", "$3"
            },
            {
                // Arrow Dancing
                @"(\d+% more chance to evade projectile attacks) (\d+% less chance to evade melee attacks)",
                "$1", "$2"
            },
            {
                // Elemental Overload
                @"(\d+% more elemental damage if you've dealt a crit in the past \d+ seconds) (your critical strikes do not deal extra damage) (ailments never count as being from critical strikes)",
                "$1", "$2", "$3"
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
                @"(modifiers to critical strike multiplier also apply to damage over time multiplier for ailments from critical strikes at \d+% of their value) (\d+% less damage with hits)",
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
            {
                // Wicked Ward
                @"(energy shield recharge is not interrupted by damage if recharge began recently) (\d+% less .*) (\d+% less .*)",
                "$2", "$3"
            },
            {
                // Imbalanced Guard
                @"(\d+% chance to defend with double armour) (maximum damage reduction for any damage type is \d+%)",
                "$1", "$2"
            },
            {
                // Wind Dancer
                @"(.+ if you haven't been hit recently) (.+ if you haven't been hit recently) (.+ if you've been hit recently)",
                "$1", "$2", "$3"
            },
            {
                // Eternal Youth
                @"(\d+% less life regeneration rate) (\d+% less maximum total recovery per second from life leech) (energy shield recharge instead applies to life)",
                "$1", "$2", "$3"
            },
            {
                // Glancing Blows
                @"(.+ is doubled) (.+ is doubled) (you take \d+% of damage from blocked hits)",
                "$1", "$2", "$3"
            },
            {
                // The Agnostic
                @"(maximum energy shield is 0) (while not on full life, .+)",
                "$1", "$2"
            },
            {
                // Call to Arms
                @"(using warcries is instant) (warcries share their cooldown)",
                "$1"
            },
            {
                // Supreme Ego
                @"(you can only have one permanent aura on you from your skills) (auras from your skills .+) (auras from your skills .+) (\d+% more mana reserved)",
                "$2", "$3", "$4"
            },
            // Ascendancies
            {
                // Berserker
                @"(warcries sacrifice \d+ rage if you have at least \d+ rage) (exerted attacks deal \d+% more damage if a warcry sacrificed rage recently)",
                "$2"
            },
            // Skills
            {
                // Arcane Surge Support
                "(arcane surge grants .*) (arcane surge grants .*) (arcane surge grants .*)",
                "$1", "$2", "$3"
            },
            {
                // Storm Barrier Support
                "(.* while channelling supported skills) (.* while channelling supported skills) (.* while channelling supported skills)",
                "$1", "$2", "$3"
            },
            {
                // Penance Brand
                @"(\+\d+ to explosion radius per energy) (pulse deals .*)",
                "$1", "$2"
            },
        }.ToList();
    }
}