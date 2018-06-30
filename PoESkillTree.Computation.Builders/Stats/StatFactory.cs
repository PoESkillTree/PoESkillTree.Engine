﻿using System;
using System.Collections.Generic;
using PoESkillTree.Common.Utils.Extensions;
using PoESkillTree.Computation.Builders.Behaviors;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders.Damage;
using PoESkillTree.Computation.Common.Builders.Effects;
using PoESkillTree.Computation.Common.Builders.Skills;
using PoESkillTree.Computation.Common.Builders.Stats;

namespace PoESkillTree.Computation.Builders.Stats
{
    public class StatFactory : IStatFactory
    {
        private readonly IDictionary<(string, Entity), IStat> _cache = new Dictionary<(string, Entity), IStat>();
        private readonly BehaviorFactory _behaviorFactory;

        public StatFactory()
        {
            _behaviorFactory = new BehaviorFactory(this);
        }

        public IStat FromIdentity(string identity, Entity entity, Type dataType, bool isExplicitlyRegistered = false) =>
            GetOrAdd(identity, entity, dataType, isExplicitlyRegistered);

        public IStat ChanceToDouble(IStat stat) =>
            CopyWithSuffix(stat, nameof(ChanceToDouble), typeof(int));

        public IEnumerable<IStat> ConvertTo(IStat source, IEnumerable<IStat> targets)
        {
            foreach (var target in targets)
            {
                yield return ConvertTo(source, target);
            }
            yield return Conversion(source);
            yield return SkillConversion(source);
        }

        public IEnumerable<IStat> GainAs(IStat source, IEnumerable<IStat> targets)
        {
            foreach (var target in targets)
            {
                yield return GainAs(source, target);
            }
        }

        public IStat ConvertTo(IStat source, IStat target) =>
            CopyWithSuffix(source, $"{nameof(ConvertTo)}({target.Identity})", typeof(int),
                behaviors: _behaviorFactory.ConvertTo(source, target));

        public IStat GainAs(IStat source, IStat target) =>
            CopyWithSuffix(source, $"{nameof(GainAs)}({target.Identity})", typeof(int),
                behaviors: _behaviorFactory.GainAs(source, target));

        public IStat Conversion(IStat source) =>
            CopyWithSuffix(source, "Conversion", typeof(int));

        public IStat SkillConversion(IStat source) =>
            CopyWithSuffix(source, "SkillConversion", typeof(int),
                behaviors: _behaviorFactory.SkillConversion(source));

        public IStat Regen(Pool pool, Entity entity) =>
            GetOrAdd($"{pool}.Regen", entity, typeof(double), behaviors: _behaviorFactory.Regen(pool, entity));

        public IStat RegenTargetPool(Pool regenPool, Entity entity) =>
            GetOrAdd($"{regenPool}.Regen.TargetPool", entity, typeof(Pool));

        public IStat LeechPercentage(IStat damage) =>
            CopyWithSuffix(damage, "Leech", typeof(int));

        public IStat ActiveSkillId(Entity entity) =>
            GetOrAdd("ActiveSkill.Id", entity, typeof(int));

        public IStat ActiveSkillPartHasKeyword(Entity entity, Keyword keyword) =>
            GetOrAdd($"ActiveSkillPart.Has.{keyword}", entity, typeof(bool));

        public IStat ActiveSkillPartCastSpeedHasKeyword(Entity entity, Keyword keyword) =>
            GetOrAdd($"ActiveSkillPart.CastSpeed.Has.{keyword}", entity, typeof(bool));

        public IStat Damage(DamageType damageType, Entity entity) =>
            GetOrAdd($"{damageType}.Damage", entity, typeof(int));

        public IStat ConcretizeDamage(IStat stat, IDamageSpecification damageSpecification) =>
            CopyWithSuffix(stat, damageSpecification.StatIdentitySuffix, stat.DataType,
                behaviors: _behaviorFactory.ConcretizeDamage(stat, damageSpecification));

        public IStat ApplyModifiersToSkillDamage(IStat stat, DamageSource damageSource, Form form) =>
            CopyWithSuffix(stat, $"ApplyModifiersToSkills({damageSource} for form {form})", typeof(int));

        public IStat ApplyModifiersToAilmentDamage(IStat stat, Form form) =>
            CopyWithSuffix(stat, $"ApplyModifiersToAilments(for form {form})", typeof(int));

        public IStat DamageTaken(IStat damage) =>
            CopyWithSuffix(damage, "Taken", damage.DataType);

        public IStat AilmentDealtDamageType(Ailment ailment, Entity entity) =>
            GetOrAdd($"{ailment}.DamageType", entity, typeof(DamageType));

        private IStat CopyWithSuffix(IStat source, string identitySuffix, Type dataType,
            bool isRegisteredExplicitly = false, IReadOnlyList<Behavior> behaviors = null)
        {
            return GetOrAdd(source.Identity + "." + identitySuffix, source.Entity,
                dataType, isRegisteredExplicitly, behaviors);
        }

        private IStat GetOrAdd(string identity, Entity entity, Type dataType,
            bool isRegisteredExplicitly = false, IReadOnlyList<Behavior> behaviors = null)
        {
            return _cache.GetOrAdd((identity, entity), _ =>
                new Stat(identity, entity, dataType, isRegisteredExplicitly, behaviors));
        }
    }
}