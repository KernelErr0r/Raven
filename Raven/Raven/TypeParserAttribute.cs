using System;

namespace Raven
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]   
    public sealed class TypeParserAttribute : Attribute
    {
        public Type Type { get; }
        public int ArgumentsMin { get; }
        public int ArgumentsMax { get; }
        public int Priority { get; }

        public TypeParserAttribute(Type type, int argumentsMin = 1, int argumentsMax = 1, int priority = 1)
        {
            if(type == null || (argumentsMin == argumentsMax && argumentsMin > 1) || (argumentsMin <= 0 || argumentsMax <= 0))
                throw new ArgumentException("Invalid arguments");
            
            Type = type;
            ArgumentsMin = argumentsMin;
            ArgumentsMax = argumentsMax;
            Priority = priority;
        }
    }
}