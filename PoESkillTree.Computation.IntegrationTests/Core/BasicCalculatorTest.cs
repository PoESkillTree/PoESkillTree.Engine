﻿using System;
using System.ComponentModel;
using NUnit.Framework;
using PoESkillTree.Common.Model.Items.Enums;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Core;

namespace PoESkillTree.Computation.IntegrationTests.Core
{
    [TestFixture]
    public class BasicCalculatorTest
    {
        [Test]
        public void SimpleCalculation()
        {
            var sut = Calculator.CreateCalculator();

            var expected = new NodeValue(5);
            var stat = new Stat();

            sut.NewBatchUpdate()
                .AddModifier(stat, Form.BaseAdd, new Constant(expected))
                .DoUpdate();

            var actual = sut.NodeRepository.GetNode(stat).Value;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MultipleUpdates()
        {
            var sut = Calculator.CreateCalculator();
            var stat = new Stat();
            var source = new ModifierSource.Global();
            var removedModifier = new Modifier(new[] { stat }, Form.BaseAdd, new Constant(100), source);

            sut.NewBatchUpdate()
                .AddModifier(stat, Form.BaseAdd, new Constant(10), source)
                .DoUpdate();
            sut.NewBatchUpdate()
                .AddModifier(stat, Form.BaseAdd, new Constant(1), source)
                .AddModifier(removedModifier)
                .DoUpdate();
            sut.NewBatchUpdate()
                .RemoveModifier(removedModifier)
                .AddModifier(stat, Form.BaseAdd, new Constant(1000), source)
                .DoUpdate();

            var actual = sut.NodeRepository.GetNode(stat).Value;

            Assert.AreEqual(new NodeValue(1011), actual);
        }

        [Test]
        public void EvasionCalculation()
        {
            var sut = Calculator.CreateCalculator();

            var evasionStat = new Stat();
            var lvlStat = new Stat();
            var dexterityStat = new Stat();
            var globalSource = new ModifierSource.Global();
            var bodySource = new ModifierSource.Local.Item(ItemSlot.BodyArmour);
            var shieldSource = new ModifierSource.Local.Item(ItemSlot.OffHand);

            sut.NewBatchUpdate()
                .AddModifier(evasionStat, Form.BaseSet, new Constant(53), globalSource)
                .AddModifier(evasionStat, Form.BaseAdd, new PerStatValue(lvlStat, 3), globalSource)
                .AddModifier(evasionStat, Form.BaseSet, new Constant(1000), bodySource)
                .AddModifier(evasionStat, Form.BaseSet, new Constant(500), shieldSource)
                .AddModifier(evasionStat, Form.Increase, new Constant(100), globalSource)
                .AddModifier(evasionStat, Form.Increase, new PerStatValue(dexterityStat, 1, 5), globalSource)
                .AddModifier(evasionStat, Form.Increase, new Constant(20), shieldSource)
                .AddModifier(evasionStat, Form.More, new Constant(100), bodySource)
                .AddModifier(lvlStat, Form.BaseSet, new Constant(90), globalSource)
                .AddModifier(dexterityStat, Form.BaseSet, new Constant(32), globalSource)
                .AddModifier(dexterityStat, Form.BaseAdd, new Constant(50), globalSource)
                .DoUpdate();
            var lvlTotal = 90 * 3;
            var dexterityTotal = 50 + 32;
            var globalBase = 53 + lvlTotal;
            var globalIncrease = 1 + Math.Ceiling(dexterityTotal / 5.0) / 100;
            var globalTotal = globalBase * (1 + globalIncrease);
            var bodyBase = 1000;
            var bodyTotal = bodyBase * (1 + globalIncrease) * 2;
            var shieldBase = 500;
            var shieldTotal = shieldBase * (1 + globalIncrease + 0.2);
            var evasionTotal = globalTotal + bodyTotal + shieldTotal;

            var actual = sut.NodeRepository.GetNode(evasionStat).Value;

            Assert.AreEqual(new NodeValue(evasionTotal), actual);
        }

        [TestCase(5)]
        [TestCase(15)]
        [TestCase(25)]
        public void Clip(double value)
        {
            var sut = Calculator.CreateCalculator();
            var min = new Stat();
            var max = new Stat();
            var stat = new Stat { Minimum = min, Maximum = max };

            sut.NewBatchUpdate()
                .AddModifier(stat, Form.BaseAdd, new Constant(value))
                .AddModifier(min, Form.BaseAdd, new Constant(10))
                .AddModifier(max, Form.BaseAdd, new Constant(20))
                .DoUpdate();
            var expected = new NodeValue(Math.Max(Math.Min(value, 20), 10));

            var actual = sut.NodeRepository.GetNode(stat).Value;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Pruning()
        {
            var sut = Calculator.CreateCalculator();
            var stat = new Stat();
            var mod = new Modifier(new []{stat}, Form.BaseAdd, new Constant(1), new ModifierSource.Global());

            sut.NewBatchUpdate().AddModifier(mod).DoUpdate();

            var removedNode = sut.NodeRepository.GetNode(stat);
            var keptNode = sut.NodeRepository.GetNode(stat, NodeType.Subtotal);
            keptNode.ValueChanged += (sender, args) => { };

            sut.NewBatchUpdate().RemoveModifier(mod).DoUpdate();

            Assert.AreEqual(keptNode, sut.NodeRepository.GetNode(stat, NodeType.Subtotal));
            Assert.AreNotEqual(removedNode, sut.NodeRepository.GetNode(stat));
        }

        [Test]
        public void Events()
        {
            var sut = Calculator.CreateCalculator();
            var stat = new Stat();
            var mod = new Modifier(new []{stat}, Form.BaseAdd, new Constant(1), new ModifierSource.Global());
            var node = sut.NodeRepository.GetNode(stat);
            var invovcations = 0;
            node.ValueChanged += (sender, args) => invovcations++;

            Assert.IsNull(node.Value);
            sut.NewBatchUpdate().AddModifier(mod).DoUpdate();
            Assert.AreEqual(new NodeValue(1), node.Value);
            sut.NewBatchUpdate().AddModifier(mod).DoUpdate();
            Assert.AreEqual(new NodeValue(2), node.Value);
            sut.NewBatchUpdate().AddModifier(mod).DoUpdate();
            Assert.AreEqual(new NodeValue(3), node.Value);

            Assert.AreEqual(3, invovcations);
        }

        [Test]
        public void ExplicitlyRegistered()
        {
            var sut = Calculator.CreateCalculator();
            var stat = new Stat { IsRegisteredExplicitly = true };
            IStat actual = null;
            sut.ExplicitlyRegisteredStats.CollectionChanged += (sender, args) =>
            {
                Assert.IsNull(actual);
                Assert.AreEqual(CollectionChangeAction.Add, args.Action);
                actual = (((ICalculationNode, IStat)) args.Element).Item2;
            };

            sut.NewBatchUpdate().AddModifier(stat, Form.BaseAdd, new Constant(0)).DoUpdate();

            Assert.AreEqual(stat, actual);
        }

        [Test]
        public void Behavior()
        {
            var sut = Calculator.CreateCalculator();
            var transformedStat = new Stat();
            var behavior = new Behavior(new[] { transformedStat }, new[] { NodeType.Subtotal },
                BehaviorPathInteraction.AllPaths, new ValueTransformation(_ => new Constant(5)));
            var stat = new Stat { Behaviors = new[] { behavior } };

            sut.NewBatchUpdate().AddModifier(stat, Form.BaseAdd, new Constant(1)).DoUpdate();

            var actual = sut.NodeRepository.GetNode(transformedStat).Value;

            Assert.AreEqual(new NodeValue(5), actual);
        }
    }
}