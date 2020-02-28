using System;
using LiveSharp.Interfaces;

namespace LiveSharp
{
    public interface ILiveSharpRuntime
    {
        ILiveSharpConfig Config { get; }
        ILogger Logger { get; }
        object GetUpdate(object instance, string methodName, Type[] parameterTypes, object[] args);
        void ExecuteVoid(object methodBody, object instance, object[] args);
    }
}