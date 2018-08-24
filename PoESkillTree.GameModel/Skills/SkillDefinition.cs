﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PoESkillTree.GameModel.Items;
using PoESkillTree.Utils.Extensions;

namespace PoESkillTree.GameModel.Skills
{
    public class SkillDefinition
    {
        [Obsolete("Temporary constructor to support PoESkillTree.Computation.Console.SkillDefinition")]
        public SkillDefinition(string skillName, int numericId, IReadOnlyList<Keyword> keywords, bool providesBuff)
        {
            Id = skillName;
            NumericId = numericId;
            IsSupport = false;
            ActiveSkill = new ActiveSkillDefinition(skillName, 0, new string[0], new string[0], keywords, providesBuff,
                null, new ItemClass[0]);
            Levels = new Dictionary<int, SkillLevelDefinition>();
        }

        private SkillDefinition(
            string id, int numericId, bool isSupport, string statTranslationFile,
            SkillBaseItemDefinition baseItem, ActiveSkillDefinition activeSkill, SupportSkillDefinition supportSkill,
            IReadOnlyDictionary<int, SkillLevelDefinition> levels)
            => (Id, NumericId, IsSupport, BaseItem, ActiveSkill, SupportSkill, Levels, StatTranslationFile) =
                (id, numericId, isSupport, baseItem, activeSkill, supportSkill, levels, statTranslationFile);

        public static SkillDefinition CreateActive(
            string id, int numericId, string statTranslationFile,
            SkillBaseItemDefinition baseItem, ActiveSkillDefinition activeSkill,
            IReadOnlyDictionary<int, SkillLevelDefinition> levels)
            => new SkillDefinition(id, numericId, false, statTranslationFile, baseItem, activeSkill, null, levels);

        public static SkillDefinition CreateSupport(
            string id, int numericId, string statTranslationFile,
            SkillBaseItemDefinition baseItem, SupportSkillDefinition supportSkill,
            IReadOnlyDictionary<int, SkillLevelDefinition> levels)
            => new SkillDefinition(id, numericId, true, statTranslationFile, baseItem, null, supportSkill, levels);

        public string Id { get; }
        public int NumericId { get; }
        public bool IsSupport { get; }
        public string StatTranslationFile { get; }

        [CanBeNull]
        public SkillBaseItemDefinition BaseItem { get; }

        public ActiveSkillDefinition ActiveSkill { get; }
        public SupportSkillDefinition SupportSkill { get; }

        public IReadOnlyDictionary<int, SkillLevelDefinition> Levels { get; }
    }

    public class SkillBaseItemDefinition
    {
        public SkillBaseItemDefinition(
            string displayName, string metadataId, ReleaseState releaseState, IEnumerable<string> gemTags)
            => (DisplayName, MetadataId, ReleaseState, GemTags) = (displayName, metadataId, releaseState, gemTags);

        public string DisplayName { get; }
        public string MetadataId { get; }
        public ReleaseState ReleaseState { get; }
        public IEnumerable<string> GemTags { get; }
    }

    public class ActiveSkillDefinition
    {
        public ActiveSkillDefinition(
            string displayName, int castTime,
            IEnumerable<string> activeSkillTypes, IEnumerable<string> minionActiveSkillTypes,
            IReadOnlyList<Keyword> keywords, bool providesBuff, double? totemLifeMultiplier,
            IReadOnlyList<ItemClass> weaponRestrictions)
            => (DisplayName, CastTime, ActiveSkillTypes, MinionActiveSkillTypes, Keywords, ProvidesBuff,
                    TotemLifeMultiplier, WeaponRestrictions) =
                (displayName, castTime, activeSkillTypes, minionActiveSkillTypes, keywords, providesBuff,
                    totemLifeMultiplier, weaponRestrictions);

        public string DisplayName { get; }
        public int CastTime { get; }
        public IEnumerable<string> ActiveSkillTypes { get; }
        public IEnumerable<string> MinionActiveSkillTypes { get; }
        public IReadOnlyList<Keyword> Keywords { get; }
        public bool ProvidesBuff { get; }
        public double? TotemLifeMultiplier { get; }
        public IReadOnlyList<ItemClass> WeaponRestrictions { get; }
    }

