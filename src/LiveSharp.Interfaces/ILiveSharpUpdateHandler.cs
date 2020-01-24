using System;
using System.Collections.Generic;

namespace LiveSharp
{
    public interface ILiveSharpUpdateHandler
    {
        void Initialize(ILiveSharpRuntime runtime);
        void HandleCall(object instance, string methodIdentifier, object[] args, Type[] argTypes);
        void HandleUpdate(Dictionary<string, IReadOnlyList<object>> updatedMethods);
    }
}