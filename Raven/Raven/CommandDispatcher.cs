using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Raven
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private IArgumentParser argumentParser = new ArgumentParser();
        
        public CommandDispatcher() { }

        public CommandDispatcher(IArgumentParser argumentParser)
        {
            this.argumentParser = argumentParser;
        }

        public List<object> Dispatch(MethodInfo methodInfo, params string[] arguments)
        {
            var result = new List<object>();
            var parameters = methodInfo.GetParameters();
            var candidates = new Dictionary<ParameterInfo, List<TypeParserPlaceholder>>();
            var argumentIndex = 0;
            var optionalParameters = CountOfOptionalParameters(parameters);

            arguments = arguments ?? new string[0];

            if (arguments.Length >= parameters.Length - optionalParameters)
            {
                foreach (var parameter in parameters)
                {
                    candidates.Add(parameter, GetCandidates(parameter));
                    candidates[parameter] = candidates[parameter].OrderBy(x => x.Attribute.Priority).ToList();

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

                FillOptionalParameters(ref result, parameters);
            }
            else
            {
                throw new ArgumentException();
            }

            return result;
        }

        private int CountOfOptionalParameters(ParameterInfo[] parameters)
        {
            var optionalParameters = 0;
            
            foreach (var parameter in parameters)
                if (parameter.IsOptional)
                    optionalParameters++;
            
            return optionalParameters;
        }

        private List<TypeParserPlaceholder> GetCandidates(ParameterInfo parameter)
        {
            var candidates = new List<TypeParserPlaceholder>();
            
            foreach (var parser in argumentParser.Parsers)
            {
                var status = parser.Type.GetMethod("CanParse")
                    .Invoke(parser.Instance, new[] {parameter.ParameterType}) as bool?;

                if (status ?? false)
                {
                    candidates.Add(parser);
                }
            }
            
            return candidates;
        }

        private object ParseType(List<TypeParserPlaceholder> placeholders, ParameterInfo parameterInfo, string[] arguments, ref int argumentIndex)
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

                    return result ?? Type.Missing;
                }
            }

            return Type.Missing;
        }

        private void FillOptionalParameters(ref List<object> result, ParameterInfo[] parameters)
        {
            for (int i = 0; i < Math.Max(parameters.Length - result.Count, 0); i++)
            {
                result.Add(Type.Missing);   
            }
        }
    }
}