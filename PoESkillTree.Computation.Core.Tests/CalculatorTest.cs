﻿using Moq;
using MoreLinq;
using NUnit.Framework;
using PoESkillTree.Computation.Common;
using PoESkillTree.Computation.Core.Events;
using PoESkillTree.Computation.Core.Graphs;

namespace PoESkillTree.Computation.Core
{
    [TestFixture]
    public class CalculatorTest
    {
        [Test]
        public void SutIsCalculator()
        {
            var sut = CreateSut();

            Assert.IsInstanceOf<ICalculator>(sut);
        }

        [Test]
        public void NodeRepositoryReturnsInjectedInstance()
        {
            var expected = Mock.Of<INodeRepository>();
            var sut = CreateSut(nodeRepository: expected);

            var actual = sut.NodeRepository;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UpdateCallsInjectedInstancesInCorrectSequence()
        {
            var modifier = Helper.MockModifier();
            var modifierCollectionMock = new Mock<IModifierCollection>(MockBehavior.Strict);
            var graphPrunerMock = new Mock<ICalculationGraphPruner>(MockBehavior.Strict);
            var seq = new MockSequence();
            modifierCollectionMock.InSequence(seq).Setup(c => c.AddModifier(modifier));
            graphPrunerMock.InSequence(seq).Setup(p => p.RemoveUnusedNodes());
            var sut = CreateSut(modifierCollectionMock.Object, graphPrunerMock.Object);

            sut.NewBatchUpdate().AddModifier(modifier).DoUpdate();

            graphPrunerMock.Verify(p => p.RemoveUnusedNodes());
        }

        [Test]
        public void UpdateCallsInjectedModifierCollectionCorrectly()
        {
            var addedModifiers = Helper.MockManyModifiers();
            var removedModifiers = Helper.MockManyModifiers();
            var modifierCollectionMock = new Mock<IModifierCollection>();
            var sut = CreateSut(modifierCollectionMock.Object, Mock.Of<ICalculationGraphPruner>());

            sut.Update(new CalculatorUpdate(addedModifiers, removedModifiers));

            addedModifiers.ForEach(m => modifierCollectionMock.Verify(c => c.AddModifier(m)));
            removedModifiers.ForEach(m => modifierCollectionMock.Verify(c => c.RemoveModifier(m)));
        }

        [Test]
        public void ExplicitlyRegisteredStatsReturnsInjectedInstance()
        {
            var expected = Mock.Of<INodeCollection<IStat>>();
            var sut = CreateSut(explicitlyRegisteredStats: expected);

            var actual = sut.ExplicitlyRegisteredStats;

            Assert.AreSame(expected, actual);
        }

        private static Calculator CreateSut(
            IModifierCollection modifierCollection = null, ICalculationGraphPruner graphPruner = null,
            INodeRepository nodeRepository = null, INodeCollection<IStat> explicitlyRegisteredStats = null)
            => new Calculator(new EventBuffer(), modifierCollection, graphPruner, nodeRepository,
                explicitlyRegisteredStats);
    }
}