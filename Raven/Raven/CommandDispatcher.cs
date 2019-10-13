using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Raven
{
    public class CommandDispatcher : ICommandDispatcher
    {
        public IArgumentParser ArgumentParser { get; } = new ArgumentParser();

        private BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        
        public CommandDispatcher() { }

        public CommandDispatcher(IArgumentParser argumentParser)
        {
            ArgumentParser = argumentParser;
        }

        public MethodInfo Dispatch(object command, string subcommand, params string[] arguments)
        {
            var commandType = command.GetType();

            if (commandType.GetCustomAttribute<CommandAttribute>() is { })
            {
                var handlers = GetHandlers(commandType, subcommand);

                foreach (var handler in handlers)
                {
                    if (IsCorrectHandler(handler, arguments))
                    {
                        return handler;
                    }
                }

                throw new HandlerNotFoundException();
            }
            else
            {
                throw new InvalidCommandException();
            }
        }

        private List<MethodInfo> GetHandlers(Type type, string subcommand)
        {
            var handlers = new List<MethodInfo>();

            foreach (var method in type.GetMethods(flags))
            {
                if (string.IsNullOrWhiteSpace(subcommand) && method.GetCustomAttribute<DefaultAttribute>() is { })
                {
                    handlers.Add(method);
                }
                else if (!string.IsNullOrWhiteSpace(subcommand) && method.GetCustomAttribute<SubcommandAttribute>() is { } subcommandAttribute)
                {
                    if (string.Equals(subcommandAttribute.Name, subcommand, StringComparison.InvariantCultureIgnoreCase))
                    {
                        handlers.Add(method);
                    }
                }
            }

            return handlers;
        }

        private bool IsCorrectHandler(MethodInfo handler, params string[] arguments)
        {
            var parameters = handler.GetParameters();
            var candidates = new Dictionary<ParameterInfo, List<TypeParserPlaceholder>>();
            var optionalParameters = ArgumentParser.CountOfOptionalParameters(parameters);
            var argumentIndex = 0;
                
            arguments ??= new string[0];

            if (arguments.Length >= parameters.Length - optionalParameters)
            {
                foreach (var parameter in parameters)
                {
                    candidates.Add(parameter, ArgumentParser.GetCandidates(parameter));
                    candidates[parameter] = candidates[parameter].OrderBy(x => x.Attribute.Priority).ToList();

                    if (argumentIndex < arguments.Length && candidates[parameter].Any(n => n.Attribute.Type == parameter.ParameterType))
                    {
                        if (!parameter.ParameterType.IsArray)
                        {
                            var result = ArgumentParser.ParseType(candidates[parameter], parameter, arguments, ref argumentIndex);

                            if (result == null && parameter.GetCustomAttribute<DefaultValueAttribute>() is null)
                                return false;
                        }
                        else
                        {
                            //TODO
                        }
                    }
                    else if (parameter.GetCustomAttribute<DefaultValueAttribute>() is null)
                    {
                        return false;
                    }
                }
            }
            else
            {
                throw new ArgumentException();
            }

            return true;
        }
    }
}