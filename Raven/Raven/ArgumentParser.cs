using System;
using System.Reflection;
using System.Collections.Generic;

namespace Raven
{
    public class ArgumentParser : IArgumentParser
    {
        public IReadOnlyList<TypeParserPlaceholder> Parsers => parsers;
        
        private List<TypeParserPlaceholder> parsers = new List<TypeParserPlaceholder>();

        public void RegisterTypeParser(object parser)
        {
            var type = parser.GetType();

            if (type.GetCustomAttributes(typeof(TypeParserAttribute)) is TypeParserAttribute[] attributes && type.GetMethod("CanParse") != null)
            {
                foreach (var attribute in attributes)
                {
                    parsers.Add(new TypeParserPlaceholder(attribute, type, parser));
                }
            }
            else
            {
                throw new ArgumentException("Invalid parser");
            }
        }

        public void UnregisterTypeParser(object parser)
        {
            var type = parser.GetType();

            if (type.GetCustomAttributes(typeof(TypeParserAttribute)) is TypeParserAttribute[] attributes && type.GetMethod("CanParse") != null)
            {
                for (int i = 0; i < parsers.Count; i++)
                {
                    foreach (var attribute in attributes)
                    {
                        if (parsers[i].Attribute.Equals(attribute))
                        {
                            parsers.RemoveAt(i);
                            
                            return;
                        }   
                    }
                }
            }
            else
            {
                throw new ArgumentException("Invalid parser");
            }
        }
    }
}