using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Tomorrow.Core.Json.Serialization
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SerializableMethodInfo
    {
        public Type DeclaringType { get; set; }
        public Type[] ParameterTypes { get; set; } = { };
        public Type[] GenericTypes { get; set; } = { };
        public string Name { get; set; }

        private MethodInfo _methodInfo;

        [JsonIgnore]
        public MethodInfo MethodInfo => _methodInfo ?? (_methodInfo = FindMethodInfo());

        private MethodInfo FindMethodInfo()
        {
            var targetMethod = DeclaringType.GetRuntimeMethods()
                .Single(mi =>
                    mi.Name == Name
                    && mi.GetParameters().Select(p => p.ParameterType).SequenceEqual(ParameterTypes)
                    && mi.GetGenericArguments().Length == GenericTypes.Length
                    && mi.IsGenericMethodDefinition == GenericTypes.Any());

            if (GenericTypes.Any())
            {
                targetMethod = targetMethod.MakeGenericMethod(GenericTypes);
            }

            return targetMethod;
        }

        public SerializableMethodInfo(MethodInfo methodInfo)
        {
            if (methodInfo.IsGenericMethod)
            {
                GenericTypes = methodInfo.GetGenericArguments();
                methodInfo = methodInfo.GetGenericMethodDefinition();
            }

            DeclaringType = methodInfo.DeclaringType;
            ParameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            Name = methodInfo.Name;
        }

        public SerializableMethodInfo() { }

        public static implicit operator SerializableMethodInfo(MethodInfo methodInfo)
        {
            return new SerializableMethodInfo(methodInfo);
        }

        public static implicit operator MethodInfo(SerializableMethodInfo methodReference)
        {
            return methodReference.MethodInfo;
        }
    }
}
