using System;
using LiveSharp.Interfaces;

namespace LiveSharp
{
    public interface ILiveSharpRuntime
    {
        ILiveSharpConfig Config { get; }
        ILogger Logger { get; }
        object GetUpdate(object oldContext, string ctor, Type[] argsConstructorParameterTypes, object[] args);
        void ExecuteVoid(object update, object oldContext, object[] argsConstructorArguments);
    }
}