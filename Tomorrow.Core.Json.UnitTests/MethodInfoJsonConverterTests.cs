using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Xunit;

namespace Tomorrow.Core.Json.UnitTests
{
    public class MethodInfoJsonConverterTests
    {
        public static JsonSerializerSettings SerializerSettings =>
            new JsonSerializerSettings
            {
                Converters = 
                {
                    new StrictTypeJsonConverter(),
                    new MethodInfoJsonConverter()
                },
                TypeNameHandling = TypeNameHandling.Auto
            };

        public static IEnumerable<object[]> MethodInfos => new List<object[]>
        {
            new object[] { ((Func<string, bool>) string.IsNullOrEmpty).GetMethodInfo() },
            new object[] { ((Func<int>)1.GetHashCode).GetMethodInfo() }
        };

        [Theory]
        [MemberData(nameof(MethodInfos))]
        public void MethodInfo_SurvivesRoundtrip(MethodInfo methodInfo)
        {
            var representation = JsonConvert.SerializeObject(methodInfo, SerializerSettings);
            var deserialized = JsonConvert.DeserializeObject<MethodInfo>(representation, SerializerSettings);
            
            Assert.Equal(methodInfo, deserialized);
        }
    }
}