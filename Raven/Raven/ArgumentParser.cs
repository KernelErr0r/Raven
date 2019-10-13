using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Raven
{
    public class ArgumentParser : IArgumentParser
    {
        public IReadOnlyList<TypeParserPlaceholder> Parsers => parsers;
        
        private List<TypeParserPlaceholder> parsers = new List<TypeParserPlaceholder>();
        
        private Dictionary<ParameterInfo, List<TypeParserPlaceholder>> candidatesCache = new Dictionary<ParameterInfo, List<TypeParserPlaceholder>>();


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
                throw new InvalidParserException();
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
                throw new InvalidParserException();
            }
        }

        public List<object> ParseArguments(ParameterInfo[] parameters, params string[] arguments)
        {
            var result = new List<object>();
            var candidates = new Dictionary<ParameterInfo, List<TypeParserPlaceholder>>();
            var argumentIndex = 0;
            var optionalParameters = CountOfOptionalParameters(parameters);

            arguments ??= new string[0];

            if (arguments.Length >= parameters.Length - optionalParameters)
            {
                foreach (var parameter in parameters)
                {
                    candidates.Add(parameter, GetCandidates(parameter));
                    candidates[parameter] = candidates[parameter].OrderBy(x => x.Attribute.Priority).ToList();
                    
                    if (argumentIndex < arguments.Length && candidates[parameter].Any(n => n.Attribute.Type == parameter.ParameterType))
                    {
                        if (!parameter.ParameterType.IsArray)
                        {
                            result.Add(ParseType(candidates[parameter], parameter, arguments, ref argumentIndex));
                        }
                        else
                        {
                            //TODO
                            //result.Add(ParseTypeArray());
                        }
                    }
                    else if (parameter.GetCustomAttribute<DefaultValueAttribute>() is { } attribute)
                    {
                        result.Add(attribute.Value);
                    }
                }
            }
            else
            {
                throw new ArgumentException();
            }

            return result;
        }
        
        public List<TypeParserPlaceholder> GetCandidates(ParameterInfo parameter)
        {
            if (candidatesCache.ContainsKey(parameter))
            {
                return candidatesCache[parameter];   
            }
            else
            {
                var candidates = new List<TypeParserPlaceholder>();
            
                foreach (var parser in Parsers)
                {
                    var status = parser.Type.GetMethod("CanParse")
                        .Invoke(parser.Instance, new[] {parameter.ParameterType}) as bool?;

                    if (status ?? false)
                    {
                        candidates.Add(parser);
                    }
                }

                candidatesCache.Add(parameter, candidates);
            
                return candidates;
            }
        }

        public object ParseType(List<TypeParserPlaceholder> placeholders, ParameterInfo parameterInfo, string[] arguments, ref int argumentIndex)
        {
            foreach (var type in placeholders)
            {
                if (parameterInfo.ParameterType == type.Attribute.Type && argumentIndex < arguments.Length)
                {
                    object result = null;
                    
                    foreach (var method in type.Type.GetMethods())
                    {
                        if (method.ReturnType == parameterInfo.ParameterType)
                        {
                            result = method.Invoke(type.Instance,
                                new object[] { arguments[argumentIndex++] });

                            break;
                        }
                    }

                    return result;
                }
            }

            return null;
        }
        
        public int CountOfOptionalParameters(ParameterInfo[] parameters)
        {
            var optionalParameters = 0;
            
            foreach (var parameter in parameters)
                if (parameter.GetCustomAttribute<DefaultValueAttribute>() is { })
                    optionalParameters++;
            
            return optionalParameters;
        }
    }
}