using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Conditions;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Equipment;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.GameModel.Skills;

namespace PoESkillTree.Engine.Computation.Parsing.SkillParsers
{
    /// <summary>
    /// Partial parser of <see cref="ActiveSkillParser"/> that parses general modifiers like used AttackDamageHand,
    /// skill id and cast time that don't fit any other partial parser.
    /// </summary>
    public class ActiveSkillGeneralParser : IPartialSkillParser
    {
        private readonly IBuilderFactories _builderFactories;

        private SkillModifierCollection? _parsedModifiers;

        public ActiveSkillGeneralParser(IBuilderFactories builderFactories)
            => _builderFactories = builderFactories;

        private IMetaStatBuilders MetaStats => _builderFactories.MetaStatBuilders;

        public PartialSkillParseResult Parse(Skill mainSkill, Skill parsedSkill, SkillPreParseResult preParseResult)
        {
            _parsedModifiers = new SkillModifierCollection(_builderFactories,
                preParseResult.IsMainSkill, preParseResult.LocalSource, preParseResult.ModifierSourceEntity);
            var activeSkill = preParseResult.SkillDefinition.ActiveSkill;
            var isMainSkill = preParseResult.IsMainSkill;
            var isActiveSkill = preParseResult.IsActiveSkill;

            AddHitDamageSourceModifiers(preParseResult);

            var (usesMainHandCondition, usesOffHandCondition) = GetUsesHandConditions(isMainSkill, activeSkill);
            _parsedModifiers.AddGlobal(MetaStats.SkillUsesHand(AttackDamageHand.MainHand),
                Form.TotalOverride, 1, usesMainHandCondition);
            _parsedModifiers.AddGlobal(MetaStats.SkillUsesHand(AttackDamageHand.OffHand),
                Form.TotalOverride, 1, usesOffHandCondition);

            _parsedModifiers.AddGlobalForMainSkill(MetaStats.MainSkillId,
                Form.TotalOverride, preParseResult.SkillDefinition.NumericId);
            _parsedModifiers.AddGlobalForMainSkill(_builderFactories.StatBuilders.MainSkillPart.Maximum,
                Form.TotalOverride, preParseResult.SkillDefinition.PartNames.Count - 1);

            _parsedModifiers.AddGlobalForMainSkill(_builderFactories.StatBuilders.BaseCastTime.With(DamageSource.Spell),
                Form.BaseSet, activeSkill.CastTime / 1000D);
            _parsedModifiers.AddGlobalForMainSkill(
                _builderFactories.StatBuilders.BaseCastTime.With(DamageSource.Secondary),
                Form.BaseSet, activeSkill.CastTime / 1000D);

            if (activeSkill.TotemLifeMultiplier is double lifeMulti)
            {
                var totemLifeStat = _builderFactories.StatBuilders.Pool.From(Pool.Life)
                    .For(_builderFactories.EntityBuilders.Totem);
                _parsedModifiers.AddGlobalForMainSkill(totemLifeStat, Form.More, (lifeMulti - 1) * 100);
            }

            _parsedModifiers.AddGlobal(MetaStats.ActiveSkillItemSlot(mainSkill.Id),
                Form.BaseSet, (double) mainSkill.ItemSlot);
            _parsedModifiers.AddGlobal(MetaStats.ActiveSkillSocketIndex(mainSkill.Id),
                Form.BaseSet, mainSkill.SocketIndex);

            if (activeSkill.ProvidesBuff)
            {
                AddBuffModifiers(preParseResult, activeSkill, isActiveSkill);
            }

            _parsedModifiers.AddGlobal(_builderFactories.SkillBuilders.FromId(mainSkill.Id).Instances,
                Form.BaseAdd, 1, isActiveSkill);
            _parsedModifiers.AddGlobal(_builderFactories.SkillBuilders.AllSkills.CombinedInstances,
                Form.BaseAdd, 1, isActiveSkill);
            foreach (var keyword in activeSkill.Keywords)
            {
                var keywordBuilder = _builderFactories.KeywordBuilders.From(keyword);
                _parsedModifiers.AddGlobal(_builderFactories.SkillBuilders[keywordBuilder].CombinedInstances,
                    Form.BaseAdd, 1, isActiveSkill);
            }

            var result = new PartialSkillParseResult(_parsedModifiers.Modifiers, new UntranslatedStat[0]);
            _parsedModifiers = null;
            return result;
        }

        private void AddHitDamageSourceModifiers(SkillPreParseResult preParseResult)
        {
            var partCount = preParseResult.LevelDefinition.AdditionalStatsPerPart.Count;

            for (var partIndex = 0; partIndex < partCount; partIndex++)
            {
                var damageSource = DetermineHitDamageSource(preParseResult, partIndex);
                if (damageSource != null)
                {
                    var condition =
                        partCount > 1 ? _builderFactories.StatBuilders.MainSkillPart.Value.Eq(partIndex) : null;
                    _parsedModifiers!.AddGlobalForMainSkill(MetaStats.SkillHitDamageSource,
                        Form.TotalOverride, (int) damageSource, condition);
                }
            }
        }

