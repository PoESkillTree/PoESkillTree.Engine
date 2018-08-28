﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EnumsNET;
using MoreLinq;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Damage;
using PoESkillTree.Computation.Common.Builders.Equipment;
using PoESkillTree.Computation.Common.Builders.Forms;
using PoESkillTree.Computation.Common.Builders.Modifiers;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.Computation.Common.Builders.Values;
using PoESkillTree.GameModel.Items;
using PoESkillTree.GameModel.Skills;

namespace PoESkillTree.Computation.Parsing
{
    public class ActiveSkillParser : IParser<Skill>
    {
        private const string DamageTypeRegex = "(physical|cold|fire|lightning|chaos)";

        private static readonly Regex HitDamageRegex =
            new Regex($"^(attack|spell|secondary)_(minimum|maximum)_base_{DamageTypeRegex}_damage$");

        private static readonly Regex DamageOverTimeRegex =
            new Regex($"^base_{DamageTypeRegex}_damage_to_deal_per_minute$");

        private static readonly IReadOnlyList<Keyword> KeywordsExcludedForDamageOverTime = new[]
        {
            Keyword.Attack, Keyword.Spell, Keyword.Melee, Keyword.Projectile, Keyword.AreaOfEffect, Keyword.Movement
        };

        private static readonly IReadOnlyList<string> AreaDamageOverTimeSkills = new[]
        {
            "PoisonArrow", "ColdSnap", "VaalColdSnap", "Desecrate", "FireTrap", "RighteousFire", "VaalRighteousFire",
            "FrostBoltNova"
        };

        private readonly SkillDefinitions _skillDefinitions;
        private readonly IBuilderFactories _builderFactories;
        private readonly IMetaStatBuilders _metaStatBuilders;
        private readonly IModifierBuilder _modifierBuilder = new ModifierBuilder();

        public ActiveSkillParser(
            SkillDefinitions skillDefinitions, IBuilderFactories builderFactories, IMetaStatBuilders metaStatBuilders)
            => (_skillDefinitions, _builderFactories, _metaStatBuilders) =
                (skillDefinitions, builderFactories, metaStatBuilders);

