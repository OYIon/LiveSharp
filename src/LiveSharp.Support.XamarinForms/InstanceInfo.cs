using System;

namespace LiveSharp
{
    class InstanceInfo
    {
        public int InstanceId { get; }
        public object[] ConstructorArguments { get; }
        public Type[] ConstructorParameterTypes { get; }

        public InstanceInfo(int instanceId, object[] constructorArguments, Type[] constructorParameterTypes)
        {
            InstanceId = instanceId;
            ConstructorArguments = constructorArguments;
            ConstructorParameterTypes = constructorParameterTypes;
        }
    }
}