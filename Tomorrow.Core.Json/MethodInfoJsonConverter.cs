using System;
using System.Reflection;
using Newtonsoft.Json;
using Tomorrow.Core.Json.Serialization;

namespace Tomorrow.Core.Json
{
    public class MethodInfoJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var val = value as MethodInfo;

            if (val == null)
            {
                writer.WriteNull();
            }
            else
            {
                serializer.Serialize(writer, val, typeof(SerializableMethodInfo));
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var pointer = serializer.Deserialize<SerializableMethodInfo>(reader);

            return pointer?.MethodInfo ?? existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MethodInfo);
        }
    }
}
