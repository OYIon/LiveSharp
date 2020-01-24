using System;

namespace LiveSharp.Runtime
{
    public interface ILiveSharpInspector
    {
        void StartInspector();
        event EventHandler<string> SerializedInstanceUpdate;
    }
}