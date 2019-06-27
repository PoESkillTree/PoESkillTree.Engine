using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Parsing.ItemParsers;
using PoESkillTree.Engine.GameModel;

namespace PoESkillTree.Engine.Computation.Parsing.JewelParsers
{
    public class JewelInItemParser : IParser<ItemParserParameter>
    {
        private readonly ICoreParser _coreParser;

        public JewelInItemParser(ICoreParser coreParser)
            => _coreParser = coreParser;

        public ParseResult Parse(ItemParserParameter parameter)
        {
            var (item, slot) = parameter;
            if (!item.IsEnabled)
                return ParseResult.Empty;

            var localSource = new ModifierSource.Local.Item(slot, item.Name);
            var globalSource = new ModifierSource.Global(localSource);

            var results = new List<ParseResult>(item.Modifiers.Count);
            foreach (var modifier in item.Modifiers)
            {
                results.Add(_coreParser.Parse(modifier, globalSource, Entity.Character));
            }
            return ParseResult.Aggregate(results);
        }
    }
}