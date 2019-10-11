using System;

namespace Raven
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class DefaultValueAttribute : Attribute
    {
        public object Value { get; }

        public DefaultValueAttribute(object value)
        {
            Value = value;
        }
    }
}