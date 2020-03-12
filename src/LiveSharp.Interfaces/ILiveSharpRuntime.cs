using System;
using LiveSharp.Interfaces;
using LiveSharp.Runtime;

namespace LiveSharp
{
    public interface ILiveSharpRuntime
    {
        ILiveSharpConfig Config { get; }
        ILogger Logger { get; }
        ILiveSharpInspector Inspector { get; set; }
        
        object GetUpdate(object instance, string methodName, Type[] parameterTypes, object[] args);
        void ExecuteVoid(object methodBody, object instance, object[] args);

        IDisposable Subscribe<THandlerType>(THandlerType handler) where THandlerType : ILiveSharpEventHandler;
    }
}