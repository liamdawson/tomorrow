using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Tomorrow.Core.Json.Serialization
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SimpleTypePointer
    {
        private Type _type;
        [JsonIgnore]
        public Type Type => _type ?? (_type = Type.GetType(QualifiedName));

        public string QualifiedName { get; set; }

        public SimpleTypePointer(Type type)
        {
            QualifiedName = type.AssemblyQualifiedName;
        }

        public SimpleTypePointer() { }

        public static implicit operator SimpleTypePointer(Type type)
        {
            return new SimpleTypePointer(type);
        }

        public static implicit operator Type(SimpleTypePointer typeReference)
        {
            return typeReference.Type;
        }
    }
}
