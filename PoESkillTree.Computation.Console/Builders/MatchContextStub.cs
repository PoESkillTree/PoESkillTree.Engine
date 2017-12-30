﻿using PoESkillTree.Computation.Parsing.Builders.Matching;
using PoESkillTree.Computation.Parsing.Builders.Values;

namespace PoESkillTree.Computation.Console.Builders
{
    public class MatchContextsStub : IMatchContexts
    {
        public IMatchContext<IReferenceConverter> References => new ReferenceMatchContext();

        public IMatchContext<ValueBuilder> Values => new ValueMatchContext();

        /*
         * These clases are the leaf nodes of resolve method chains. Resolving actually does something here, which
         * is why the properties look slightly different and use the passed context to return resolved objects.
         */

        private class ValueMatchContext : BuilderStub, IMatchContext<ValueBuilder>
        {
            public ValueMatchContext() : base("Values")
            {
            }

            public ValueBuilder this[int index] =>
                new ValueBuilder(new ValueBuilderStub($"{this}[{index}]", (_, c) => c.ValueContext[index]));

            public ValueBuilder First =>
                new ValueBuilder(new ValueBuilderStub($"{this}.First", (_, c) => c.ValueContext.First));

            public ValueBuilder Last =>
                new ValueBuilder(new ValueBuilderStub($"{this}.Last", (_, c) => c.ValueContext.Last));

            public ValueBuilder Single =>
                new ValueBuilder(new ValueBuilderStub($"{this}.Single", (_, c) => c.ValueContext.Single));
        }


        private class ReferenceMatchContext : BuilderStub, IMatchContext<IReferenceConverter>
        {
            public ReferenceMatchContext() : base("References")
            {
            }

            public IReferenceConverter this[int index] =>
                new ReferenceConverterStub($"{this}[{index}]", (_, c) => c.ReferenceContext[index]);

            public IReferenceConverter First =>
                new ReferenceConverterStub($"{this}.First", (_, c) => c.ReferenceContext.First);

            public IReferenceConverter Last =>
                new ReferenceConverterStub($"{this}.Last", (_, c) => c.ReferenceContext.Last);

            public IReferenceConverter Single =>
                new ReferenceConverterStub($"{this}.Single", (_, c) => c.ReferenceContext.Single);
        }
    }
}