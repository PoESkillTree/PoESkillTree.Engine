﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PoESkillTree.Computation.Common;

namespace PoESkillTree.Computation.Core.Nodes
{
    [TestFixture]
    public class NodeValueAggregatorsTest
    {
        [TestCase(null)]
        [TestCase(42, 42.0)]
        [TestCase(0, 42.0, 43.0, 0.0, 4.0)]
        [TestCase(43, null, 43.0, null)]
        [TestCase(43, 43.0, 43.0)]
        [TestCase(1e-200, 1e-200)]
        [TestCase(1e-12, 1e-12, 1e-11)]
        public void CalculateOverrideReturnsCorrectResult(double? expected, params double?[] values)
        {
            AssertReturnsCorrectResult(NodeValueAggregators.CalculateTotalOverride, expected, values);
        }

        [Test]
        public void CalculateOverrideThrowsExceptionIfNoValueIsZero()
        {
            var values = new double?[] { 42, 43, null, 4, -3 }.Select(v => (NodeValue?) v).ToList();

            Assert.Throws<NotSupportedException>(() => NodeValueAggregators.CalculateTotalOverride(values));
        }

        [TestCase(null)]
        [TestCase(1.42, 42.0)]
        [TestCase(1.5, 50.0, 100.0, -50.0)]
        public void CalculateMoreReturnsCorrectResult(double? expected, params double?[] values)
        {
            AssertReturnsCorrectResult(NodeValueAggregators.CalculateMore, expected, values);
        }

        [TestCase(null)]
        [TestCase(0.42, 42.0)]
        [TestCase(1, 50.0, 100.0, -50.0)]
        public void CalculateIncreaseReturnsCorrectResult(double? expected, params double?[] values)
        {
            AssertReturnsCorrectResult(NodeValueAggregators.CalculateIncrease, expected, values);
        }

        [TestCase(null)]
        [TestCase(42, 42.0)]
        [TestCase(100, 50.0, 100.0, -50.0)]
        public void CalculateBaseAddReturnsCorrectResult(double? expected, params double?[] values)
        {
            AssertReturnsCorrectResult(NodeValueAggregators.CalculateBaseAdd, expected, values);
        }
        
        [TestCase(null)]
        [TestCase(42, 42.0)]
        [TestCase(0, 42.0, 0.0, null)]
        public void CalculateBaseSetReturnsCorrectResult(double? expected, params double?[] values)
        {
            AssertReturnsCorrectResult(NodeValueAggregators.CalculateBaseSet, expected, values);
        }

        [Test]
        public void CalculateBaseSetThrowsExceptionIfNoValueIsZero()
        {
            var values = new List<NodeValue?> { new NodeValue(0, 5), new NodeValue(0, 44)};

            Assert.Throws<NotSupportedException>(() => NodeValueAggregators.CalculateBaseSet(values));
        }

        private static void AssertReturnsCorrectResult(
            NodeValueAggregator aggregator, double? expected, IEnumerable<double?> values)
        {
            var actual = aggregator(values.Select(v => (NodeValue?) v).ToList());

            Assert.AreEqual((NodeValue?) expected, actual);
        }
    }
}