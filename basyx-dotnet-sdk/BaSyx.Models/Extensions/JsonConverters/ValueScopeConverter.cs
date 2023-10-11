using BaSyx.Models.AdminShell;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaSyx.Models.Extensions
{
    public class ValueScopeConverter : JsonConverter<ValueScope>
    {
        public override ValueScope Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ValueScope value, JsonSerializerOptions options)
        {
            if (value is PropertyValue propValue)
            {
                JsonSerializer.Serialize(writer, propValue.Value.Value.ToString());
            }
        }
    }
}
