using System;
using System.Collections.Generic;
using LiveSharp.Interfaces;

namespace LiveSharp
{
    public interface ILiveSharpUpdateHandler
    {
        void Initialize(ILiveSharpRuntime runtime);
        void HandleCall(object instance, string methodIdentifier, object[] args, Type[] argTypes);
        void HandleUpdate(IReadOnlyList<IUpdatedMethodContext> updatedMethods);
    }
}