using System;
using System.Reflection;
using Raven.Parsers;
using Xunit;

namespace Raven.Tests
{
    public class OptionalParametersTest
    {
        private CommandDispatcher dispatcher = new CommandDispatcher();
        
        private MethodInfo method1;
        private MethodInfo method2;

        public OptionalParametersTest()
        {
            method1 = GetType().GetMethod("Method1", BindingFlags.NonPublic | BindingFlags.Instance);
            method2 = GetType().GetMethod("Method2", BindingFlags.NonPublic | BindingFlags.Instance);

            dispatcher.ArgumentParser.RegisterTypeParser(new NumberParser());
            dispatcher.ArgumentParser.RegisterTypeParser(new StringParser());
        }

        [Fact]
        public void TestOptionalParameters1()
        {
            var parsedArguments = dispatcher.Dispatch(method1, "test1", "test2");

            Assert.Equal(2, parsedArguments.Count);
            Assert.Equal("test1", parsedArguments[0]);
            Assert.Equal("test2", parsedArguments[1]);

            method1.Invoke(this, parsedArguments.ToArray());
        }

        [Fact]
        public void TestOptionalParameters2()
        {
            var parsedArguments = dispatcher.Dispatch(method1, "test1");

            Assert.Equal(2, parsedArguments.Count);
            Assert.Equal("test1", parsedArguments[0]);
            Assert.Equal(Type.Missing, parsedArguments[1]);

            method1.Invoke(this, parsedArguments.ToArray());
        }

        [Fact]
        public void TestOptionalParameters3()
        {
            var parsedArguments = dispatcher.Dispatch(method2, "test1", "3", "test2");

            Assert.Equal(3, parsedArguments.Count);
            Assert.Equal("test1", parsedArguments[0]);
            Assert.Equal(3, parsedArguments[1]);
            Assert.Equal("test2", parsedArguments[2]);

            method2.Invoke(this, parsedArguments.ToArray());
        }

        private void Method1(string arg1, string arg2 = "") { }
        private void Method2(string arg1, int arg2 = 5, string arg3 = "") { }
    }
}