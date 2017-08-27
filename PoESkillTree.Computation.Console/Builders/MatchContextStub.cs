﻿using System;
using PoESkillTree.Computation.Parsing.Builders.Conditions;
using PoESkillTree.Computation.Parsing.Builders.Matching;
using PoESkillTree.Computation.Parsing.Builders.Values;

namespace PoESkillTree.Computation.Console.Builders
{
    public class MatchContextStub<T> : BuilderStub, IMatchContext<T>
    {
        private readonly Func<string, T> _tFactory;

        public MatchContextStub(string stringRepresentation, Func<string, T> tFactory) 
            : base(stringRepresentation)
        {
            _tFactory = tFactory;
        }

        public T this[int index] => _tFactory($"{this}[{index}]");

        public T First => _tFactory($"{this}.First");
        public T Last => _tFactory($"{this}.Last");
        public T Single => _tFactory($"{this}.Single");
    }


    public class MatchContextsStub : IMatchContexts
    {
        private readonly IConditionBuilders _conditionBuilders;

        public MatchContextsStub(IConditionBuilders conditionBuilders)
        {
            _conditionBuilders = conditionBuilders;
        }

        public IMatchContext<IGroupConverter> Groups =>
            new MatchContextStub<IGroupConverter>("Groups",
                s => new GroupConverterStub(s, _conditionBuilders));

        public IMatchContext<ValueBuilder> Values =>
            new MatchContextStub<ValueBuilder>("Values",
                s => new ValueBuilder(new ValueBuilderStub(s), _conditionBuilders));
    }
}