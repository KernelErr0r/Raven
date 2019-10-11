using System.Collections.Generic;

namespace Raven
{
    public interface IArgumentParser
    {
        IReadOnlyList<TypeParserPlaceholder> Parsers { get; }
        
        void RegisterTypeParser(object parser);
        void UnregisterTypeParser(object parser);
    }
}