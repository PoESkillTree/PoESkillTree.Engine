﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MoreLinq;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders;
using PoESkillTree.Computation.Common.Builders.Conditions;
using PoESkillTree.Computation.Common.Builders.Damage;
using PoESkillTree.Computation.Common.Builders.Entities;
using PoESkillTree.Computation.Common.Builders.Resolving;
using PoESkillTree.Computation.Common.Builders.Skills;
using PoESkillTree.Computation.Common.Builders.Stats;
using PoESkillTree.Computation.Common.Builders.Values;
using PoESkillTree.GameModel;
using PoESkillTree.GameModel.Skills;
using static PoESkillTree.Computation.Common.ExplicitRegistrationTypes;

namespace PoESkillTree.Computation.Builders.Stats
{
    internal class StatBuilders : StatBuildersBase, IStatBuilders
    {
        public StatBuilders(IStatFactory statFactory) : base(statFactory)
        {
        }

        public IStatBuilder Level => FromIdentity(typeof(uint));
        public IStatBuilder CharacterClass => FromIdentity(typeof(CharacterClass));
        public IStatBuilder PassivePoints => FromIdentity(typeof(uint));
        public IStatBuilder AscendancyPassivePoints => FromIdentity(typeof(uint));

        public IStatBuilder Armour => FromIdentity(typeof(uint));

        public IEvasionStatBuilder Evasion => new EvasionStatBuilder(StatFactory);

        public IDamageRelatedStatBuilder Accuracy
            => DamageRelatedFromIdentity(typeof(uint)).WithSkills(DamageSource.Attack);

        public IDamageRelatedStatBuilder ChanceToHit
            => DamageRelatedFromIdentity(typeof(uint)).WithSkills(DamageSource.Attack);

        public IStatBuilder MovementSpeed => FromIdentity(typeof(double));
        public IStatBuilder ActionSpeed => FromIdentity(typeof(double));

        public IDamageRelatedStatBuilder CastRate => new CastRateStatBuilder(StatFactory);
        public IDamageRelatedStatBuilder BaseCastTime => DamageRelatedFromIdentity(typeof(double)).WithHits;
        public IStatBuilder HitRate => FromIdentity(typeof(double));

        public IStatBuilder DamageHasKeyword(DamageSource damageSource, IKeywordBuilder keyword)
        {
            var coreBuilder = new CoreStatBuilderFromCoreBuilder<Keyword>(
                CoreBuilder.Proxy(keyword, (ps, b) => b.Build(ps)),
                (e, k) => StatFactory.MainSkillPartDamageHasKeyword(e, k, damageSource));
            return new StatBuilder(StatFactory, coreBuilder);
        }

        public IStatBuilder AreaOfEffect => FromIdentity(typeof(int));
        public IStatBuilder Radius => FromIdentity(typeof(uint));

        public IDamageRelatedStatBuilder Range
            => DamageRelatedFromIdentity(typeof(uint)).WithSkills(DamageSource.Attack);

        public IStatBuilder Cooldown => FromIdentity(typeof(double));
        public IStatBuilder CooldownRecoverySpeed => FromIdentity(typeof(double));
        public IStatBuilder Duration => FromIdentity(typeof(double));
        public IStatBuilder SecondaryDuration => FromIdentity(typeof(double));
        public IStatBuilder SkillStage => FromIdentity(typeof(uint), UserSpecifiedValue(double.MaxValue));
        public IStatBuilder MainSkillPart => FromIdentity(typeof(uint));

        public ITrapStatBuilders Trap => new TrapStatBuilders(StatFactory);
        public IMineStatBuilders Mine => new MineStatBuilders(StatFactory);
        public ISkillEntityStatBuilders Totem => new TotemStatBuilders(StatFactory);

        public IStatBuilder ItemQuantity => FromIdentity(typeof(int));
        public IStatBuilder ItemRarity => FromIdentity(typeof(int));

        public IStatBuilder PrimordialJewelsSocketed => FromIdentity(typeof(uint));
        public IStatBuilder GrandSpectrumJewelsSocketed => FromIdentity(typeof(uint));

        public IStatBuilder RampageStacks => FromIdentity(typeof(uint));
        public IStatBuilder CharacterSize => FromIdentity(typeof(double));
        public IStatBuilder LightRadius => FromIdentity(typeof(double));
        public IStatBuilder AttachedBrands => FromIdentity(typeof(uint));

        public IStatBuilder PassiveNodeSkilled(ushort nodeId) => FromIdentity($"{nodeId}.Skilled", typeof(bool));

        public IStatBuilder DamageTakenGainedAsMana =>
            FromIdentity("% of damage taken gained as mana over 4 seconds", typeof(uint));

