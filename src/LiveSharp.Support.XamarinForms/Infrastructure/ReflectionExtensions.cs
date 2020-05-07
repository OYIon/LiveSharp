using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LiveSharp.Support.XamarinForms
{
    public static class ReflectionExtensions
    {
        private static readonly Dictionary<Type, string> Aliases =
            new Dictionary<Type, string>()
            {
                { typeof(byte), "byte" },
                { typeof(sbyte), "sbyte" },
                { typeof(short), "short" },
                { typeof(ushort), "ushort" },
                { typeof(int), "int" },
                { typeof(uint), "uint" },
                { typeof(long), "long" },
                { typeof(ulong), "ulong" },
                { typeof(float), "float" },
                { typeof(double), "double" },
                { typeof(decimal), "decimal" },
                { typeof(object), "object" },
                { typeof(bool), "bool" },
                { typeof(char), "char" },
                { typeof(string), "string" },
                { typeof(void), "void" }
            };
        
        public static ConstructorInfo FindConstructor(this Type owner, Type[] requiredParameterTypes)
        {
            var constructors = owner.GetConstructors();

            foreach (var ctor in constructors) {
                var parameters = ctor.GetParameters();
                var currentParameterTypes = parameters.Select(p => p.ParameterType).ToArray();
                
                if (requiredParameterTypes.TypesMatch(currentParameterTypes))
                    return ctor;
            }

            return null;
        }

        private static bool TypesMatch(this Type[] left, Type[] right)
        {
            if (left.Length != right.Length)
                return false;

            for (int i = 0; i < left.Length; i++)
                if (left[i] != right[i])
                    return false;

            return true;
        }

        public static object GetAndCallMethod(this object instance, string name, TypeInfo[] parameterTypes, object[] values)
        {
            var method = GetMethod(instance, name, true, parameterTypes);
            if (method != null)
                return method.Invoke(instance, values);

            throw new InvalidOperationException("Unable to call method " + name + " on type " + instance.GetType() + ". Method not found");
        }
        
        public static MethodInfo GetMethod(this object instance, string name, bool isInstance, TypeInfo[] parameterTypes = null)
        {
            var reflectableInstance = instance as IReflectableType;
            var typeInfo = reflectableInstance?.GetTypeInfo() ?? instance.GetType().GetTypeInfo();

            return GetMethod(typeInfo, name, isInstance, parameterTypes);
        }

        public static MethodInfo GetMethod(this TypeInfo type, string name, bool isInstance, TypeInfo[] parameterTypes = null)
        {
            return GetAllMethods(type).Where(m => m.Name == name)
                .FirstOrDefault(mi => (isInstance ? !mi.IsStatic : mi.IsStatic) && 
                                      HasMatchingParameterTypes(mi, parameterTypes));
        }

        public static IEnumerable<MethodInfo> GetAllMethods(this TypeInfo type)
        {
            var typeInfo = type.GetTypeInfo();

            while (true) {
                foreach (var method in typeInfo.DeclaredMethods)
                    yield return method;

                if (typeInfo.BaseType != null)
                    typeInfo = typeInfo.BaseType.GetTypeInfo();
                else
                    break;
            }
        }
        
        public static bool Is(this object instance, string typeName)
        {
            var type = instance.GetType();
            
            return type.Is(typeName);
        }
        
        public static bool Is(this Type type, string typeName)
        {
            while (type != null) {
                if (type.FullName == typeName)
                    return true;
                
                if (type.GetInterfaces().Any(i => i.FullName == typeName))
                    return true;
                
                type = type.BaseType;
            }
            
            return false;
        }
        
        public static object GetPropertyValue(this object instance, string propertyName)
        {
            var type = instance.GetType();
            return instance.GetPropertyValue(type, propertyName);
        }
        
        public static object GetPropertyValue(this object instance, Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            
            if (property == null)
                throw new Exception($"Property {propertyName} not found on type {type.FullName}");
            
            return property.GetValue(instance);
        }
        
        private static bool HasMatchingParameterTypes(this MethodInfo methodInfo, TypeInfo[] parameterTypes)
        {
            if (parameterTypes == null)
                return true;

            var parameters = methodInfo.GetParameters();

            if (parameters.Length != parameterTypes.Length)
                return false;

            for (var i = 0; i < parameterTypes.Length; i++)
                if (!parameters[i].ParameterType.IsAssignableFrom(parameterTypes[i]))
                    return false;

            return true;
        }
        
        internal static string GetTypeName(this Type type)
        {
            if (type.IsConstructedGenericType) {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                var genericCountIndex = genericTypeDefinition.Name.IndexOf('`');
                if (genericCountIndex == -1)
                    genericCountIndex = genericTypeDefinition.Name.Length;
                var typeDefinitionName = genericTypeDefinition.Name.Substring(0, genericCountIndex);
                var genericTypeArguments = string.Join(", ", type.GenericTypeArguments.Select(t => GetTypeName(t)));
                return typeDefinitionName + "<" + genericTypeArguments + ">";
            } 
            
            if (type.IsArray) {
                return GetTypeName(type.GetElementType()) + "[]";
            }
            
            if (Aliases.TryGetValue(type, out var alias))
                return alias;
            
            return type.Name;
        }
    }
}