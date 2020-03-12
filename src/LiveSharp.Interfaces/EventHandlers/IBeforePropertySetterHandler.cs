using System;

namespace LiveSharp
{
    public interface IBeforePropertySetterHandler : ILiveSharpEventHandler
    {
        void BeforePropertySetter(object instance, Type instanceType, string propertyName, object newValue);
    }
}