    public class SupportSkillDefinition
    {
        public SupportSkillDefinition(
            bool supportsGemsOnly, IReadOnlyList<string> allowedActiveSkillTypes,
            IReadOnlyList<string> excludedActiveSkillTypes, IReadOnlyList<string> addedActiveSkillTypes)
            => (SupportsGemsOnly, AllowedActiveSkillTypes, ExcludedActiveSkillTypes, AddedActiveSkillTypes) =
                (supportsGemsOnly, allowedActiveSkillTypes, excludedActiveSkillTypes, addedActiveSkillTypes);

        public bool SupportsGemsOnly { get; }

        public IReadOnlyList<string> AllowedActiveSkillTypes { get; }
        public IReadOnlyList<string> ExcludedActiveSkillTypes { get; }
        public IReadOnlyList<string> AddedActiveSkillTypes { get; }
    }

    public class SkillLevelDefinition
    {
        public SkillLevelDefinition(
            double? damageEffectiveness, double? damageMultiplier, double? criticalStrikeChance,
            int? manaCost, double? manaMultiplier, int? manaReservationOverride, int cooldown,
            int requiredLevel, int requiredDexterity, int requiredIntelligence, int requiredStrength,
            IReadOnlyList<UntranslatedStat> qualityStats, IReadOnlyList<UntranslatedStat> stats,
            SkillTooltipDefinition tooltip)
        {
            DamageEffectiveness = damageEffectiveness;
            DamageMultiplier = damageMultiplier;
            CriticalStrikeChance = criticalStrikeChance;
            ManaCost = manaCost;
            ManaMultiplier = manaMultiplier;
            ManaReservationOverride = manaReservationOverride;
            Cooldown = cooldown;
            RequiredLevel = requiredLevel;
            RequiredDexterity = requiredDexterity;
            RequiredIntelligence = requiredIntelligence;
            RequiredStrength = requiredStrength;
            QualityStats = qualityStats;
            Stats = stats;
            Tooltip = tooltip;
        }

        public double? DamageEffectiveness { get; }
        public double? DamageMultiplier { get; }
        public double? CriticalStrikeChance { get; }

        public int? ManaCost { get; }
        public double? ManaMultiplier { get; }
        public int? ManaReservationOverride { get; }
        public int Cooldown { get; }

        public int RequiredLevel { get; }
        public int RequiredDexterity { get; }
        public int RequiredIntelligence { get; }
        public int RequiredStrength { get; }

        public IReadOnlyList<UntranslatedStat> QualityStats { get; }
        public IReadOnlyList<UntranslatedStat> Stats { get; }

        public SkillTooltipDefinition Tooltip { get; }
    }

    public struct UntranslatedStat
    {
        public UntranslatedStat(string statId, int value) => (StatId, Value) = (statId, value);

        public string StatId { get; }
        public int Value { get; }
    }

    public class SkillTooltipDefinition
    {
        public SkillTooltipDefinition(
            string name, IReadOnlyList<TranslatedStat> properties, IReadOnlyList<TranslatedStat> requirements,
            IReadOnlyList<string> description,
            IReadOnlyList<TranslatedStat> qualityStats, IReadOnlyList<TranslatedStat> stats)
        {
            Name = name;
            Properties = properties;
            Requirements = requirements;
            Description = description;
            QualityStats = qualityStats;
            Stats = stats;
        }

        public string Name { get; }
        public IReadOnlyList<TranslatedStat> Properties { get; }
        public IReadOnlyList<TranslatedStat> Requirements { get; }
        public IReadOnlyList<string> Description { get; }
        public IReadOnlyList<TranslatedStat> QualityStats { get; }
        public IReadOnlyList<TranslatedStat> Stats { get; }
    }

    public class TranslatedStat
    {
        public TranslatedStat(string formatText, params double[] values)
            => (FormatText, Values) = (formatText, values);

        public string FormatText { get; }
        public IReadOnlyList<double> Values { get; }

        public override bool Equals(object obj)
            => this == obj || (obj is TranslatedStat other && Equals(other));

        private bool Equals(TranslatedStat other)
            => FormatText == other.FormatText && Values.SequenceEqual(other.Values);

        public override int GetHashCode()
            => (FormatText, Values.SequenceHash()).GetHashCode();

        public override string ToString()
            => string.Format(FormatText, Values.Cast<object>().ToArray());
    }
}