        private static DamageSource? DetermineHitDamageSource(SkillPreParseResult preParseResult, int partIndex)
        {
            var statIds = preParseResult.LevelDefinition.Stats
                .Concat(preParseResult.LevelDefinition.AdditionalStatsPerPart[partIndex])
                .Select(s => s.StatId).ToList();

            if (statIds.Any(s => s == SkillStatIds.DealsSecondaryDamage))
            {
                return DamageSource.Secondary;
            }

            if (preParseResult.SkillDefinition.ActiveSkill.ActiveSkillTypes.Contains(ActiveSkillType.Attack))
            {
                return DamageSource.Attack;
            }

            foreach (var statId in statIds)
            {
                var match = SkillStatIds.HitDamageRegex.Match(statId);
                if (match.Success)
                    return Enums.Parse<DamageSource>(match.Groups[1].Value, true);
            }
            return null;
        }

        private (IConditionBuilder usesMainHandCondition, IConditionBuilder usesOffHandCondition) GetUsesHandConditions(IConditionBuilder isMainSkill,
            ActiveSkillDefinition activeSkill)
        {
            var shieldOnly = activeSkill.WeaponRestrictions.Contains(ItemClass.Shield);
            var offHandHasWeapon = OffHand.Has(Tags.Weapon);
            var usesMainHandCondition = isMainSkill;
            var usesOffHandCondition = isMainSkill;
            if (activeSkill.ActiveSkillTypes.Contains(ActiveSkillType.RequiresDualWield))
                usesMainHandCondition = usesMainHandCondition.And(offHandHasWeapon);
            else if (activeSkill.ActiveSkillTypes.Contains(ActiveSkillType.RequiresShield))
                usesMainHandCondition = usesMainHandCondition.And(OffHand.Has(Tags.Shield));
            if (!shieldOnly)
            {
                usesOffHandCondition = usesOffHandCondition.And(offHandHasWeapon);
            }
            if (activeSkill.WeaponRestrictions.Any())
            {
                var suitableMainHand = CreateWeaponRestrictionCondition(MainHand, activeSkill.WeaponRestrictions);
                var suitableOffHand = CreateWeaponRestrictionCondition(OffHand, activeSkill.WeaponRestrictions);
                usesMainHandCondition = usesMainHandCondition.And(suitableMainHand);
                usesOffHandCondition = usesOffHandCondition.And(suitableOffHand);
                if (!shieldOnly)
                {
                    usesMainHandCondition = usesMainHandCondition.And(suitableOffHand.Or(offHandHasWeapon.Not));
                    usesOffHandCondition = usesOffHandCondition.And(suitableMainHand);
                }
            }

            return (usesMainHandCondition, usesOffHandCondition);
        }

        private static IConditionBuilder CreateWeaponRestrictionCondition(
            IEquipmentBuilder hand, IEnumerable<ItemClass> weaponRestrictions)
            => weaponRestrictions.Select(hand.Has).Aggregate((l, r) => l.Or(r));

        private void AddBuffModifiers(SkillPreParseResult preParseResult, ActiveSkillDefinition activeSkill, IConditionBuilder isActiveSkill)
        {
            var allBuffStats =
                preParseResult.LevelDefinition.BuffStats.Concat(preParseResult.LevelDefinition.QualityBuffStats);
            var allAffectedEntities = allBuffStats
                .SelectMany(s => s.GetAffectedEntities(preParseResult.ModifierSourceEntity))
                .Distinct().ToList();
            if (!allAffectedEntities.Any())
                return;

            var buff = _builderFactories.SkillBuilders.FromId(preParseResult.SkillDefinition.Id).Buff;
            var target = _builderFactories.EntityBuilders.From(allAffectedEntities);
            if (activeSkill.Keywords.Contains(Keyword.Curse))
            {
                var numericSkillId = preParseResult.SkillDefinition.NumericId;
                _parsedModifiers!.AddGlobal(MetaStats.ActiveCurses.For(target),
                    Form.BaseAdd, numericSkillId,
                    isActiveSkill.And(buff.IgnoresCurseLimit.IsSet.Not));
                _parsedModifiers.AddGlobal(buff.On(target),
                    Form.BaseSet, 1,
                    isActiveSkill.And(buff.IgnoresCurseLimit.IsSet
                        .Or(MetaStats.ActiveCurseIndex(numericSkillId) < _builderFactories.BuffBuilders.CurseLimit.Value)));
            }
            else
            {
                _parsedModifiers!.AddGlobal(buff.On(target), Form.BaseSet, 1, isActiveSkill);
            }

            if (allAffectedEntities.Contains(Entity.Enemy))
            {
                _parsedModifiers.AddGlobalForMainSkill(buff.Duration, Form.More,
                    100 / _builderFactories.EffectBuilders.ExpirationModifier.For(_builderFactories.EntityBuilders.MainOpponentOfSelf).Value - 100);
            }
        }

        private IEquipmentBuilder MainHand => Equipment[ItemSlot.MainHand];
        private IEquipmentBuilder OffHand => Equipment[ItemSlot.OffHand];
        private IEquipmentBuilderCollection Equipment => _builderFactories.EquipmentBuilders.Equipment;
    }
}