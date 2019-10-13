using System;

namespace Raven
{
    public class CommandNotFoundException : Exception
    {
        public readonly string CommandName;
    
        public CommandNotFoundException(string commandName)
        {
            CommandName = commandName;
        }
    }
}