        public ParseResult Parse(Skill parameter)
        {
            var definition = _skillDefinitions.GetSkillById(parameter.Id);
            var activeSkill = definition.ActiveSkill;
            var level = definition.Levels[parameter.Level];

            var displayName = definition.BaseItem?.DisplayName ??
                              (definition.IsSupport ? parameter.Id : definition.ActiveSkill.DisplayName);
            var localSource = new ModifierSource.Local.Skill(displayName);
            var globalSource = new ModifierSource.Global(localSource);
            var isMainSkill = _metaStatBuilders.MainSkillSocket(parameter.ItemSlot, parameter.SocketIndex).IsSet;
            var modifiers = new List<Modifier>();

            void AddLocal(IIntermediateModifier m) => modifiers.AddRange(BuildLocal(m));
            IReadOnlyList<Modifier> BuildLocal(IIntermediateModifier m) => m.Build(localSource, Entity.Character);
            void AddGlobal(IIntermediateModifier m) => modifiers.AddRange(BuildGlobal(m));
            IReadOnlyList<Modifier> BuildGlobal(IIntermediateModifier m) => m.Build(globalSource, Entity.Character);

            var hitDamageSource = DetermineHitDamageSource(activeSkill, level);
            var hasSkillDamageOverTime = HasSkillDamageOverTime(level);

            if (hitDamageSource.HasValue)
            {
                AddGlobal(Modifier(_metaStatBuilders.SkillHitDamageSource,
                    Forms.TotalOverride, (int) hitDamageSource.Value, isMainSkill));
            }
            var usesMainHandCondition = isMainSkill;
            var usesOffHandCondition = isMainSkill.And(OffHand.Has(Tags.Weapon));
            if (activeSkill.ActiveSkillTypes.Contains(ActiveSkillType.RequiresDualWield))
                usesMainHandCondition = usesMainHandCondition.And(OffHand.Has(Tags.Weapon));
            else if (activeSkill.ActiveSkillTypes.Contains(ActiveSkillType.RequiresShield))
                usesMainHandCondition = usesMainHandCondition.And(OffHand.Has(Tags.Shield));
            if (activeSkill.WeaponRestrictions.Any())
            {
                usesMainHandCondition = usesMainHandCondition.And(
                    CreateWeaponRestrictionCondition(MainHand, activeSkill.WeaponRestrictions));
                usesOffHandCondition = usesOffHandCondition.And(
                    CreateWeaponRestrictionCondition(OffHand, activeSkill.WeaponRestrictions));
            }
            AddGlobal(Modifier(_metaStatBuilders.SkillUsesHand(AttackDamageHand.MainHand),
                Forms.TotalOverride, 1, usesMainHandCondition));
            if (!activeSkill.ActiveSkillTypes.Contains(ActiveSkillType.DoesNotUseOffHand))
            {
                AddGlobal(Modifier(_metaStatBuilders.SkillUsesHand(AttackDamageHand.OffHand),
                    Forms.TotalOverride, 1, usesOffHandCondition));
            }
            AddGlobal(Modifier(_metaStatBuilders.MainSkillId,
                Forms.TotalOverride, definition.NumericId, isMainSkill));

            void AddKeywordModifiers(
                Func<Keyword, IStatBuilder> statFactory, Func<Keyword, IConditionBuilder> conditionFactory,
                Func<Keyword, bool> preCondition = null)
                => modifiers.AddRange(
                    KeywordModifiers(activeSkill.Keywords, statFactory, conditionFactory, preCondition)
                        .SelectMany(BuildGlobal));

            AddKeywordModifiers(_metaStatBuilders.MainSkillHasKeyword, _ => isMainSkill);
            AddKeywordModifiers(
                _metaStatBuilders.MainSkillPartHasKeyword,
                k => PartHasKeywordCondition(hitDamageSource, isMainSkill, k));
            AddKeywordModifiers(
                _metaStatBuilders.MainSkillPartCastRateHasKeyword,
                k => PartHasKeywordCondition(hitDamageSource, isMainSkill, k));
            if (hitDamageSource.HasValue)
            {
                var hitIsAreaDamage = HitDamageIsArea(level);
                AddKeywordModifiers(
                    k => _metaStatBuilders.MainSkillPartDamageHasKeyword(k, hitDamageSource.Value),
                    k => PartHasKeywordCondition(hitDamageSource, isMainSkill, k),
                    k => k != Keyword.AreaOfEffect || hitIsAreaDamage);
            }
            if (hasSkillDamageOverTime)
            {
                var dotIsAreaDamage = AreaDamageOverTimeSkills.Contains(definition.Id);
                AddKeywordModifiers(
                    k => _metaStatBuilders.MainSkillPartDamageHasKeyword(k, DamageSource.OverTime),
                    k => PartHasKeywordCondition(hitDamageSource, isMainSkill, k),
                    k => k == Keyword.AreaOfEffect ? dotIsAreaDamage : !KeywordsExcludedForDamageOverTime.Contains(k));
            }
            AddKeywordModifiers(
                k => _metaStatBuilders.MainSkillPartAilmentDamageHasKeyword(k),
                k => PartHasKeywordCondition(hitDamageSource, isMainSkill, k),
                k => !KeywordsExcludedForDamageOverTime.Contains(k));

            if (hitDamageSource != DamageSource.Attack)
            {
                var castRateDamageSource = hitDamageSource ?? DamageSource.Spell;
                AddLocal(Modifier(_builderFactories.StatBuilders.CastRate.With(castRateDamageSource),
                    Forms.BaseSet, 1000D / activeSkill.CastTime, isMainSkill));
            }

            if (activeSkill.TotemLifeMultiplier is double lifeMulti)
            {
                var totemLifeStat = _builderFactories.StatBuilders.Pool.From(Pool.Life)
                    .For(_builderFactories.EntityBuilders.Totem);
                AddGlobal(Modifier(totemLifeStat, Forms.PercentMore, (lifeMulti - 1) * 100, isMainSkill));
            }

            if (level.DamageEffectiveness.HasValue)
            {
                AddGlobal(Modifier(_metaStatBuilders.DamageBaseAddEffectiveness,
                    Forms.TotalOverride, level.DamageEffectiveness.Value, isMainSkill));
            }
            if (level.DamageMultiplier.HasValue)
            {
                AddGlobal(Modifier(_metaStatBuilders.DamageBaseSetEffectiveness,
                    Forms.TotalOverride, level.DamageMultiplier.Value, isMainSkill));
            }
            if (level.CriticalStrikeChance.HasValue && hitDamageSource.HasValue)
            {
                AddLocal(Modifier(_builderFactories.ActionBuilders.CriticalStrike.Chance.With(hitDamageSource.Value),
                    Forms.BaseSet, level.CriticalStrikeChance.Value, isMainSkill));
            }

            if (level.ManaCost.HasValue)
            {
                AddGlobal(Modifier(_builderFactories.StatBuilders.Pool.From(Pool.Mana).Cost,
                    Forms.BaseSet, level.ManaCost.Value, isMainSkill));
            }
            if (level.Cooldown.HasValue)
            {
                AddGlobal(Modifier(_builderFactories.StatBuilders.Cooldown,
                    Forms.BaseSet, level.Cooldown.Value, isMainSkill));
            }

            AddLocal(Modifier(_builderFactories.StatBuilders.Requirements.Level, Forms.BaseSet, level.RequiredLevel));
            if (level.RequiredDexterity > 0)
            {
                AddLocal(Modifier(_builderFactories.StatBuilders.Requirements.Dexterity,
                    Forms.BaseSet, level.RequiredDexterity));
            }
            if (level.RequiredIntelligence > 0)
            {
                AddLocal(Modifier(_builderFactories.StatBuilders.Requirements.Intelligence,
                    Forms.BaseSet, level.RequiredIntelligence));
            }
            if (level.RequiredStrength > 0)
            {
                AddLocal(Modifier(_builderFactories.StatBuilders.Requirements.Strength,
                    Forms.BaseSet, level.RequiredStrength));
            }

            if (level.QualityStats.Any())
            {
                AddGlobal(_modifierBuilder
                    .WithStat(_builderFactories.StatBuilders.CastRate.With(DamageSource.Attack))
                    .WithForm(Forms.PercentIncrease)
                    .WithValue(CreateValue(level.QualityStats[0].Value * parameter.Quality))
                    .WithCondition(isMainSkill).Build());
            }

            var (local, global, remainingStats) = ParseWithoutTranslating(level.Stats, isMainSkill);
            local.ForEach(AddLocal);
            global.ForEach(AddGlobal);
            if (level.Stats.Any())
            {
                AddGlobal(_modifierBuilder
                    .WithStat(_builderFactories.DamageTypeBuilders.Physical.Damage)
                    .WithForm(Forms.PercentIncrease)
                    .WithValue(level.Stats[0].Value * _builderFactories.ChargeTypeBuilders.Frenzy.Amount.Value)
                    .WithCondition(isMainSkill).Build());
                AddGlobal(_modifierBuilder
                    .WithStat(_builderFactories.StatBuilders.CastRate.With(DamageSource.Attack))
                    .WithForm(Forms.PercentIncrease)
                    .WithValue(level.Stats[0].Value * _builderFactories.ChargeTypeBuilders.Frenzy.Amount.Value)
                    .WithCondition(isMainSkill).Build());
            }

            return ParseResult.Success(modifiers);
        }

