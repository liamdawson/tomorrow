using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Tomorrow.Core.Json.UnitTests
{
    internal static class JsonSerializerExtensions
    {
        internal static async Task<object> Deserialize<T>(this JsonSerializer serializer, string json)
        {
            using (var mstr = new MemoryStream())
            using (var writer = new StreamWriter(mstr))
            using (var reader = new StreamReader(mstr))
            {
                await writer.WriteAsync(json);
                await writer.FlushAsync();

                mstr.Position = 0;

                return serializer.Deserialize(reader, typeof(T));
            }
        }

        internal static async Task<string> Serialize(this JsonSerializer serializer, object val, Type objectType = null)
        {
            using (var mstr = new MemoryStream())
            using (var writer = new StreamWriter(mstr))
            using (var reader = new StreamReader(mstr))
            {
                if (objectType != null)
                {
                    serializer.Serialize(writer, val, objectType);
                }
                else
                {
                    serializer.Serialize(writer, val);
                }
                await writer.FlushAsync();
                mstr.Position = 0;

                return await reader.ReadToEndAsync();
            }
        }
    }
}