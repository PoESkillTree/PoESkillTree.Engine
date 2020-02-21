using System;
using System.Linq;
using EnumsNET;
using PoESkillTree.Engine.Computation.Builders.Values;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.Computation.Common.Builders.Damage;
using PoESkillTree.Engine.Computation.Common.Builders.Effects;
using PoESkillTree.Engine.Computation.Common.Builders.Stats;
using PoESkillTree.Engine.Computation.Common.Builders.Values;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.GameModel.Skills;
using PoESkillTree.Engine.Utils.Extensions;

namespace PoESkillTree.Engine.Computation.Builders.Stats
{
    public class MetaStatBuilders : StatBuildersBase, IMetaStatBuilders
    {
        public MetaStatBuilders(IStatFactory statFactory) : base(statFactory)
        {
        }

        public ValueBuilder RegenTargetPoolValue(Pool sourcePool) =>
            new ValueBuilder(new ValueBuilderImpl(
                ps => BuildTargetPoolValue(ps, StatFactory.RegenTargetPool(ps.ModifierSourceEntity, sourcePool)),
                _ => RegenTargetPoolValue(sourcePool)));

        private IValue BuildTargetPoolValue(BuildParameters parameters, IStat targetPoolStat)
        {
            var entity = parameters.ModifierSourceEntity;
            var targetPoolValue = new StatValue(targetPoolStat);
            return new FunctionalValue(
                c => c.GetValue(TargetPoolValueStat(targetPoolValue.Calculate(c))),
                $"Value of Pool {targetPoolValue}");

            IStat TargetPoolValueStat(NodeValue? targetPool)
            {
                var targetPoolString = ((Pool) targetPool.Single()).ToString();
                return StatFactory.FromIdentity(targetPoolString, entity, typeof(uint));
            }
        }

        public IStatBuilder EffectiveRegen(Pool pool) => FromIdentity($"{pool}.EffectiveRegen", typeof(double));
        public IStatBuilder EffectiveRecharge(Pool pool) => FromIdentity($"{pool}.EffectiveRecharge", typeof(double));
        public IStatBuilder RechargeStartDelay(Pool pool) => FromIdentity($"{pool}.RechargeStartDelay", typeof(double));

        public IStatBuilder EffectiveLeechRate(Pool pool) => FromIdentity($"{pool}.Leech.EffectiveRate", typeof(uint));

        public IStatBuilder AbsoluteLeechRate(Pool pool) => FromIdentity($"{pool}.Leech.AbsoluteRate", typeof(double));

        public IStatBuilder AbsoluteLeechRateLimit(Pool pool)
            => FromIdentity($"{pool}.Leech.AbsoluteRateLimit", typeof(double));

        public IStatBuilder TimeToReachLeechRateLimit(Pool pool)
            => FromIdentity($"{pool}.Leech.SecondsToReachRateLimit", typeof(double));


        public IDamageRelatedStatBuilder Damage(DamageType damageType)
            => DamageRelatedFromIdentity($"{damageType}.Damage", typeof(double));

        public IDamageRelatedStatBuilder EnemyResistanceFromArmourAgainstNonCrits => DamageRelatedFromIdentity(typeof(double)).WithHits;
        public IDamageRelatedStatBuilder EnemyResistanceFromArmourAgainstCrits => DamageRelatedFromIdentity(typeof(double)).WithHits;

        public IDamageRelatedStatBuilder EnemyResistanceAgainstNonCrits(DamageType damageType)
            => DamageRelatedFromIdentity($"{damageType}.EnemyResistance.NonCrits", typeof(double)).WithHits;

        public IDamageRelatedStatBuilder EnemyResistanceAgainstCrits(DamageType damageType)
            => DamageRelatedFromIdentity($"{damageType}.EnemyResistance.Crits", typeof(double)).WithHits;

        public IDamageRelatedStatBuilder EffectiveDamageMultiplierWithNonCrits(DamageType damageType)
            => DamageRelatedFromIdentity($"{damageType}.EffectiveDamageMultiplier.NonCrits", typeof(double));

        public IDamageRelatedStatBuilder EffectiveDamageMultiplierWithCrits(DamageType damageType)
            => DamageRelatedFromIdentity($"{damageType}.EffectiveDamageMultiplier.Crits", typeof(double))
                .WithHitsAndAilments;

        public IDamageRelatedStatBuilder DamageWithNonCrits(DamageType damageType)
            => DamageRelatedFromIdentity($"{damageType}.Damage.NonCrits", typeof(double));

        public IDamageRelatedStatBuilder DamageWithCrits(DamageType damageType)
            => DamageRelatedFromIdentity($"{damageType}.Damage.Crits", typeof(double));

        public IDamageRelatedStatBuilder DamageWithNonCrits()
            => DamageRelatedFromIdentity("Damage.NonCrits", typeof(double));