        private static IConditionBuilder CreateWeaponRestrictionCondition(
            IEquipmentBuilder hand, IEnumerable<ItemClass> weaponRestrictions)
            => weaponRestrictions.Select(hand.Has).Aggregate((l, r) => l.Or(r));

        private static DamageSource? DetermineHitDamageSource(
            ActiveSkillDefinition activeSkill, SkillLevelDefinition level)
        {
            if (activeSkill.ActiveSkillTypes.Contains(ActiveSkillType.Attack))
                return DamageSource.Attack;
            var statIds = level.Stats.Select(s => s.StatId);
            foreach (var statId in statIds)
            {
                var match = HitDamageRegex.Match(statId);
                if (match.Success)
                    return Enums.Parse<DamageSource>(match.Groups[1].Value, true);
                if (statId == "display_skill_deals_secondary_damage")
                    return DamageSource.Secondary;
            }
            return null;
        }

        private static bool HasSkillDamageOverTime(SkillLevelDefinition level)
            => level.Stats.Select(s => s.StatId).Any(DamageOverTimeRegex.IsMatch);

        private static bool HitDamageIsArea(SkillLevelDefinition level)
            => level.Stats.Any(s => s.StatId == "is_area_damage");

        private IConditionBuilder PartHasKeywordCondition(
            DamageSource? hitDamageSource, IConditionBuilder baseCondition, Keyword keyword)
        {
            var mainHandIsRanged = MainHand.Has(Tags.Ranged);
            switch (keyword)
            {
                case Keyword.Melee:
                    return baseCondition.And(mainHandIsRanged.Not);
                case Keyword.Projectile when hitDamageSource == DamageSource.Attack:
                    return baseCondition.And(mainHandIsRanged);
                default:
                    return baseCondition;
            }
        }

        private IEnumerable<IIntermediateModifier> KeywordModifiers(
            IEnumerable<Keyword> keywords, Func<Keyword, IStatBuilder> statFactory,
            Func<Keyword, IConditionBuilder> conditionFactory, Func<Keyword, bool> preCondition = null)
        {
            if (preCondition != null)
                keywords = keywords.Where(preCondition);
            foreach (var keyword in keywords)
            {
                yield return Modifier(statFactory(keyword),
                    Forms.TotalOverride, CreateValue(1), conditionFactory(keyword));
            }
        }

