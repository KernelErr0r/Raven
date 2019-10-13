using Raven.Parsers;
using Raven.Tests.Commands;
using Xunit;

namespace Raven.Tests.Parsers
{
    public class NumberParserTest
    {
        private CommandDispatcher dispatcher = new CommandDispatcher();

        private NumberParserTestCommand testCommand = new NumberParserTestCommand();

        public NumberParserTest()
        {
            dispatcher.ArgumentParser.RegisterTypeParser(new NumberParser());
            dispatcher.ArgumentParser.RegisterTypeParser(new StringParser());
        }
    
        [Fact]
        public void TestNumberParser()
        {
            var handler = dispatcher.Dispatch(testCommand, "", "15");
            var parsedArguments = dispatcher.ArgumentParser.ParseArguments(handler.GetParameters(), "15");
            
            Assert.Single(parsedArguments);
            Assert.Equal(15, parsedArguments[0]);
        }
    }
}