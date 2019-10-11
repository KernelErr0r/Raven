using System;

namespace Raven
{
    public struct TypeParserPlaceholder
    {
        public TypeParserAttribute Attribute { get; }
        public Type Type { get; }
        public object Instance { get; }

        public TypeParserPlaceholder(TypeParserAttribute attribute, Type type, object instance)
        {
            Attribute = attribute;
            Type = type;
            Instance = instance;
        }
    }
}