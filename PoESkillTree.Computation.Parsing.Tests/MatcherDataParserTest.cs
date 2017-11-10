﻿using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PoESkillTree.Computation.Parsing.Data;
using PoESkillTree.Computation.Parsing.ModifierBuilding;

namespace PoESkillTree.Computation.Parsing.Tests
{
    [TestFixture]
    public class MatcherDataParserTest
    {
        [TestCase("ac", ExpectedResult = true)]
        [TestCase("a c", ExpectedResult = true)]
        [TestCase("a b", ExpectedResult = true)]
        [TestCase("test a b stuff", ExpectedResult = true)]
        [TestCase("abc", ExpectedResult = true)]
        [TestCase("xabx", ExpectedResult = true)]
        [TestCase("a", ExpectedResult = false)]
        public bool TryParseReturnsCorrectResult(string stat)
        {
            var sut = DefaultSut;

            return sut.TryParse(stat, out var _, out var _);
        }

        [TestCase("ac", ExpectedResult = "")]
        [TestCase("a c", ExpectedResult = "")]
        [TestCase("a b", ExpectedResult = "")]
        [TestCase("test a b stuff", ExpectedResult = "test  stuff")]
        [TestCase("abc", ExpectedResult = "ac")]
        [TestCase("xabx", ExpectedResult = "xax")]
        public string TryParseOutputsCorrectRemaining(string stat)
        {
            var sut = DefaultSut;

            sut.TryParse(stat, out var remaining, out var _);

            return remaining;
        }

        [TestCase("ac", 0)]
        [TestCase("a c", 0)]
        [TestCase("a b", 1)]
        [TestCase("test a b stuff", 1)]
        [TestCase("abc", 2)]
        [TestCase("xabx", 2)]
        public void TryParseOutputsCorrectModifierBuilder(string stat, int matcherDataIndex)
        {
            var expected = DefaultMatcherData[matcherDataIndex].ModifierBuilder;
            var sut = DefaultSut;

            sut.TryParse(stat, out var _, out var result);

            Assert.AreEqual(expected, result.ModifierBuilder);
        }

        [TestCase("ac", new[] {"ac", "c"})]
        [TestCase("a c", new[] {"a c", " c"})]
        public void TryParseOutputsCorrectGroups(string stat, string[] groups)
        {
            var expected = new Dictionary<string, string>
            {
                ["0"] = groups[0],
                ["g1"] = groups[0],
                ["g2"] = groups[1]
            };
            var sut = DefaultSut;

            sut.TryParse(stat, out var _, out var result);

            CollectionAssert.AreEqual(expected, result.Groups);
        }

        [TestCase("a b", "a b")]
        [TestCase("test a b stuff", "a b")]
        [TestCase("abc", "ab")]
        [TestCase("xabx", "ab")]
        public void TryParseOutputsOnlFullGroupCorrectly(string stat, string fullGroup)
        {
            var sut = DefaultSut;

            sut.TryParse(stat, out var _, out var result);
            var groups = result.Groups;

            Assert.That(groups, Has.Exactly(1).Items.EqualTo(new KeyValuePair<string, string>("0", fullGroup)));
        }

        private static readonly MatcherData[] DefaultMatcherData =
        {
            CreateMatcherData("(?<g1>(a)(?<g2>[ c]+))"),
            CreateMatcherData("a b"),
            CreateMatcherData("ab", "a")
        };

        private static readonly MatcherDataParser DefaultSut = new MatcherDataParser(DefaultMatcherData);

        private static MatcherData CreateMatcherData(string regex, string matchSubstitution = "")
        {
            var modifierBuilder = Mock.Of<IModifierBuilder>();
            return new MatcherData(regex, modifierBuilder, matchSubstitution);
        }
    }
}