using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Raven
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private List<TypeParserPlaceholder> parsers = new List<TypeParserPlaceholder>();

        public void RegisterTypeParser(object parser)
        {
            var type = parser.GetType();

            if (type.GetCustomAttributes(typeof(TypeParser)) is TypeParser[] attributes && type.GetMethod("CanParse") != null)
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

            if (type.GetCustomAttributes(typeof(TypeParser)) is TypeParser[] attributes && type.GetMethod("CanParse") != null)
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

        public List<object> Dispatch(MethodInfo methodInfo, params string[] arguments)
        {
            var result = new List<object>();
            var parameters = methodInfo.GetParameters();
            var typeCandidates = new Dictionary<ParameterInfo, List<TypeParserPlaceholder>>();
            var argumentIndex = 0;
            var optionalParameters = 0;

            arguments = arguments ?? new string[0];

            foreach (var parameter in parameters)
                if (parameter.IsOptional)
                    optionalParameters++;

            if (arguments.Length >= parameters.Length - optionalParameters)
            {
                foreach (var parameter in parameters)
                {
                    foreach (var parser in parsers)
                    {
                        var status = parser.Type.GetMethod("CanParse")
                            .Invoke(parser.Instance, new[] {parameter.ParameterType}) as bool?;

                        if (status ?? false)
                        {
                            if (!typeCandidates.ContainsKey(parameter))
                                typeCandidates.Add(parameter, new List<TypeParserPlaceholder>());

                            typeCandidates[parameter].Add(parser);
                        }
                    }

                    typeCandidates[parameter] = typeCandidates[parameter].OrderBy(x => x.Attribute.Priority).ToList();

                    if (!parameter.ParameterType.IsArray)
                    {
                        foreach (var type in typeCandidates[parameter])
                        {
                            if (parameter.ParameterType == type.Attribute.Type && argumentIndex < arguments.Length)
                            {
                                foreach (var method in type.Type.GetMethods())
                                {
                                    if (method.ReturnType == parameter.ParameterType)
                                    {
                                        result.Add(method.Invoke(type.Instance,
                                            new object[] {arguments[argumentIndex++]}));

                                        break;
                                    }
                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        //TODO
                    }
                }

                for (int i = 0; i < Math.Max(parameters.Length - result.Count, 0); i++)
                    result.Add(Type.Missing);
            }
            else
            {
                throw new ArgumentException();
            }

            return result;
        }

        private struct TypeParserPlaceholder
        {
            public TypeParser Attribute { get; }
            public Type Type { get; }
            public object Instance { get; }

            public TypeParserPlaceholder(TypeParser attribute, Type type, object instance)
            {
                Attribute = attribute;
                Type = type;
                Instance = instance;
            }
        }
    }
}