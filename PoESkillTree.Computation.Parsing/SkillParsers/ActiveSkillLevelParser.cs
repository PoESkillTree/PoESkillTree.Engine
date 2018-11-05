﻿using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Damage;
using PoESkillTree.Computation.Common.Builders.Modifiers;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.Computation.Common.Builders.Values;
using PoESkillTree.GameModel.Skills;

namespace PoESkillTree.Computation.Parsing.SkillParsers
{
    public class ActiveSkillLevelParser : IPartialSkillParser
    {
        private readonly IBuilderFactories _builderFactories;
        private readonly IMetaStatBuilders _metaStatBuilders;
        private readonly IModifierBuilder _modifierBuilder = new ModifierBuilder();

        private List<Modifier> _modifiers;
        private SkillPreParseResult _preParseResult;

        public ActiveSkillLevelParser(IBuilderFactories builderFactories, IMetaStatBuilders metaStatBuilders)
            => (_builderFactories, _metaStatBuilders) = (builderFactories, metaStatBuilders);

        public PartialSkillParseResult Parse(Skill mainSkill, Skill parsedSkill, SkillPreParseResult preParseResult)
        {
            _modifiers = new List<Modifier>();
            _preParseResult = preParseResult;
            var level = preParseResult.LevelDefinition;

            if (level.DamageEffectiveness is double effectiveness)
            {
                AddMainSkillModifier(_metaStatBuilders.DamageBaseAddEffectiveness, Form.TotalOverride, effectiveness);
            }
            if (level.DamageMultiplier is double multiplier)
            {
                AddMainSkillModifier(_metaStatBuilders.DamageBaseSetEffectiveness, Form.TotalOverride, multiplier);
            }
            if (level.CriticalStrikeChance is double crit &&
                preParseResult.HitDamageSource is DamageSource hitDamageSource)
            {
                AddMainSkillModifier(_builderFactories.ActionBuilders.CriticalStrike.Chance.With(hitDamageSource),
                    Form.BaseSet, crit);
            }
            if (level.ManaCost is int cost)
            {
                var costStat = _metaStatBuilders.SkillBaseCost(parsedSkill.ItemSlot, parsedSkill.SocketIndex);
                AddModifier(costStat, Form.BaseSet, cost);
                AddMainSkillModifier(_builderFactories.StatBuilders.Pool.From(Pool.Mana).Cost, Form.BaseSet,
                    costStat.Value);
                ParseReservation(mainSkill, costStat);
            }
            if (level.Cooldown is int cooldown)
            {
                AddMainSkillModifier(_builderFactories.StatBuilders.Cooldown, Form.BaseSet, cooldown);
            }

            var result = new PartialSkillParseResult(_modifiers, new UntranslatedStat[0]);
            _modifiers = null;
            _preParseResult = preParseResult;
            return result;
        }

        private void ParseReservation(Skill skill, IStatBuilder costStat)
        {
            var isReservation = _metaStatBuilders
                .SkillHasType(skill.ItemSlot, skill.SocketIndex, ActiveSkillType.ManaCostIsReservation).IsSet;
            var isReservationAndActive = isReservation.And(_preParseResult.IsActiveSkill);
            var isPercentage = _metaStatBuilders
                .SkillHasType(skill.ItemSlot, skill.SocketIndex, ActiveSkillType.ManaCostIsPercentage).IsSet;
            var skillBuilder = _builderFactories.SkillBuilders.FromId(_preParseResult.SkillDefinition.Id);

            AddModifier(skillBuilder.Reservation, Form.BaseSet, costStat.Value, isReservationAndActive);

            foreach (var pool in Enums.GetValues<Pool>())
            {
                var poolBuilder = _builderFactories.StatBuilders.Pool.From(pool);
                var value = skillBuilder.Reservation.Value;
                value = _builderFactories.ValueBuilders
                    .If(isPercentage).Then(value.AsPercentage * poolBuilder.Value)
                    .Else(value);
                AddModifier(poolBuilder.Reservation, Form.BaseAdd, value,
                    skillBuilder.ReservationPool.Value.Eq((double) pool).And(isReservationAndActive));
            }
        }

        private void AddMainSkillModifier(IStatBuilder stat, Form form, double value)
            => AddModifier(stat, form, value, _preParseResult.IsMainSkill.IsSet);

        private void AddMainSkillModifier(IStatBuilder stat, Form form, IValueBuilder value)
            => AddModifier(stat, form, value, _preParseResult.IsMainSkill.IsSet);

        private void AddModifier(IStatBuilder stat, Form form, double value, IConditionBuilder condition = null)
            => AddModifier(stat, form, _builderFactories.ValueBuilders.Create(value), condition);

        private void AddModifier(IStatBuilder stat, Form form, IValueBuilder value, IConditionBuilder condition = null)
        {
            var builder = _modifierBuilder
                .WithStat(stat)
                .WithForm(_builderFactories.FormBuilders.From(form))
                .WithValue(value);
            if (condition != null)
                builder = builder.WithCondition(condition);
            var intermediateModifier = builder.Build();
            _modifiers.AddRange(intermediateModifier.Build(_preParseResult.GlobalSource, Entity.Character));
        }
    }
}