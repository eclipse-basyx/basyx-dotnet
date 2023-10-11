using BaSyx.Models.AdminShell;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaSyx.Models.Extensions
{
    public class DataTypeConverter : JsonConverter<DataType>
    {
        public override DataType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            DataType dataType = reader.GetString();
            return dataType;
        }

        public override void Write(Utf8JsonWriter writer, DataType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
