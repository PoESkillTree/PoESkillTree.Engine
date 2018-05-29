﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace PoESkillTree.Computation.Common
{
    /// <summary>
    /// Each instance represents one calculation subgraph.
    /// <para>
    /// <see cref="object.Equals(object)"/> and <see cref="IEquatable{T}.Equals(T)"/> return <c>true</c> if the
    /// parameter is an <see cref="IStat"/> instance representing the same calculation subgraph.
    /// </para>
    /// </summary>
    public interface IStat : IEquatable<IStat>
    {
        /// <summary>
        /// Returns a string naming the represented calculation subgraph.
        /// </summary>
        string ToString();

        /// <summary>
        /// The <see cref="IStat"/> determining the minimum value of this stat or<c>null</c> if the stat can never
        /// have an lower bound.
        /// </summary>
        [CanBeNull]
        IStat Minimum { get; }
        
        /// <summary>
        /// The <see cref="IStat"/> determining the maximum value of this stat or <c>null</c> if the stat can never
        /// have an upper bound.
        /// </summary>
        [CanBeNull]
        IStat Maximum { get; }

        /// <summary>
        /// True if the existence/usage of this stat should be explicitly announced to clients
        /// </summary>
        bool IsRegisteredExplicitly { get; }

        /// <summary>
        /// The type of this stat's values. Can be double, int or bool (0 or 1).
        /// The value range is determined by Minimum and Maximum (which have the same DataType).
        /// </summary>
        Type DataType { get; }

        /// <summary>
        /// The behaviors that should be applied to the calculation graph when this stat's subgraph is created.
        /// </summary>
        IEnumerable<Behavior> Behaviors { get; }
    }
}