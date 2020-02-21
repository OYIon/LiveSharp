using System.Xml;

namespace LiveSharp.Interfaces
{
    public interface ILiveSharpConfig
    {
        bool TryGetValue(string xpath, out string value);
    }
}