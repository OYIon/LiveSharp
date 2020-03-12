using System;

namespace LiveSharp
{
    public interface IAfterPropertySetterHandler : ILiveSharpEventHandler
    {
        void AfterPropertySetter(object instance, Type instanceType, string propertyName, object newValue);
    }
}