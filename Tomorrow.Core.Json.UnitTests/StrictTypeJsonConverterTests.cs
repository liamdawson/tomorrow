using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tomorrow.Core.Json.Serialization;
using Xunit;

namespace Tomorrow.Core.Json.UnitTests
{
    public class StrictTypeJsonConverterTests
    {
        public static IEnumerable<object[]> TypeStringPairs => new List<object[]>
        {
            new object[] { typeof(object), new StrictTypeReference(typeof(object)).QualifiedName },
            new object[] { typeof(IServiceProvider), new StrictTypeReference(typeof(IServiceProvider)).QualifiedName },
            new object[] { typeof(StrictTypeJsonConverter), new StrictTypeReference(typeof(StrictTypeJsonConverter)).QualifiedName }
        };

        [Theory]
        [MemberData(nameof(TypeStringPairs))]
        public async Task CanSerializeTypes(Type type, string expectedTypeString)
        {
            var serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters =
                {
                    new StrictTypeJsonConverter()
                }
            };

            Assert.Equal($"{{\"QualifiedName\":\"{expectedTypeString}\"}}", await serializer.Serialize(type));
        }

        [Theory]
        [MemberData(nameof(TypeStringPairs))]
        public async Task CanDeserializeTypes(Type expectedType, string typeString)
        {
            var serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters =
                {
                    new StrictTypeJsonConverter()
                }
            };

            Assert.Equal(expectedType, await serializer.Deserialize<Type>($"{{\"QualifiedName\":\"{typeString}\"}}") as Type);
        }
    }

}