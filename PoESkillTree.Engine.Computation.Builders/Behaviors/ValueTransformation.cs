using System;
using PoESkillTree.Engine.Computation.Common;

namespace PoESkillTree.Engine.Computation.Builders.Behaviors
{
    public class ValueTransformation : IValueTransformation
    {
        private readonly Func<IValue, IValue> _transformation;

        public ValueTransformation(Func<IValue, IValue> transformation)
        {
            _transformation = transformation;
        }

        public IValue Transform(IValue value) => _transformation(value);
    }
}