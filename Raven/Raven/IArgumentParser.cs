using System.Collections.Generic;
using System.Reflection;

namespace Raven
{
    public interface IArgumentParser
    {
        IReadOnlyList<TypeParserPlaceholder> Parsers { get; }
        
        void RegisterTypeParser(object parser);
        void UnregisterTypeParser(object parser);

        List<object> ParseArguments(ParameterInfo[] parameters, params string[] arguments);
        List<TypeParserPlaceholder> GetCandidates(ParameterInfo parameter);
        object ParseType(List<TypeParserPlaceholder> placeholders, ParameterInfo parameterInfo, string[] arguments,
            ref int argumentIndex);

        int CountOfOptionalParameters(ParameterInfo[] parameters);
    }
}