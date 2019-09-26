namespace Raven
{
    public interface ICommandManager
    {
        ICommandDispatcher Dispatcher { get; }

        void RegisterCommand(object command);
        void UnregisterCommand(object command);

        void Invoke(string commandName, params string[] arguments);
    }
}