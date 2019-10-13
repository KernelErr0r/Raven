using Raven.Parsers;
using Raven.Tests.Commands;
using Xunit;

namespace Raven.Tests
{
    public class SubcommandsTest
    {
        private CommandManager commandManager = new CommandManager();

        private SubcommandsTestCommand testCommand = new SubcommandsTestCommand();

        public SubcommandsTest()
        {
            commandManager.Dispatcher.ArgumentParser.RegisterTypeParser(new NumberParser());
            commandManager.Dispatcher.ArgumentParser.RegisterTypeParser(new StringParser());
            commandManager.RegisterCommand(testCommand);
        }
        
        [Fact]
        public void TestSubcommands()
        {
            commandManager.Invoke("subcommandstest", "sub1", "test1", "test2");

            Assert.Equal("test1", testCommand.Arg1);
            Assert.Equal("test2", testCommand.Arg3);
        }
    }
}