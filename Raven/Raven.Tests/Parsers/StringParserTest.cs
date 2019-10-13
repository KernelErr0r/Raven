using Raven.Parsers;
using Raven.Tests.Commands;
using Xunit;

namespace Raven.Tests.Parsers
{
    public class StringParserTest
    {
        private CommandDispatcher dispatcher = new CommandDispatcher();
        
        private StringParserTestCommand testCommand = new StringParserTestCommand();

        public StringParserTest()
        {
            dispatcher.ArgumentParser.RegisterTypeParser(new StringParser());
        }
    
        [Fact]
        public void TestStringParser()
        {
            var handler = dispatcher.Dispatch(testCommand, "", "test1", "test2");
            var parsedArguments = dispatcher.ArgumentParser.ParseArguments(handler.GetParameters(), "test1", "test2");
            
            Assert.Equal(2, parsedArguments.Count);
            Assert.Equal("test1", parsedArguments[0]);
            Assert.Equal("test2", parsedArguments[1]);
        }
    }
}