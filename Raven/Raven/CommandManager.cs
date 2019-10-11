using System;
using System.Collections.Generic;
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
                throw new ArgumentException("Invalid command");
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
                throw new ArgumentException("Invalid command");
            }
        }

        public void Invoke(string commandName, params string[] arguments)
        {
            foreach (var placeholder in commands)
            {
                if (String.Equals(placeholder.Attribute.Name, commandName, StringComparison.CurrentCultureIgnoreCase))
                {
                    var type = placeholder.Instance.GetType();
                    var invokeMethod = type.GetMethod("Invoke");
                    var parsedArgs = Dispatcher.Dispatch(invokeMethod, arguments);
                        
                    invokeMethod.Invoke(placeholder.Instance, parsedArgs.ToArray());

                    return;
                }
            }
            
            throw new ArgumentException($"Command {commandName} doesn't exist");
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