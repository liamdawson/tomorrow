using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Tomorrow.Core.Json.Serialization
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SimpleMethodPointer
    {
        public SimpleTypePointer DeclaringType { get; set; }
        public SimpleTypePointer[] ParameterTypes { get; set; } = { };
        public SimpleTypePointer[] GenericTypes { get; set; } = { };
        public string Name { get; set; }

        private MethodInfo _methodInfo;

        [JsonIgnore]
        public MethodInfo MethodInfo => _methodInfo ?? (_methodInfo = FindMethodInfo());

        private MethodInfo FindMethodInfo()
        {
            var parameterTypes = ParameterTypes.Select(pt => pt.QualifiedName);

            var targetMethod = DeclaringType.Type.GetRuntimeMethods()
                .Single(mi =>
                    mi.Name == Name
                    && mi.GetParameters().Select(p => p.ParameterType.AssemblyQualifiedName).SequenceEqual(parameterTypes)
                    && mi.GetGenericArguments().Length == GenericTypes.Length
                    && mi.IsGenericMethodDefinition == GenericTypes.Any());

            if (GenericTypes.Any())
            {
                targetMethod = targetMethod.MakeGenericMethod(GenericTypes.Select(stp => stp.Type).ToArray());
            }

            return targetMethod;
        }

        public SimpleMethodPointer(MethodInfo methodInfo)
        {
            if (methodInfo.IsGenericMethod)
            {
                GenericTypes = methodInfo.GetGenericArguments().Select(t => (SimpleTypePointer)t).ToArray();
                methodInfo = methodInfo.GetGenericMethodDefinition();
            }

            DeclaringType = methodInfo.DeclaringType;
            ParameterTypes = methodInfo.GetParameters().Select(p => (SimpleTypePointer)p.ParameterType).ToArray();
            Name = methodInfo.Name;
        }

        public SimpleMethodPointer() { }

        public static implicit operator SimpleMethodPointer(MethodInfo methodInfo)
        {
            return new SimpleMethodPointer(methodInfo);
        }

        public static implicit operator MethodInfo(SimpleMethodPointer methodReference)
        {
            return methodReference.MethodInfo;
        }
    }
}
