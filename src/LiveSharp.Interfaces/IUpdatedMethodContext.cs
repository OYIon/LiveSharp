using System.Collections.Generic;

namespace LiveSharp.Interfaces
{
    public interface IUpdatedMethodContext
    {
        string MethodIdentifier { get; }
        object MethodMetadata { get; }
        IReadOnlyList<object> Instances { get; }

        object Invoke(object instance, params object[] arguments);
    }
}