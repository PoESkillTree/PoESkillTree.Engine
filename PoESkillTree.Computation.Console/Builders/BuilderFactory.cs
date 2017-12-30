﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PoESkillTree.Computation.Parsing.Builders.Conditions;
using PoESkillTree.Computation.Parsing.Builders.Matching;
using PoESkillTree.Computation.Parsing.Builders.Stats;
using PoESkillTree.Computation.Parsing.Builders.Values;

namespace PoESkillTree.Computation.Console.Builders
{
    public static class BuilderFactory
    {
        #region IValueBuilder

        // Identical to the Create<> methods at the bottom but with `new ValueBuilderStub()` as constructor.

        public static IValueBuilder CreateValue<T>(
            [CanBeNull] T operand,
            Func<T, string> stringRepresentation)
            where T : class, IResolvable<T>
        {
            return Create<IValueBuilder, T>(Construct, operand, stringRepresentation);
        }

        public static IValueBuilder CreateValue<T1, T2>(
            [CanBeNull] T1 operand1, [CanBeNull] T2 operand2,
            Func<T1, T2, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
        {
            return Create<IValueBuilder, T1, T2>(Construct, operand1, operand2, stringRepresentation);
        }

        public static IValueBuilder CreateValue(string stringRepresentation)
        {
            return Create<IValueBuilder>(Construct, stringRepresentation);
        }

        private static IValueBuilder Construct(string stringRepresentation, Resolver<IValueBuilder> resolver)
        {
            return new ValueBuilderStub(stringRepresentation, resolver);
        }

        #endregion

        #region IConditionBuilder

        // Identical to the Create<> methods at the bottom but with `new ConditionBuilderStub()` as constructor.

        public static IConditionBuilder CreateCondition<T>(
            [CanBeNull] T operand,
            Func<T, string> stringRepresentation)
            where T : class, IResolvable<T>
        {
            return Create<IConditionBuilder, T>(Construct, operand, stringRepresentation);
        }

        public static IConditionBuilder CreateCondition<T1, T2>(
            [CanBeNull] T1 operand1, [CanBeNull] T2 operand2,
            Func<T1, T2, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
        {
            return Create<IConditionBuilder, T1, T2>(Construct, operand1, operand2, stringRepresentation);
        }

        public static IConditionBuilder CreateCondition<T>(
            [ItemCanBeNull] IEnumerable<T> operands,
            Func<IEnumerable<T>, string> stringRepresentation)
            where T : class, IResolvable<T>
        {
            return Create<IConditionBuilder, T>(Construct, operands, stringRepresentation);
        }

        public static IConditionBuilder CreateCondition(string stringRepresentation)
        {
            return Create<IConditionBuilder>(Construct, stringRepresentation);
        }

        private static IConditionBuilder Construct(string stringRepresentation, Resolver<IConditionBuilder> resolver)
        {
            return new ConditionBuilderStub(stringRepresentation, resolver);
        }

        #endregion

        #region IStatBuilder

        // Identical to the Create<> methods at the bottom but with `new StatBuilderStub()` as constructor.

        public static IStatBuilder CreateStat<T>(
            [CanBeNull] T operand,
            Func<T, string> stringRepresentation)
            where T : class, IResolvable<T>
        {
            return Create<IStatBuilder, T>(Construct, operand, stringRepresentation);
        }

        public static IStatBuilder CreateStat<T1, T2>(
            [CanBeNull] T1 operand1, [CanBeNull] T2 operand2,
            Func<T1, T2, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
        {
            return Create<IStatBuilder, T1, T2>(Construct, operand1, operand2, stringRepresentation);
        }

        public static IStatBuilder CreateStat<T>(
            [ItemCanBeNull] IEnumerable<T> operands,
            Func<IEnumerable<T>, string> stringRepresentation)
            where T : class, IResolvable<T>
        {
            return Create<IStatBuilder, T>(Construct, operands, stringRepresentation);
        }

        public static IStatBuilder CreateStat(string stringRepresentation)
        {
            return Create<IStatBuilder>(Construct, stringRepresentation);
        }

        private static IStatBuilder Construct(string stringRepresentation, Resolver<IStatBuilder> resolver)
        {
            return new StatBuilderStub(stringRepresentation, resolver);
        }

        #endregion

        #region IFlagStatBuilder

        // Identical to the Create<> methods at the bottom but with `new FlagStatBuilderStub()` as constructor.

        public static IFlagStatBuilder CreateFlagStat<T>(
            [CanBeNull] T operand,
            Func<T, string> stringRepresentation)
            where T : class, IResolvable<T>
        {
            return (IFlagStatBuilder) Create<IStatBuilder, T>(ConstructFlag, operand, stringRepresentation);
        }

        public static IFlagStatBuilder CreateFlagStat<T1, T2>(
            [CanBeNull] T1 operand1, [CanBeNull] T2 operand2,
            Func<T1, T2, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
        {
            return (IFlagStatBuilder) Create<IStatBuilder, T1, T2>(
                ConstructFlag, operand1, operand2, stringRepresentation);
        }

        public static IFlagStatBuilder CreateFlagStat<T1, T2, T3>(
            [CanBeNull] T1 operand1, [CanBeNull] T2 operand2, [CanBeNull] T3 operand3,
            Func<T1, T2, T3, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
            where T3 : class, IResolvable<T3>
        {
            return (IFlagStatBuilder) Create<IStatBuilder, T1, T2, T3>(
                ConstructFlag, operand1, operand2, operand3, stringRepresentation);
        }

        public static IFlagStatBuilder CreateFlagStat<T1, T2>(
            [CanBeNull] T1 operand1, [ItemCanBeNull] IEnumerable<T2> operand2,
            Func<T1, IEnumerable<T2>, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
        {
            return (IFlagStatBuilder) Create<IStatBuilder, T1, T2>(
                ConstructFlag, operand1, operand2, stringRepresentation);
        }

        private static IFlagStatBuilder ConstructFlag(string stringRepresentation, Resolver<IStatBuilder> resolver)
        {
            return new FlagStatBuilderStub(stringRepresentation, resolver);
        }

        #endregion

        #region IPoolStatBuilder

        // Identical to the Create<> methods at the bottom but with `new PoolStatBuilderStub()` as constructor.

        public static IPoolStatBuilder CreatePoolStat<T1, T2>(
            [CanBeNull] T1 operand1, [CanBeNull] T2 operand2,
            Func<T1, T2, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
        {
            return (IPoolStatBuilder) Create<IStatBuilder, T1, T2>(
                ConstructPool, operand1, operand2, stringRepresentation);
        }

        private static IPoolStatBuilder ConstructPool(string stringRepresentation, Resolver<IStatBuilder> resolver)
        {
            return new PoolStatBuilderStub(stringRepresentation, resolver);
        }

        #endregion

        #region IDamageStatBuilder

        // Identical to the Create<> methods at the bottom but with `new DamageStatBuilderStub()` as constructor.

        public static IDamageStatBuilder CreateDamageStat<T>(
            [CanBeNull] T operand,
            Func<T, string> stringRepresentation)
            where T : class, IResolvable<T>
        {
            return (IDamageStatBuilder) Create<IStatBuilder, T>(ConstructDamage, operand, stringRepresentation);
        }

        public static IDamageStatBuilder CreateDamageStat<T1, T2>(
            [CanBeNull] T1 operand1, [CanBeNull] T2 operand2,
            Func<T1, T2, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
        {
            return (IDamageStatBuilder) Create<IStatBuilder, T1, T2>(
                ConstructDamage, operand1, operand2, stringRepresentation);
        }

        private static IDamageStatBuilder ConstructDamage(string stringRepresentation,
            Resolver<IStatBuilder> resolver)
        {
            return new DamageStatBuilderStub(stringRepresentation, resolver);
        }

        #endregion

        /// <summary>
        /// Creates a <typeparamref name="TOut"/> with the given constructor that has the string representation
        /// <c>stringRepresentation(operand)</c>. It resolves to a new instance created with the given constructor
        /// and the string represenation <c>stringRepresentation(operand?.Resolve())</c>, which resolves to itself.
        /// </summary>
        public static TOut Create<TOut, T>(
            Func<string, Resolver<TOut>, TOut> constructor,
            [CanBeNull] T operand,
            Func<T, string> stringRepresentation)
            where T : class, IResolvable<T>
        {
            TOut Resolve(ResolveContext context) =>
                Create(constructor, stringRepresentation(operand?.Resolve(context)));

            return Create(constructor, stringRepresentation(operand), Resolve);
        }

        /// <summary>
        /// Creates a <typeparamref name="T"/> with the given constructor that has the string representation
        /// <c>stringRepresentation(operand)</c>. It resolves to a new instance created with the given constructor
        /// and the string represenation <c>stringRepresentation(operand?.Resolve())</c>, which resolves to itself.
        /// </summary>
        public static T Create<T>(
            Func<string, Resolver<T>, T> constructor,
            [CanBeNull] T operand,
            Func<T, string> stringRepresentation)
            where T : class, IResolvable<T>
        {
            T Resolve(ResolveContext context) =>
                Create(constructor, stringRepresentation(operand?.Resolve(context)));

            return Create(constructor, stringRepresentation(operand), Resolve);
        }

        /// <summary>
        /// Creates a <typeparamref name="TOut"/> with the given constructor that has the string representation
        /// <c>stringRepresentation(operand1, operand2)</c>. It resolves to a new instance created with the given 
        /// constructor and the string represenation 
        /// <c>stringRepresentation(operand1?.Resolve(), operand2?.Resolve())</c>, which resolves to itself.
        /// </summary>
        public static TOut Create<TOut, T1, T2>(
            Func<string, Resolver<TOut>, TOut> constructor,
            [CanBeNull] T1 operand1,
            [CanBeNull] T2 operand2,
            Func<T1, T2, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
        {
            TOut Resolve(ResolveContext context) =>
                Create(constructor, stringRepresentation(
                    operand1?.Resolve(context),
                    operand2?.Resolve(context)));

            return Create(constructor, stringRepresentation(operand1, operand2), Resolve);
        }

        /// <summary>
        /// Creates a <typeparamref name="T1"/> with the given constructor that has the string representation
        /// <c>stringRepresentation(operand1, operand2)</c>. It resolves to a new instance created with the given 
        /// constructor and the string represenation 
        /// <c>stringRepresentation(operand1?.Resolve(), operand2?.Resolve())</c>, which resolves to itself.
        /// </summary>
        public static T1 Create<T1, T2>(
            Func<string, Resolver<T1>, T1> constructor,
            [CanBeNull] T1 operand1,
            [CanBeNull] T2 operand2,
            Func<T1, T2, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
        {
            T1 Resolve(ResolveContext context) =>
                Create(constructor, stringRepresentation(
                    operand1?.Resolve(context),
                    operand2?.Resolve(context)));

            return Create(constructor, stringRepresentation(operand1, operand2), Resolve);
        }

        /// <summary>
        /// Creates a <typeparamref name="TOut"/> with the given constructor that has the string representation
        /// <c>stringRepresentation(operand1, operand2, operand3)</c>. It resolves to a new instance created with the 
        /// given constructor and the string represenation 
        /// <c>stringRepresentation(operand1?.Resolve(), operand2?.Resolve(), operand3?.Resolve())</c>, which resolves
        /// to itself.
        /// </summary>
        private static TOut Create<TOut, T1, T2, T3>(
            Func<string, Resolver<TOut>, TOut> constructor,
            [CanBeNull] T1 operand1,
            [CanBeNull] T2 operand2,
            [CanBeNull] T3 operand3,
            Func<T1, T2, T3, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
            where T3 : class, IResolvable<T3>
        {
            TOut Resolve(ResolveContext context) =>
                Create(constructor, stringRepresentation(
                    operand1?.Resolve(context),
                    operand2?.Resolve(context),
                    operand3?.Resolve(context)));

            return Create(constructor, stringRepresentation(operand1, operand2, operand3), Resolve);
        }

        /// <summary>
        /// Creates a <typeparamref name="TOut"/> with the given constructor that has the string representation
        /// <c>stringRepresentation(operands)</c>. It resolves to a new instance created with the 
        /// given constructor and the string represenation
        /// <c>stringRepresentation(operands.Select(o => o?.Resolve()))</c>, which resolves to itself.
        /// </summary>
        /// <remarks>
        /// <paramref name="operands"/> is only enumerated once.
        /// </remarks>
        public static TOut Create<TOut, T>(
            Func<string, Resolver<TOut>, TOut> constructor,
            [ItemCanBeNull] IEnumerable<T> operands,
            Func<IEnumerable<T>, string> stringRepresentation)
            where T : class, IResolvable<T>
        {
            var os = operands.ToList();

            TOut Resolve(ResolveContext context) =>
                Create(constructor, stringRepresentation(
                    os.Select(o => o?.Resolve(context))));

            return Create(constructor, stringRepresentation(os), Resolve);
        }

        /// <summary>
        /// Creates a <typeparamref name="TOut"/> with the given constructor that has the string representation
        /// <c>stringRepresentation(operand1, operands)</c>. It resolves to a new instance created with the 
        /// given constructor and the string represenation
        /// <c>stringRepresentation(operand1?.Resolve(), operands.Select(o => o?.Resolve()))</c>, which resolves to 
        /// itself.
        /// </summary>
        /// <remarks>
        /// <paramref name="operands"/> is only enumerated once.
        /// </remarks>
        private static TOut Create<TOut, T1, T2>(
            Func<string, Resolver<TOut>, TOut> constructor,
            [CanBeNull] T1 operand1,
            [ItemCanBeNull] IEnumerable<T2> operands,
            Func<T1, IEnumerable<T2>, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
        {
            var os = operands.ToList();

            TOut Resolve(ResolveContext context) =>
                Create(constructor, stringRepresentation(
                    operand1?.Resolve(context),
                    os.Select(o => o?.Resolve(context))));

            return Create(constructor, stringRepresentation(operand1, os), Resolve);
        }

        /// <summary>
        /// Creates a <typeparamref name="T1"/> with the given constructor that has the string representation
        /// <c>stringRepresentation(operand1, operands)</c>. It resolves to a new instance created with the 
        /// given constructor and the string represenation
        /// <c>stringRepresentation(operand1?.Resolve(), operands.Select(o => o?.Resolve()))</c>, which resolves to 
        /// itself.
        /// </summary>
        /// <remarks>
        /// <paramref name="operands"/> is only enumerated once.
        /// </remarks>
        public static T1 Create<T1, T2>(
            Func<string, Resolver<T1>, T1> constructor,
            [CanBeNull] T1 operand1,
            [ItemCanBeNull] IEnumerable<T2> operands,
            Func<T1, IEnumerable<T2>, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
        {
            var os = operands.ToList();

            T1 Resolve(ResolveContext context) =>
                Create(constructor, stringRepresentation(
                    operand1?.Resolve(context),
                    os.Select(o => o?.Resolve(context))));

            return Create(constructor, stringRepresentation(operand1, os), Resolve);
        }

        /// <summary>
        /// Creates a <typeparamref name="T1"/> with the given constructor that has the string representation
        /// <c>stringRepresentation(operand1, operand2, operands)</c>. It resolves to a new instance created with the 
        /// given constructor and the string represenation
        /// <c>stringRepresentation(operand1?.Resolve(), operand2?.Resolve(), operands.Select(o => o?.Resolve()))</c>,
        /// which resolves to itself.
        /// </summary>
        /// <remarks>
        /// <paramref name="operands"/> is only enumerated once.
        /// </remarks>
        public static T1 Create<T1, T2, T3>(
            Func<string, Resolver<T1>, T1> constructor,
            [CanBeNull] T1 operand1,
            [CanBeNull] T2 operand2,
            [ItemCanBeNull] IEnumerable<T3> operands,
            Func<T1, T2, IEnumerable<T3>, string> stringRepresentation)
            where T1 : class, IResolvable<T1>
            where T2 : class, IResolvable<T2>
            where T3 : class, IResolvable<T3>
        {
            var os = operands.ToList();

            T1 Resolve(ResolveContext context) =>
                Create(constructor, stringRepresentation(
                    operand1?.Resolve(context),
                    operand2?.Resolve(context),
                    os.Select(o => o?.Resolve(context))));

            return Create(constructor, stringRepresentation(operand1, operand2, os), Resolve);
        }

        /// <summary>
        /// Creates a <typeparamref name="T"/> with the given constructor that has the given string representation
        /// and resolves to itself.
        /// </summary>
        private static T Create<T>(
            Func<string, Resolver<T>, T> constructor,
            string stringRepresentation)
        {
            return constructor(stringRepresentation, (current, _) => current);
        }

        /// <summary>
        /// Creates a <typeparamref name="T"/> with the given constructor that has the given string representation
        /// and resolves to <c>resolver(context)</c>.
        /// </summary>
        private static T Create<T>(
            Func<string, Resolver<T>, T> constructor,
            string stringRepresentation,
            Func<ResolveContext, T> resolver)
        {
            return constructor(stringRepresentation, (_, context) => resolver(context));
        }
    }
}