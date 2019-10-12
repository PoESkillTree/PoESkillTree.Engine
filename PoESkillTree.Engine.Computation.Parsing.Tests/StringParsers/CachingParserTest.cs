using Moq;
using NUnit.Framework;

namespace PoESkillTree.Engine.Computation.Parsing.StringParsers
{
    [TestFixture]
    public class CachingParserTest
    {
        private const string TrueStat = "true";
        private const string TrueRemaining = "trueRemaining";
        private const string TrueParsed = "trueParsed";
        private const string FalseStat = "false";
        private const string FalseRemaining = "falseRemaining";
        private const string FalseParsed = "falseParsed";

#pragma warning disable 8618 // Initialized in SetUp
        private Mock<IStringParser<string>> _innerMock;
        private IStringParser<string> _inner;
#pragma warning restore

        [SetUp]
        public void SetUp()
        {
            _innerMock = StringParserTestUtils.MockParser(
                (TrueStat, new StringParseResult<string>(true, TrueRemaining, TrueParsed)),
                (FalseStat, new StringParseResult<string>(false, FalseRemaining, FalseParsed)));
            _inner = _innerMock.Object;
        }

        [Test]
        public void IsIParserString()
        {
            var sut = new CachingStringParser<string>(_inner);

            Assert.IsInstanceOf<IStringParser<string>>(sut);
        }

        [Test]
        public void IsIParserInt()
        {
            var sut = new CachingStringParser<string>(Mock.Of<IStringParser<string>>());

            Assert.IsInstanceOf<IStringParser<string>>(sut);
        }

        [TestCase(TrueStat, ExpectedResult = true)]
        [TestCase(FalseStat, ExpectedResult = false)]
        public bool TryParsePassesSuccessfullyParsed(string stat)
        {
            var sut = new CachingStringParser<string>(_inner);

            var (actual, _, _) = sut.Parse(stat);

            return actual;
        }

        [TestCase(TrueStat, ExpectedResult = TrueRemaining)]
        public string TryParsePassesRemaining(string stat)
        {
            var sut = new CachingStringParser<string>(_inner);

            var (_, actual, _) = sut.Parse(stat);

            return actual;
        }

        [TestCase(TrueStat, ExpectedResult = TrueParsed)]
        public string? TryParsePassesResult(string stat)
        {
            var sut = new CachingStringParser<string>(_inner);

            var (_, _, actual) = sut.Parse(stat);

            return actual;
        }

        [Test]
        public void TryParseCachesSingleStat()
        {
            var sut = new CachingStringParser<string>(_inner);

            sut.Parse(TrueStat);
            sut.Parse(TrueStat);

            _innerMock.VerifyParse(TrueStat, Times.Once);
        }

        [Test]
        public void TryParsesCachesMultipleStats()
        {
            var sut = new CachingStringParser<string>(_inner);

            sut.Parse(TrueStat);
            sut.Parse(FalseStat);
            sut.Parse(FalseStat);
            sut.Parse("whatever");
            sut.Parse(TrueStat);
            sut.Parse(TrueStat);
            sut.Parse("whatever");
            
            _innerMock.VerifyParse(TrueStat, Times.Once);
            _innerMock.VerifyParse(FalseStat, Times.Once);
            _innerMock.VerifyParse("whatever", Times.Once);
        }
    }
}