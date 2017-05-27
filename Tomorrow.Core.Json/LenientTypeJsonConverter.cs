using System;
using Newtonsoft.Json;
using Tomorrow.Core.Json.Serialization;

namespace Tomorrow.Core.Json
{
    public class LenientTypeJsonConverter : JsonConverter
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
                serializer.Serialize(writer, val, typeof(SerializableTypeReference));
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var pointer = serializer.Deserialize<SerializableTypeReference>(reader);

            return pointer?.Type ?? existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Type);
        }
    }
}
