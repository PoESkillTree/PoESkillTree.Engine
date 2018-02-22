﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PoESkillTree.Computation.Common;

namespace PoESkillTree.Computation.Core.Tests
{
    [DebuggerDisplay("{" + nameof(_instance) + "}")]
    internal class StatStub : IStat
    {
        private static int _instanceCounter;

        private readonly int _instance;

        public StatStub(IStat minimum = null, IStat maximum = null)
        {
            _instance = _instanceCounter++;
            Minimum = minimum;
            Maximum = maximum;
        }

        public bool Equals(IStat other) => Equals((object) other);

        public IStat Minimum { get; }
        public IStat Maximum { get; }
        public bool IsRegisteredExplicitly { get; set; }
        public Type DataType => typeof(double);
        public IEnumerable<IBehavior> Behaviors => Enumerable.Empty<IBehavior>();
    }
}