        private (IEnumerable<IIntermediateModifier> local, IEnumerable<IIntermediateModifier> global,
            IEnumerable<UntranslatedStat> unparsed)
            ParseWithoutTranslating(IEnumerable<UntranslatedStat> stats, IConditionBuilder isMainSkill)
        {
            var localModifiers = new List<IIntermediateModifier>();
            var globalModifiers = new List<IIntermediateModifier>();
            var unparsedStats = new List<UntranslatedStat>();
            DamageType? hitDamageType = null;
            DamageSource? hitDamageSource = null;
            double hitDamageMinimum = 0D;
            double? hitDamageMaximum = null;
            foreach (var stat in stats)
            {
                var match = HitDamageRegex.Match(stat.StatId);
                if (match.Success)
                {
                    hitDamageSource = Enums.Parse<DamageSource>(match.Groups[1].Value, true);
                    hitDamageType = Enums.Parse<DamageType>(match.Groups[3].Value, true);
                    if (match.Groups[2].Value == "minimum")
                        hitDamageMinimum = stat.Value;
                    else
                        hitDamageMaximum = stat.Value;
                    continue;
                }
                match = DamageOverTimeRegex.Match(stat.StatId);
                if (match.Success)
                {
                    var type = Enums.Parse<DamageType>(match.Groups[1].Value, true);
                    var statBuilder = _builderFactories.DamageTypeBuilders.From(type).Damage
                        .WithSkills(DamageSource.OverTime);
                    localModifiers.Add(Modifier(statBuilder,
                        Forms.BaseSet, stat.Value / 60D, isMainSkill));
                    continue;
                }
                if (stat.StatId == "base_skill_number_of_additional_hits")
                {
                    globalModifiers.Add(Modifier(_metaStatBuilders.SkillNumberOfHitsPerCast,
                        Forms.BaseAdd, CreateValue(stat.Value), isMainSkill));
                    continue;
                }
                if (stat.StatId == "skill_double_hits_when_dual_wielding")
                {
                    globalModifiers.Add(Modifier(_metaStatBuilders.SkillDoubleHitsWhenDualWielding,
                        Forms.TotalOverride, CreateValue(stat.Value), isMainSkill));
                    continue;
                }
                unparsedStats.Add(stat);
            }
            if (hitDamageMaximum.HasValue)
            {
                var statBuilder = _builderFactories.DamageTypeBuilders.From(hitDamageType.Value).Damage
                    .WithSkills(hitDamageSource.Value);
                var valueBuilder = _builderFactories.ValueBuilders.FromMinAndMax(CreateValue(hitDamageMinimum),
                    CreateValue(hitDamageMaximum.Value));
                localModifiers.Add(Modifier(statBuilder, Forms.BaseSet, valueBuilder, isMainSkill));
            }
            return (localModifiers, globalModifiers, unparsedStats);
        }

        private IIntermediateModifier Modifier(
            IStatBuilder stat, IFormBuilder form, double value, IConditionBuilder condition)
            => Modifier(stat, form, CreateValue(value), condition);

        private IIntermediateModifier Modifier(
            IStatBuilder stat, IFormBuilder form, IValueBuilder value, IConditionBuilder condition)
            => _modifierBuilder.WithStat(stat).WithForm(form).WithValue(value).WithCondition(condition).Build();

        private IIntermediateModifier Modifier(IStatBuilder stat, IFormBuilder form, double value)
            => Modifier(stat, form, CreateValue(value));

        private IIntermediateModifier Modifier(IStatBuilder stat, IFormBuilder form, IValueBuilder value)
            => _modifierBuilder.WithStat(stat).WithForm(form).WithValue(value).Build();

        private IFormBuilders Forms => _builderFactories.FormBuilders;
        private IValueBuilder CreateValue(double value) => _builderFactories.ValueBuilders.Create(value);

        private IEquipmentBuilder MainHand => Equipment[ItemSlot.MainHand];
        private IEquipmentBuilder OffHand => Equipment[ItemSlot.OffHand];
        private IEquipmentBuilderCollection Equipment => _builderFactories.EquipmentBuilders.Equipment;
    }

    public struct Skill
    {
        public Skill(string id, int level, int quality, ItemSlot itemSlot, int socketIndex, int? gemGroup)
            => (Id, Level, Quality, ItemSlot, SocketIndex, GemGroup) =
                (id, level, quality, itemSlot, socketIndex, gemGroup);

        public string Id { get; }
        public int Level { get; }
        public int Quality { get; }

        public ItemSlot ItemSlot { get; }

        // Null: item inherent skill
        public int? GemGroup { get; }

        public int SocketIndex { get; }
    }
}