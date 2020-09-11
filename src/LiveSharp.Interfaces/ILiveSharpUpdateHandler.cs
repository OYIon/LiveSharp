using System;
using System.Collections.Generic;
using LiveSharp.Interfaces;

namespace LiveSharp
{
    public interface ILiveSharpUpdateHandler
    {
        void Attach(ILiveSharpRuntime runtime);
    }
}