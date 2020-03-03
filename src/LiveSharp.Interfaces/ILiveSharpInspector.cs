using System;

namespace LiveSharp.Runtime
{
    public interface ILiveSharpInspector
    {
        void StartInspector();
        void SetCurrentContext(object context);
        event EventHandler<string> SerializedInstanceUpdate;
    }
}