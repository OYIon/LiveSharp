using System;
using LiveSharp.Interfaces;
using LiveSharp.Runtime;

namespace LiveSharp
{
    public interface ILiveSharpRuntime
    {
        ILogger Logger { get; }
        ILiveSharpInspector Inspector { get; set; }
        
        object GetUpdate(object instance, string methodName, Type[] parameterTypes, object[] args);
        void ExecuteVoid(object methodBody, object instance, object[] args);

        void SendBroadcast(string message, byte contentType, int groupId);
        void LiveSharpMessageReceived(string message, byte contentType, int groupId);
    }
}