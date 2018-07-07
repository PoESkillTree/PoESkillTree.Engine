﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PoESkillTree.Computation.Common;

namespace PoESkillTree.Computation.Builders.Stats
{
    public class Stat : IStat
    {
        public Stat(string identity, Entity entity = default, Type dataType = null,
            ExplicitRegistrationType explicitRegistrationType = null, IReadOnlyList<Behavior> behaviors = null,
            bool hasRange = true)
        {
            if (!IsDataTypeValid(dataType))
                throw new ArgumentException($"Stats only support double, int, bool or enum data types, {dataType} given",
                    nameof(dataType));

            Identity = identity;
            _hasRange = hasRange;
            Entity = entity;
            ExplicitRegistrationType = explicitRegistrationType;
            DataType = dataType ?? typeof(double);
            Behaviors = behaviors ?? new Behavior[0];
        }

        private static bool IsDataTypeValid(Type dataType)
        {
            return dataType == null
                   || dataType == typeof(int) || dataType == typeof(double) || dataType == typeof(bool)
                   || dataType.IsEnum;
        }

        private readonly bool _hasRange;
        public string Identity { get; }
        public Entity Entity { get; }
        public ExplicitRegistrationType ExplicitRegistrationType { get; }
        public Type DataType { get; }
        public IReadOnlyList<Behavior> Behaviors { get; }

        public IStat Minimum => MinOrMax();
        public IStat Maximum => MinOrMax();

        private IStat MinOrMax([CallerMemberName] string identitySuffix = null) =>
            _hasRange ? CopyWithSuffix(identitySuffix, hasRange: false) : null;

        private IStat CopyWithSuffix(string identitySuffix, bool hasRange = true) =>
            new Stat(Identity + "." + identitySuffix, Entity, DataType, hasRange: hasRange);

        public override string ToString() => Entity + "." + Identity;

        public override bool Equals(object obj) =>
            (obj == this) || (obj is IStat other && Equals(other));

        public bool Equals(IStat other) =>
            (other != null) && Identity.Equals(other.Identity) && Entity == other.Entity;

        public override int GetHashCode() =>
            (Identity, Entity).GetHashCode();
    }
}