        public ValueBuilder UniqueAmount(string name)
            => FromIdentity(name, typeof(uint), UserSpecifiedValue(0)).Value;

        public IAttributeStatBuilders Attribute => new AttributeStatBuilders(StatFactory);
        public IRequirementStatBuilders Requirements => new RequirementStatBuilders(StatFactory);
        public IPoolStatBuilders Pool => new PoolStatBuilders(StatFactory);
        public IDodgeStatBuilders Dodge => new DodgeStatBuilders(StatFactory);
        public IFlaskStatBuilders Flask => new FlaskStatBuilders(StatFactory);
        public IProjectileStatBuilders Projectile => new ProjectileStatBuilders(StatFactory);
        public IFlagStatBuilders Flag => new FlagStatBuilders(StatFactory);
        public IGemStatBuilders Gem => new GemStatBuilders(StatFactory);
    }

    internal class TrapStatBuilders : StatBuildersBase, ITrapStatBuilders
    {
        public TrapStatBuilders(IStatFactory statFactory) : base(statFactory)
        {
        }

        public IStatBuilder Speed => FromIdentity("Trap throwing speed", typeof(double));
        public IStatBuilder Duration => FromIdentity("Trap duration", typeof(double));
        public IStatBuilder TriggerAoE => FromIdentity("Trap trigger AoE", typeof(int));
    }

    internal class MineStatBuilders : StatBuildersBase, IMineStatBuilders
    {
        public MineStatBuilders(IStatFactory statFactory) : base(statFactory)
        {
        }

        public IStatBuilder Speed => FromIdentity("Mine laying speed", typeof(double));
        public IStatBuilder Duration => FromIdentity("Mine duration", typeof(double));
        public IStatBuilder DetonationAoE => FromIdentity("Mine detonation AoE", typeof(int));
    }

    internal class TotemStatBuilders : StatBuildersBase, ISkillEntityStatBuilders
    {
        public TotemStatBuilders(IStatFactory statFactory) : base(statFactory)
        {
        }

        public IStatBuilder Speed => FromIdentity("Totem placement speed", typeof(double));
        public IStatBuilder Duration => FromIdentity("Totem duration", typeof(double));
    }

    internal class AttributeStatBuilders : StatBuildersBase, IAttributeStatBuilders
    {
        public AttributeStatBuilders(IStatFactory statFactory) : base(statFactory)
        {
        }

        public IStatBuilder Strength => FromIdentity(typeof(uint));
        public IStatBuilder Dexterity => FromIdentity(typeof(uint));
        public IStatBuilder Intelligence => FromIdentity(typeof(uint));
        public IStatBuilder StrengthDamageBonus => FromIdentity(typeof(uint));
        public IStatBuilder DexterityEvasionBonus => FromIdentity(typeof(uint));
    }

    internal class RequirementStatBuilders : StatBuildersBase, IRequirementStatBuilders
    {
        public RequirementStatBuilders(IStatFactory statFactory) : base(statFactory)
        {
        }

        public IStatBuilder Level => Requirement();
        public IStatBuilder Strength => Requirement();
        public IStatBuilder Dexterity => Requirement();
        public IStatBuilder Intelligence => Requirement();

        private IStatBuilder Requirement([CallerMemberName] string requiredStat = null)
            => FromStatFactory(e => StatFactory.Requirement(StatFactory.FromIdentity(requiredStat, e, typeof(uint))));
    }

    internal class DodgeStatBuilders : StatBuildersBase, IDodgeStatBuilders
    {
        public DodgeStatBuilders(IStatFactory statFactory) : base(statFactory)
        {
        }

        public IStatBuilder AttackChance => FromIdentity("Chance to dodge attacks", typeof(uint));
        public IStatBuilder SpellChance => FromIdentity("Chance to dodge spells", typeof(uint));
    }

    internal class FlaskStatBuilders : StatBuildersBase, IFlaskStatBuilders
    {
        public FlaskStatBuilders(IStatFactory statFactory) : base(statFactory)
        {
        }

        public IStatBuilder Effect => FromIdentity("Flask.Effect", typeof(int));
        public IStatBuilder Duration => FromIdentity("Flask.EffectDuration", typeof(double));
        public IStatBuilder LifeRecovery => FromIdentity("Flask.LifeRecovery", typeof(int));
        public IStatBuilder ManaRecovery => FromIdentity("Flask.ManaRecovery", typeof(int));
        public IStatBuilder RecoverySpeed => FromIdentity("Flask.RecoverySpeed", typeof(double));
        public IStatBuilder ChargesUsed => FromIdentity("Flask.ChargesUsed", typeof(int));
        public IStatBuilder ChargesGained => FromIdentity("Flask.ChargesGained", typeof(double));

