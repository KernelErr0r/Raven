using System.Collections.Generic;
using System.Reflection;

namespace Raven
{
    public interface ICommandDispatcher
    {
        List<object> Dispatch(MethodInfo methodInfo, params string[] arguments);
    }
}