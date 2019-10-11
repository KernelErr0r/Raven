using System.Reflection;
using Raven.Parsers;
using Xunit;

namespace Raven.Tests.Parsers
{
    public class StringParserTest
    {
        private CommandDispatcher dispatcher = new CommandDispatcher();
        
        private MethodInfo method1;

        public StringParserTest()
        {
            method1 = GetType().GetMethod("Method1", BindingFlags.NonPublic | BindingFlags.Instance);

            dispatcher.ArgumentParser.RegisterTypeParser(new StringParser());
        }
    
        [Fact]
        public void TestStringParser()
        {
            var parsedArguments = dispatcher.Dispatch(method1, "test1", "test2");
            
            Assert.Equal(2, parsedArguments.Count);
            Assert.Equal("test1", parsedArguments[0]);
            Assert.Equal("test2", parsedArguments[1]);

            method1.Invoke(this, parsedArguments.ToArray());
        }
        
        private void Method1(string arg1, string arg2) {}
    }
}