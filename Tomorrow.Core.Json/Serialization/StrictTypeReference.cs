using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Tomorrow.Core.Json.Serialization
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class StrictTypeReference
    {
        private Type _type;
        [JsonIgnore]
        public Type Type => _type ?? (_type = Type.GetType(QualifiedName));

        public string QualifiedName { get; set; }

        public StrictTypeReference(Type type)
        {
            QualifiedName = type.AssemblyQualifiedName;
        }

        public StrictTypeReference() { }
    }
}
