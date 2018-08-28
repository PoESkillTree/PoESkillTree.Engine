﻿using System;
using System.Collections.Generic;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Common.Builders.Damage;
using PoESkillTree.Computation.Common.Builders.Effects;
using PoESkillTree.Computation.Common.Builders.Skills;
using PoESkillTree.Computation.Common.Builders.Stats;

namespace PoESkillTree.Computation.Builders.Stats
{
    public interface IStatFactory
    {
        IStat FromIdentity(string identity, Entity entity, Type dataType,
            ExplicitRegistrationType explicitRegistrationType = null);

        IStat CopyWithSuffix(IStat stat, string identitySuffix, Type dataType,
            ExplicitRegistrationType explicitRegistrationType = null);

        IStat ChanceToDouble(IStat stat);

        IEnumerable<IStat> ConvertTo(IStat sourceStat, IEnumerable<IStat> targetStats);
        IEnumerable<IStat> GainAs(IStat sourceStat, IEnumerable<IStat> targetStats);
        IStat ConvertTo(IStat source, IStat target);
        IStat GainAs(IStat source, IStat target);
        IStat Conversion(IStat source);
        IStat SkillConversion(IStat source);

        IStat Regen(Entity entity, Pool pool);
        IStat RegenTargetPool(Entity entity, Pool regenPool);
        IStat LeechTargetPool(Entity entity, Pool leechPool);

        IStat ActiveSkillId(Entity entity);
        IStat ActiveSkillPartHasKeyword(Entity entity, Keyword keyword);
        IStat ActiveSkillPartCastSpeedHasKeyword(Entity entity, Keyword keyword);
        IStat ActiveSkillPartDamageHasKeyword(Entity entity, Keyword keyword, DamageSource damageSource);

        IStat BuffEffect(Entity source, Entity target, string buffIdentity);        
        IStat BuffIsActive(Entity target, string buffIdentity);
        IStat BuffSourceIs(Entity source, Entity target, string buffIdentity);

        IStat Damage(Entity entity, DamageType damageType);
        IStat ConcretizeDamage(IStat stat, IDamageSpecification damageSpecification);
        IStat ApplyModifiersToSkillDamage(IStat stat, DamageSource damageSource, Form form);
        IStat ApplyModifiersToAilmentDamage(IStat stat, Form form);
        IStat DamageTaken(IStat damage);
        IStat AilmentDealtDamageType(Entity entity, Ailment ailment);
    }
}