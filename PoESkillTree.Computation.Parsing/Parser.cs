﻿using System;
using System.Collections.Generic;
using System.Linq;
using PoESkillTree.Common.Utils.Extensions;
using PoESkillTree.Computation.Parsing.Builders;
using PoESkillTree.Computation.Parsing.Data;
using PoESkillTree.Computation.Parsing.ModifierBuilding;
using PoESkillTree.Computation.Parsing.Referencing;

namespace PoESkillTree.Computation.Parsing
{
    /// <inheritdoc />
    /// <summary>
    /// Implementation of <see cref="IParser" /> using the parsing pipeline layed out by this project.
    /// <para> Dependencies not instantiated here are the actual data (lists of <see cref="IReferencedMatchers" />,
    /// <see cref="IStatMatchers" /> and <see cref="StatReplacerData" />), contained in the <c>Computation.Data</c>
    /// project, and an implementation of the interfaces in <see cref="Builders" />. These must be passed to the
    /// constructor.
    /// </para>
    /// <para> This should be the only <see cref="IParser{TResult}" /> implementation that is relevant outside of
    /// this project (excluding their own tests, obviously).
    /// </para>
    /// </summary>
    /// <remarks>
    /// <see cref="CreateParser" /> is a good overview to learn how the parts in this project interact.
    /// </remarks>
    public class Parser<TStep> : IParser
    {
        private readonly IParsingData<TStep> _parsingData;
        private readonly IBuilderFactories _builderFactories;

        private readonly Lazy<IParser<IReadOnlyList<Modifier>>> _parser;

        public Parser(IParsingData<TStep> parsingData, IBuilderFactories builderFactories)
        {
            _parsingData = parsingData;
            _builderFactories = builderFactories;
            _parser = new Lazy<IParser<IReadOnlyList<Modifier>>>(CreateParser);
        }

        public bool TryParse(string stat, out string remaining, out IReadOnlyList<Modifier> result)
        {
            return _parser.Value.TryParse(stat, out remaining, out result);
        }

        private IParser<IReadOnlyList<Modifier>> CreateParser()
        {
            var referenceManager = new ReferenceManager(_parsingData.ReferencedMatchers, _parsingData.StatMatchers);
            var regexGroupService = new RegexGroupService(_builderFactories.ValueBuilders);

            // The parsing pipeline using one IStatMatchers instance to parse a part of the stat.
            IParser<IModifierResult> CreateInnerParser(IStatMatchers statMatchers) =>
                new CachingParser<IModifierResult>(
                    new StatNormalizingParser<IModifierResult>(
                        new ResolvingParser(
                            new MatcherDataParser(
                                new StatMatcherRegexExpander(statMatchers, referenceManager, regexGroupService)),
                            referenceManager,
                            new ModifierResultResolver(new ModifierBuilder()),
                            regexGroupService
                        )
                    )
                );

            var innerParserCache = new Dictionary<IStatMatchers, IParser<IModifierResult>>();
            // The steps define the order in which the inner parsers, and by extent the IStatMatchers, are executed.
            IParser<IModifierResult> StepToParser(TStep step) =>
                innerParserCache.GetOrAdd(_parsingData.SelectStatMatcher(step), CreateInnerParser);

            // The full parsing pipeline.
            return
                new CachingParser<IReadOnlyList<Modifier>>(
                    new ValidatingParser<IReadOnlyList<Modifier>>(
                        new StatNormalizingParser<IReadOnlyList<Modifier>>(
                            new ResultMappingParser<IReadOnlyList<IReadOnlyList<Modifier>>, IReadOnlyList<Modifier>>(
                                new StatReplacingParser<IReadOnlyList<Modifier>>(
                                    new ResultMappingParser<IReadOnlyList<IModifierResult>, IReadOnlyList<Modifier>>(
                                        new CompositeParser<IModifierResult, TStep>(_parsingData.Stepper, StepToParser),
                                        l => l.Aggregate().Build(_builderFactories.ConditionBuilders.True)),
                                    _parsingData.StatReplacers
                                ),
                                ls => ls.Flatten().ToList()
                            )
                        )
                    )
                );
        }
    }
}