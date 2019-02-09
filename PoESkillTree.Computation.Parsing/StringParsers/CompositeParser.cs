﻿using System;
using System.Collections.Generic;
using PoESkillTree.Computation.Common.Data;

namespace PoESkillTree.Computation.Parsing.StringParsers
{
    /// <inheritdoc />
    /// <summary>
    /// Parser that uses <see cref="IStepper{TStep}"/>s to add results of multiple parsers (one for each step) into
    /// a list until a completed step is reached.
    /// <para> The <see cref="IStepper{TStep}"/> is used like a state machine. At each step, the parser for the
    /// current step is executed and the returned value is used as a (boolean) state transition.
    /// </para>
    /// <para> The results of successful parses are added to a list that is returned at the end. The output remaining of 
    /// one step serves as the input stat of the next. The last step's remaining is used as the method's output.
    /// <see cref="Parse"/> returns <see cref="ParseResult{T}.SuccessfullyParsed"/> if the Stepper ends in a success
    /// step.
    /// </para>
    /// </summary>
    public class CompositeParser<TInnerResult, TStep> : IStringParser<IReadOnlyList<TInnerResult>>
    {
        private readonly IStepper<TStep> _stepper;
        private readonly Func<TStep, IStringParser<TInnerResult>> _stepToParserFunc;

        public CompositeParser(IStepper<TStep> stepper, Func<TStep, IStringParser<TInnerResult>> stepToParserFunc)
        {
            _stepper = stepper;
            _stepToParserFunc = stepToParserFunc;
        }

        public StringParseResult<IReadOnlyList<TInnerResult>> Parse(CoreParserParameter parameter)
        {
            var step = _stepper.InitialStep;
            var remaining = parameter.ModifierLine;
            var results = new List<TInnerResult>();
            while (!_stepper.IsTerminal(step))
            {
                var (innerSuccess, innerRemaining, innerResult) = _stepToParserFunc(step).Parse(remaining, parameter);
                remaining = innerRemaining;
                if (innerSuccess)
                {
                    results.Add(innerResult);
                    step = _stepper.NextOnSuccess(step);
                }
                else
                {
                    step = _stepper.NextOnFailure(step);
                }
            }

            return (_stepper.IsSuccess(step), remaining, results);
        }
    }
}