        public IDamageRelatedStatBuilder DamageWithCrits()
            => DamageRelatedFromIdentity("Damage.Crits", typeof(double));

        public IDamageRelatedStatBuilder AverageDamagePerHit
            => DamageRelatedFromIdentity(typeof(double)).WithHits;

        public IDamageRelatedStatBuilder AverageDamage => DamageRelatedFromIdentity(typeof(double));
        public IStatBuilder AverageHitDamage => FromIdentity("AverageDamage.Hit", typeof(double));
        public IStatBuilder SkillDpsWithHits => FromIdentity("DPS.Hit", typeof(double));
        public IStatBuilder SkillDpsWithDoTs => FromIdentity("DPS.OverTime", typeof(double));

        public IStatBuilder AverageAilmentDamage(Ailment ailment)
            => FromIdentity($"AverageDamage.{ailment}", typeof(double));

        public IStatBuilder AilmentInstanceLifetimeDamage(Ailment ailment)
            => FromIdentity($"InstanceLifetimeDamage.{ailment}", typeof(double));

        public IStatBuilder AilmentDps(Ailment ailment)
            => FromIdentity($"DPS.{ailment}", typeof(double));

        public IStatBuilder ImpaleRecordedDamage => FromIdentity(typeof(double));
        public IDamageRelatedStatBuilder EnemyResistanceAgainstNonCritImpales => DamageRelatedFromIdentity(typeof(double)).WithHits;
        public IDamageRelatedStatBuilder EnemyResistanceAgainstCritImpales => DamageRelatedFromIdentity(typeof(double)).WithHits;

        public IDamageRelatedStatBuilder ImpaleDamageMultiplier => DamageRelatedFromIdentity(typeof(double)).WithHits;
        public IDamageRelatedStatBuilder EffectiveImpaleDamageMultiplierAgainstNonCrits => DamageRelatedFromIdentity(typeof(double)).WithHits;
        public IDamageRelatedStatBuilder EffectiveImpaleDamageMultiplierAgainstCrits => DamageRelatedFromIdentity(typeof(double)).WithHits;


        public IStatBuilder CastRate => FromIdentity(typeof(double));
        public IStatBuilder CastTime => FromIdentity(typeof(double));

        public IStatBuilder AilmentDealtDamageType(Ailment ailment)
            => FromFactory(e => StatFactory.AilmentDealtDamageType(e, ailment));

        public IDamageRelatedStatBuilder AilmentChanceWithCrits(Ailment ailment)
            => DamageRelatedFromIdentity($"{ailment}.ChanceWithCrits", typeof(double)).WithHits;

        public IDamageRelatedStatBuilder AilmentEffectiveChance(Ailment ailment)
            => DamageRelatedFromIdentity($"{ailment}.EffectiveChance", typeof(double)).WithHits;

        public IStatBuilder AilmentCombinedEffectiveChance(Ailment ailment)
            => FromIdentity($"{ailment}.EffectiveChance", typeof(double));

        public IStatBuilder AilmentEffectiveInstances(Ailment ailment)
            => FromIdentity($"{ailment}.EffectiveInstances", typeof(double));

        public IStatBuilder IncreasedDamageTakenFromShocks
            => FromIdentity("Shock.IncreasedDamageTaken", typeof(uint),
                ExplicitRegistrationTypes.UserSpecifiedValue(15));

        public IStatBuilder ReducedActionSpeedFromChill
            => FromIdentity("Chill.ReducedActionSpeed", typeof(uint),
                ExplicitRegistrationTypes.UserSpecifiedValue(10));

        public IDamageRelatedStatBuilder EffectiveCritChance
            => DamageRelatedFromIdentity("CriticalStrike.EffectiveChance", typeof(double)).WithHits;


        public IStatBuilder ResistanceAgainstHits(DamageType damageType)
            => FromIdentity($"{damageType}.ResistanceAgainstHits", typeof(double));

        public IStatBuilder MitigationAgainstHits(DamageType damageType)
            => FromIdentity($"{damageType}.MitigationAgainstHits", typeof(double));

        public IStatBuilder MitigationAgainstDoTs(DamageType damageType)
            => FromIdentity($"{damageType}.MitigationAgainstDoTs", typeof(double));

        public IStatBuilder ChanceToAvoidMeleeAttacks => FromIdentity(typeof(uint));
        public IStatBuilder ChanceToAvoidProjectileAttacks => FromIdentity(typeof(uint));
        public IStatBuilder ChanceToAvoidSpells => FromIdentity(typeof(uint));

        public IDamageRelatedStatBuilder EffectiveStunThreshold
            => DamageRelatedFromIdentity("Stun.EffectiveThreshold", typeof(double)).WithHits;

