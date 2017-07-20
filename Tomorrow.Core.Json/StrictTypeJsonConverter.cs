using System;
using System.Reflection;
using Newtonsoft.Json;
using Tomorrow.Core.Json.Serialization;

namespace Tomorrow.Core.Json
{
    public class StrictTypeJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var val = value as Type;
            
            if (val == null)
            {
                writer.WriteNull();
            }
            else
            {
                serializer.Serialize(writer, new StrictTypeReference(val), typeof(StrictTypeReference));
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var pointer = serializer.Deserialize<StrictTypeReference>(reader);

            return pointer?.Type ?? existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            // isn't necessarily only just "Type"
            return objectType == typeof(Type)
                   || objectType.GetTypeInfo().IsSubclassOf(typeof(Type));
        }
    }
}
