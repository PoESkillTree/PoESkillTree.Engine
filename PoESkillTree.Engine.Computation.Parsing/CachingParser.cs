namespace PoESkillTree.Engine.Computation.Parsing
{
    public class CachingParser<TParameter> : GenericCachingParser<TParameter, ParseResult>, IParser<TParameter>
    {
        public CachingParser(IParser<TParameter> decoratedParser)
            : base(decoratedParser.Parse)
        {
        }
    }
}