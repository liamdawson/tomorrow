using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Tomorrow.Core.Json.Serialization
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SerializableTypeReference
    {
        private Type _type;
        [JsonIgnore]
        public Type Type => _type ?? (_type = Type.GetType(QualifiedName));

        public string QualifiedName { get; set; }

        public SerializableTypeReference(Type type)
        {
            QualifiedName = type.AssemblyQualifiedName;
        }

        public SerializableTypeReference() { }

        public static implicit operator SerializableTypeReference(Type type)
        {
            return new SerializableTypeReference(type);
        }

        public static implicit operator Type(SerializableTypeReference typeReference)
        {
            return typeReference.Type;
        }
    }
}
