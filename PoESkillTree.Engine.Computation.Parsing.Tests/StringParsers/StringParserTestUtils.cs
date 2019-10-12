using System;
using Moq;

namespace PoESkillTree.Engine.Computation.Parsing.StringParsers
{
    public static class StringParserTestUtils
    {
        public static Mock<IStringParser<TResult>> MockParser<TResult>(
            string modifier, bool successfullyParsed, string remainingSubstring, TResult result)
            where TResult : class
            => MockParser(modifier, new StringParseResult<TResult>(successfullyParsed, remainingSubstring, result));

        private static Mock<IStringParser<TResult>> MockParser<TResult>(
            string modifier, StringParseResult<TResult> result)
            where TResult : class
            => MockParser((modifier, result));

        public static Mock<IStringParser<TResult>> MockParser<TResult>(
            params (string modifier, StringParseResult<TResult> result)[] setup)
            where TResult : class
        {
            var mock = new Mock<IStringParser<TResult>>();
            foreach (var (modifier, result) in setup)
            {
                mock.Setup(p => p.Parse(modifier))
                    .Returns(result);
            }
            return mock;
        }

        public static void VerifyParse<TResult>(this Mock<IStringParser<TResult>> @this,
            string modifier, Func<Times> times)
            where TResult : class
            => @this.Verify(p => p.Parse(modifier), times);

        public static void VerifyParse<TResult>(this Mock<IStringParser<TResult>> @this, string modifier)
            where TResult : class
            => @this.Verify(p => p.Parse(modifier));
    }
}