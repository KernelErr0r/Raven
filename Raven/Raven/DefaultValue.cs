using System;

namespace Raven
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DefaultValue : Attribute
    {
        public object Value { get; }

        public DefaultValue(object value)
        {
            Value = value;
        }
    }
}