using System;
using System.Reflection;

namespace Raven
{
    public interface ICommandDispatcher
    {
        IArgumentParser ArgumentParser { get; }

        MethodInfo Dispatch(object command, string subcommand, params string[] arguments);
    }
}