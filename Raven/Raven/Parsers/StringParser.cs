using System;

namespace Raven.Parsers
{
    [TypeParser(typeof(string), argumentsMax: Int32.MaxValue, priority: 0)]
    public class StringParser
    {
        public bool CanParse(Type type)
            => true;
        
        public string ParseString(string input) 
            => input;
    }
}