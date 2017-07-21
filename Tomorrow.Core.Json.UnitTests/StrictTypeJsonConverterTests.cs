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
        public static IEnumerable<object[]> Types => new List<object[]>
        {
            new object[] { typeof(object) },
            new object[] { typeof(IServiceProvider) },
            new object[] { typeof(StrictTypeJsonConverter) }
        };

        public static JsonSerializerSettings SerializerSettings =>
        new JsonSerializerSettings
        {
            Converters = {
                new StrictTypeJsonConverter()
            },
            TypeNameHandling = TypeNameHandling.Auto
        };

        [Theory]
        [MemberData(nameof(Types))]
        public void Types_SurviveRoundTrip(Type type)
        {
            var serialized = JsonConvert.SerializeObject(type, SerializerSettings);
            var deserialized = JsonConvert.DeserializeObject<Type>(serialized, SerializerSettings);

            Assert.Equal(type, deserialized);
        }
    }
}