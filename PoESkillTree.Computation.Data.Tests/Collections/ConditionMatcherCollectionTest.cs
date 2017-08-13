﻿using System.Linq;
using Moq;
using NUnit.Framework;
using PoESkillTree.Computation.Data.Collections;
using PoESkillTree.Computation.Providers.Conditions;

namespace PoESkillTree.Computation.Data.Tests.Collections
{
    [TestFixture]
    public class ConditionMatcherCollectionTest
    {
        private const string Regex = "regex";

        private ConditionMatcherCollection _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new ConditionMatcherCollection(new MatchBuilderStub());
        }

        [Test]
        public void IsEmpty()
        {
            Assert.IsEmpty(_sut);
        }

        [Test]
        public void Add()
        {
            var condition = Mock.Of<IConditionProvider>();

            _sut.Add(Regex, condition);

            var builder = _sut.AssertSingle(Regex);
            Assert.That(builder.Conditions, Has.Exactly(1).SameAs(condition));
        }

        [Test]
        public void AddManyAddsToCount()
        {
            var condition = Mock.Of<IConditionProvider>();

            _sut.Add(Regex, condition);
            _sut.Add(Regex, condition);
            _sut.Add(Regex, condition);

            Assert.AreEqual(3, _sut.Count());
        }
    }
}