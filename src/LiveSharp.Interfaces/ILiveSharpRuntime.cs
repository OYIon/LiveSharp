using System;

namespace LiveSharp
{
    public interface ILiveSharpRuntime
    {
        ILogger Logger { get; }
        object GetUpdate(object oldContext, string ctor, Type[] argsConstructorParameterTypes, object[] args);
        void ExecuteVoid(object update, object oldContext, object[] argsConstructorArguments);
    }
}