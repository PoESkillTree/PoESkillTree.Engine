using System;
using PoESkillTree.Computation.Providers.Conditions;

namespace PoESkillTree.Computation.Providers.Values
{
    public class ValueProvider : IValueProvider
    {
        public ValueProvider(IValueProvider value)
        {
        }

        // If the == and != overloads make implementing the class difficult, they can easily be
        // removed and usages replaced by <= or >=
        public static IConditionProvider operator ==(ValueProvider left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator ==(ValueProvider left, double right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator ==(double left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator !=(ValueProvider left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator !=(ValueProvider left, double right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator !=(double left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator >=(ValueProvider left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator >=(ValueProvider left, double right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator >=(double left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator <=(ValueProvider left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator <=(ValueProvider left, double right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator <=(double left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator >(ValueProvider left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator >(ValueProvider left, double right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator >(double left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator <(ValueProvider left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator <(ValueProvider left, double right)
        {
            throw new NotImplementedException();
        }

        public static IConditionProvider operator <(double left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static ValueProvider operator *(ValueProvider left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static ValueProvider operator *(ValueProvider left, double right)
        {
            throw new NotImplementedException();
        }

        public static ValueProvider operator *(double left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static ValueProvider operator /(ValueProvider left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static ValueProvider operator /(ValueProvider left, double right)
        {
            throw new NotImplementedException();
        }

        public static ValueProvider operator /(double left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static ValueProvider operator -(ValueProvider left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static ValueProvider operator -(ValueProvider left, double right)
        {
            throw new NotImplementedException();
        }

        public static ValueProvider operator -(double left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static ValueProvider operator +(ValueProvider left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public static ValueProvider operator +(ValueProvider left, double right)
        {
            throw new NotImplementedException();
        }

        public static ValueProvider operator +(double left, ValueProvider right)
        {
            throw new NotImplementedException();
        }

        public ValueProvider AsPercentage => this / 100;
        public ValueProvider Invert => 1 / this;

        // to how many digits depends on the number of significant digits the value has
        // they also need to be rounded to more digits before floored/ceiled to avoid e.g. 0.99999 being floored to 0
        public ValueProvider Rounded => throw new NotImplementedException();
        public ValueProvider Floored => throw new NotImplementedException();
        public ValueProvider Ceiled => throw new NotImplementedException();
    }
}