        public IConditionBuilder IsAnyActive
            => FromIdentity("Is any flask active?", typeof(bool), UserSpecifiedValue(false)).IsSet;
    }

    internal class ProjectileStatBuilders : StatBuildersBase, IProjectileStatBuilders
    {
        public ProjectileStatBuilders(IStatFactory statFactory) : base(statFactory)
        {
        }

        public IStatBuilder Speed => FromIdentity("Projectile speed", typeof(int));
        public IStatBuilder Count => FromIdentity("Projectile count", typeof(uint));

        public IStatBuilder PierceCount => FromIdentity("Projectile pierce count", typeof(uint));
        public IStatBuilder ChainCount => FromIdentity("Projectile chain count", typeof(uint));
        public IStatBuilder Fork => FromIdentity("Projectile.Fork", typeof(bool));

        public ValueBuilder TravelDistance =>
            FromIdentity("Projectile travel distance", typeof(uint), UserSpecifiedValue(35)).Value;
    }

    internal class FlagStatBuilders : StatBuildersBase, IFlagStatBuilders
    {
        public FlagStatBuilders(IStatFactory statFactory) : base(statFactory)
        {
        }

        public IStatBuilder IgnoreMovementSpeedPenalties =>
            FromIdentity("Ignore movement speed penalties from equipped armor", typeof(bool));

        public IStatBuilder ShieldModifiersApplyToMinionsInstead =>
            FromIdentity("Modifiers on an equipped shield apply to your minions instead", typeof(bool));

        public IStatBuilder IgnoreHexproof => FromIdentity(typeof(bool));
        public IStatBuilder CriticalStrikeChanceIsLucky => FromIdentity(typeof(bool));
        public IStatBuilder FarShot => FromIdentity(typeof(bool));

        public IConditionBuilder AlwaysMoving
            => FromIdentity("Are you always moving?", typeof(bool), UserSpecifiedValue(false)).IsSet;

        public IConditionBuilder AlwaysStationary
            => FromIdentity("Are you always stationary?", typeof(bool), UserSpecifiedValue(false)).IsSet;

        public IConditionBuilder IsBrandAttachedToEnemy
            => FromIdentity("Is your Brand attached to an enemy?", typeof(bool), UserSpecifiedValue(false)).IsSet;

        public IConditionBuilder IsBannerPlanted
            => FromIdentity("Is your Banner planted?", typeof(bool), UserSpecifiedValue(false)).IsSet;

        public IStatBuilder IncreasesToSourceApplyToTarget(IStatBuilder source, IStatBuilder target)
            => new StatBuilder(StatFactory,
                new ModifiersApplyToOtherStatCoreStatBuilder(source, target, Form.Increase, StatFactory));

        private class ModifiersApplyToOtherStatCoreStatBuilder : ICoreStatBuilder
        {
            private readonly IStatBuilder _target;
            private readonly IStatBuilder _source;
            private readonly Form _form;
            private readonly IStatFactory _statFactory;

            public ModifiersApplyToOtherStatCoreStatBuilder(
                IStatBuilder source, IStatBuilder target, Form form, IStatFactory statFactory)
                => (_target, _source, _form, _statFactory) = (target, source, form, statFactory);

            public ICoreStatBuilder Resolve(ResolveContext context)
                => new ModifiersApplyToOtherStatCoreStatBuilder(
                    _source.Resolve(context), _target.Resolve(context), _form, _statFactory);

            public ICoreStatBuilder WithEntity(IEntityBuilder entityBuilder)
                => new ModifiersApplyToOtherStatCoreStatBuilder(
                    _source.For(entityBuilder), _target.For(entityBuilder), _form, _statFactory);

            public IEnumerable<StatBuilderResult> Build(BuildParameters parameters)
            {
                return _source.Build(parameters).EquiZip(_target.Build(parameters), MergeResults);

                StatBuilderResult MergeResults(StatBuilderResult source, StatBuilderResult target)
                {
                    var mergedStats = source.Stats.EquiZip(target.Stats,
                        (s, t) => _statFactory.StatIsAffectedByModifiersToOtherStat(t, s, _form));
                    return new StatBuilderResult(mergedStats.ToList(), source.ModifierSource, source.ValueConverter);
                }
            }
        }
    }

    internal class GemStatBuilders : StatBuildersBase, IGemStatBuilders
    {
        public GemStatBuilders(IStatFactory statFactory) : base(statFactory)
        {
        }

        public IStatBuilder IncreaseSupportLevel => FromIdentity("Level of socketed support gems", typeof(int));
    }
}