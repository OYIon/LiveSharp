using System.Xml;

namespace LiveSharp.Interfaces
{
    public interface ILiveSharpConfig
    {
        bool TryGetValue(string key, out string value);
    }
}