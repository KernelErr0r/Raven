using System;

namespace Raven
{
    public class CommandNotFound : Exception
    {
        public readonly string CommandName;
    
        public CommandNotFound(string commandName)
        {
            CommandName = commandName;
        }
    }
}