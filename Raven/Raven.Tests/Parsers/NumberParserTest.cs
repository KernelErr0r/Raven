using System.Reflection;
using Raven.Parsers;
using Xunit;

namespace Raven.Tests.Parsers
{
    public class NumberParserTest
    {
        private CommandDispatcher dispatcher = new CommandDispatcher();
        
        private MethodInfo method1;

        public NumberParserTest()
        {
            method1 = GetType().GetMethod("Method1", BindingFlags.NonPublic | BindingFlags.Instance);

            dispatcher.ArgumentParser.RegisterTypeParser(new NumberParser());
            dispatcher.ArgumentParser.RegisterTypeParser(new StringParser());
        }
    
        [Fact]
        public void TestNumberParser()
        {
            var parsedArguments = dispatcher.Dispatch(GetType().GetMethod("Method1", BindingFlags.NonPublic | BindingFlags.Instance), "15");
            
            Assert.Single(parsedArguments);
            Assert.Equal(15, parsedArguments[0]);
            
            method1.Invoke(this, parsedArguments.ToArray());
        }
        
        private void Method1(int arg1) {}
    }
}