        public IStatBuilder StunAvoidanceWhileCasting => FromIdentity("Stun.ChanceToAvoidWhileCasting", typeof(double));

        public IStatBuilder SkillHitDamageSource => FromIdentity(typeof(DamageSource));
        public IStatBuilder SkillUsesHand(AttackDamageHand hand) => FromIdentity($"SkillUses.{hand}", typeof(bool));
        public IStatBuilder SkillDoubleHitsWhenDualWielding => FromIdentity(typeof(bool));
        public IStatBuilder SkillDpsWithHitsCalculationMode => FromIdentity("DPS.Hit.CalculationMode", typeof(DpsCalculationMode));

        public IStatBuilder MainSkillId => FromFactory(StatFactory.MainSkillId);

        public IStatBuilder MainSkillHasKeyword(Keyword keyword)
            => FromFactory(e => StatFactory.MainSkillHasKeyword(e, keyword));

        public IStatBuilder MainSkillPartHasKeyword(Keyword keyword)
            => FromFactory(e => StatFactory.MainSkillPartHasKeyword(e, keyword));

        public IStatBuilder MainSkillPartCastRateHasKeyword(Keyword keyword)
            => FromFactory(e => StatFactory.MainSkillPartCastRateHasKeyword(e, keyword));

        public IStatBuilder MainSkillPartDamageHasKeyword(Keyword keyword, DamageSource damageSource)
            => FromFactory(e => StatFactory.MainSkillPartDamageHasKeyword(e, keyword, damageSource));

        public IStatBuilder MainSkillPartAilmentDamageHasKeyword(Keyword keyword)
            => FromFactory(e => StatFactory.MainSkillPartAilmentDamageHasKeyword(e, keyword));

        public IStatBuilder ActiveSkillItemSlot(string skillId)
            => FromFactory(e => StatFactory.ActiveSkillItemSlot(e, skillId));

        public IStatBuilder ActiveSkillSocketIndex(string skillId)
            => FromFactory(e => StatFactory.ActiveSkillSocketIndex(e, skillId));

        public IStatBuilder MainSkillItemSlot => FromIdentity(typeof(ItemSlot));
        public IStatBuilder MainSkillSocketIndex => FromIdentity(typeof(uint));

        public IStatBuilder SkillBaseCost(ItemSlot itemSlot, int socketIndex)
            => FromIdentity($"{itemSlot.GetName()}.{socketIndex}.Cost", typeof(uint));

        public IStatBuilder SkillHasType(ItemSlot itemSlot, int socketIndex, string activeSkillType)
            => FromIdentity($"{itemSlot.GetName()}.{socketIndex}.Type.{activeSkillType}", typeof(bool));

        public IStatBuilder ActiveCurses => FromIdentity(typeof(int));

        public ValueBuilder ActiveCurseIndex(int numericSkillId) => new ValueBuilder(new ValueBuilderImpl(
            ps => BuildActiveCurseIndex(ps, numericSkillId),
            _ => ps => BuildActiveCurseIndex(ps, numericSkillId)));

        private IValue BuildActiveCurseIndex(BuildParameters ps, int numericSkillId)
        {
            var activeCursesStat = ActiveCurses.Build(ps).SelectMany(r => r.Stats).Single();
            return new FunctionalValue(c => CalculateActiveCurseIndex(c, activeCursesStat, numericSkillId),
                $"CalculateActiveCurseIndex({activeCursesStat}, {numericSkillId})");
        }

        private static NodeValue? CalculateActiveCurseIndex(IValueCalculationContext context, IStat activeCurses, int numericSkillId)
        {
            var i = 0;
            foreach (var activeCurseId in context.GetValues(Form.BaseAdd, activeCurses).WhereNotNull())
            {
                if (activeCurseId == (NodeValue?) numericSkillId)
                    return (NodeValue?) i;
                i++;
            }

            return null;
        }

        public IStatBuilder DamageBaseAddEffectiveness => FromFactory(StatFactory.DamageBaseAddEffectiveness);
        public IStatBuilder DamageBaseSetEffectiveness => FromFactory(StatFactory.DamageBaseSetEffectiveness);

        public IStatBuilder CanBypassSkillCooldown => FromIdentity(typeof(bool));

        public IStatBuilder SelectedBandit => FromIdentity(typeof(Bandit));

        public IStatBuilder SelectedQuestPart
            => FromIdentity(typeof(QuestPart), ExplicitRegistrationTypes.UserSpecifiedValue((int) QuestPart.Epilogue));

        public IStatBuilder SelectedBossType
            => FromIdentity(typeof(BossType), ExplicitRegistrationTypes.UserSpecifiedValue((int) BossType.None));

        private IStatBuilder FromFactory(Func<Entity, IStat> factory)
            => new StatBuilder(StatFactory, new LeafCoreStatBuilder(factory));
    }
}