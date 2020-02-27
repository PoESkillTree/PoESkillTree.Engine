using System;
using System.Collections.Generic;
using PoESkillTree.Engine.Computation.Common;
using PoESkillTree.Engine.Computation.Common.Builders;
using PoESkillTree.Engine.GameModel;
using PoESkillTree.Engine.GameModel.Items;
using PoESkillTree.Engine.GameModel.Modifiers;
using PoESkillTree.Engine.GameModel.StatTranslation;
using PoESkillTree.Engine.Utils.Extensions;

namespace PoESkillTree.Engine.Computation.Parsing.ItemParsers
{
    /// <summary>
    /// Partial parser of <see cref="ItemParser"/> that parses <see cref="BaseItemDefinition.BuffStats"/>
    /// and <see cref="Item.Modifiers"/>
    /// </summary>
    public class ItemModifierParser : IParser<PartialItemParserParameter>
    {
        private readonly IBuilderFactories _builderFactories;
        private readonly ICoreParser _coreParser;
        private readonly UntranslatedStatParser _untranslatedStatParser;

        public ItemModifierParser(
            IBuilderFactories builderFactories, ICoreParser coreParser, IStatTranslator statTranslator)
        {
            (_builderFactories, _coreParser) = (builderFactories, coreParser);
            _untranslatedStatParser = new UntranslatedStatParser(statTranslator, _coreParser);
        }

        public ParseResult Parse(PartialItemParserParameter parameter)
        {
            var (item, _, entity, baseItemDefinition, localSource, globalSource) = parameter;
            var itemTags = baseItemDefinition.Tags;

            var results = new List<ParseResult>(1 + item.Modifiers.Count)
                { ParseBuffStats(entity, itemTags, localSource, baseItemDefinition.BuffStats) };
            foreach (var modifier in item.Modifiers)
            {
                if (ModifierLocalityTester.AffectsProperties(modifier, itemTags))
                    results.Add(ParsePropertyModifier(localSource, entity, modifier));
                else if (ModifierLocalityTester.IsLocal(modifier, itemTags))
                    results.Add(ParseLocalModifier(itemTags, localSource, entity, modifier));
                else
                    results.Add(ParseGlobalModifier(itemTags, globalSource, entity, modifier));
            }
            return ParseResult.Aggregate(results);
        }

        private ParseResult ParseBuffStats(
            Entity entity, Tags itemTags, ModifierSource.Local source, IReadOnlyList<UntranslatedStat> buffStats)
        {
            if (buffStats.IsEmpty())
                return ParseResult.Empty;
            if (!itemTags.HasFlag(Tags.Flask))
                throw new NotSupportedException("Buff stats are only supported for flasks");

            var result = _untranslatedStatParser.Parse(source, entity, buffStats);
            return MultiplyValuesByFlaskEffect(result, entity);
        }

        private ParseResult ParsePropertyModifier(ModifierSource.Local source, Entity entity, string modifier)
            => _coreParser.Parse(modifier + " (AsItemProperty)", source, entity);

        private ParseResult ParseLocalModifier(Tags itemTags, ModifierSource.Local source, Entity entity, string modifier)
        {
            if (itemTags.HasFlag(Tags.Weapon))
                modifier = "Attacks with this Weapon have " + modifier;
            return _coreParser.Parse(modifier, source, entity);
        }

        private ParseResult ParseGlobalModifier(Tags itemTags, ModifierSource.Global source, Entity entity, string modifier)
        {
            var result = _coreParser.Parse(modifier, source, entity);
            if (itemTags.HasFlag(Tags.Flask))
                result = MultiplyValuesByFlaskEffect(result, entity);
            return result;
        }

        private ParseResult MultiplyValuesByFlaskEffect(ParseResult result, Entity entity)
        {
            var multiplierBuilder = _builderFactories.StatBuilders.Flask.Effect.Value;
            return result.ApplyConditionalMultiplier(multiplierBuilder.Build, m => m.Form != Form.TotalOverride, entity);
        }
    }
}