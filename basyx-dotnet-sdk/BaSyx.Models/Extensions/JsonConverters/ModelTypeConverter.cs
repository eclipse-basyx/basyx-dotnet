using BaSyx.Models.AdminShell;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaSyx.Models.Extensions
{
    public class ModelTypeConverter : JsonConverter<ModelType>
    {
        public override ModelType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ModelType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
