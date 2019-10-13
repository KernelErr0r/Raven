using Raven.Parsers;
using Raven.Tests.Commands;
using Xunit;

namespace Raven.Tests
{
    public class OptionalParametersTest
    {
        private CommandDispatcher dispatcher = new CommandDispatcher();

        private OptionalParametersTestCommand testCommand = new OptionalParametersTestCommand();

        public OptionalParametersTest()
        {
            dispatcher.ArgumentParser.RegisterTypeParser(new NumberParser());
            dispatcher.ArgumentParser.RegisterTypeParser(new StringParser());
        }

        [Fact]
        public void TestOptionalParameters1()
        {
            var arguments = new[] { "test1", "test2" };
            var handler = dispatcher.Dispatch(testCommand, "sub1", arguments);
            var parsedArguments = dispatcher.ArgumentParser.ParseArguments(handler.GetParameters(), arguments);

            Assert.Equal(2, parsedArguments.Count);
            Assert.Equal("test1", parsedArguments[0]);
            Assert.Equal("test2", parsedArguments[1]);
        }

        [Fact]
        public void TestOptionalParameters2()
        {
            var arguments = new[] { "test1" };
            var handler = dispatcher.Dispatch(testCommand, "sub1", arguments);
            var parsedArguments = dispatcher.ArgumentParser.ParseArguments(handler.GetParameters(), arguments);

            Assert.Equal(2, parsedArguments.Count);
            Assert.Equal("test1", parsedArguments[0]);
            Assert.Equal("", parsedArguments[1]);
        }

        [Fact]
        public void TestOptionalParameters3()
        {
            var arguments = new[] { "test1", "3", "test2" };
            var handler = dispatcher.Dispatch(testCommand, "sub2", arguments);
            var parsedArguments = dispatcher.ArgumentParser.ParseArguments(handler.GetParameters(), arguments);

            Assert.Equal(3, parsedArguments.Count);
            Assert.Equal("test1", parsedArguments[0]);
            Assert.Equal(3, parsedArguments[1]);
            Assert.Equal("test2", parsedArguments[2]);
        }
    }
}