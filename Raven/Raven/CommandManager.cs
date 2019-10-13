using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Raven
{
    public class CommandManager : ICommandManager
    {
        public ICommandDispatcher Dispatcher { get; } = new CommandDispatcher();
        
        private List<CommandPlaceholder> commands = new List<CommandPlaceholder>();

        public CommandManager() { }
        
        public CommandManager(ICommandDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        public void RegisterCommand(object command)
        {
            var type = command.GetType();

            if (type.GetCustomAttribute(typeof(CommandAttribute)) is CommandAttribute attribute)
            {
                commands.Add(new CommandPlaceholder(attribute, command));
            }
            else
            {
                throw new InvalidCommandException();
            }
        }

        public void UnregisterCommand(object command)
        {
            var type = command.GetType();

            if (type.GetCustomAttribute(typeof(CommandAttribute)) is CommandAttribute attribute)
            {
                for (int i = 0; i < commands.Count; i++)
                {
                    if (commands[i].Attribute.Equals(attribute))
                    {
                        commands.RemoveAt(i);
                        
                        return;
                    }
                }
            }
            else
            {
                throw new InvalidCommandException();
            }
        }

        public void Invoke(string commandName, params string[] arguments)
        {
            foreach (var placeholder in commands)
            {
                if (String.Equals(placeholder.Attribute.Name, commandName, StringComparison.CurrentCultureIgnoreCase))
                {
                    var type = placeholder.Instance.GetType();
                    MethodInfo handler;

                    if (arguments.Length > 0 && GetSubcommand(type, arguments[0]) != null)
                    {
                        var subcommandName = arguments[0];

                        arguments = arguments.Skip(1).ToArray();
                        handler = Dispatcher.Dispatch(placeholder.Instance, subcommandName, arguments);
                    }
                    else
                    {
                        handler = Dispatcher.Dispatch(placeholder.Instance, "", arguments);
                    }

                    var parsedArgs = Dispatcher.ArgumentParser.ParseArguments(handler.GetParameters(), arguments);
                        
                    handler.Invoke(placeholder.Instance, parsedArgs.ToArray());

                    return;
                }
            }
            
            throw new CommandNotFoundException(commandName);
        }

        private MethodInfo GetSubcommand(Type type, string name)
        {
            foreach (var method in type.GetMethods())
            {
                if (method.GetCustomAttribute<SubcommandAttribute>() is { } attribute)
                {
                    if (String.Equals(attribute.Name, name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return method;
                    }
                }
            }
        
            return null;
        }

        private struct CommandPlaceholder
        {
            public CommandAttribute Attribute { get; }
            public object Instance { get; }

            public CommandPlaceholder(CommandAttribute attribute, object instance)
            {
                Attribute = attribute;
                Instance = instance;
            }
        }
    }
}