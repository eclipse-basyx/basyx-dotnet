/*******************************************************************************
* Copyright (c) 2022 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using BaSyx.Models.AdminShell;
using System;
using System.Buffers.Text;
using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaSyx.Models.Extensions
{
    public class ValueScopeConverter : JsonConverter<ValueScope>
    {
        public override ValueScope Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType == JsonTokenType.Number)
            {
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out double number, out int bytesConsumed) && span.Length == bytesConsumed)
                    return new PropertyValue(new ElementValue(number));
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                string value = reader.GetString();
                return new PropertyValue(new ElementValue(value, typeof(string)));
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, ValueScope value, JsonSerializerOptions options)
        {
            if (value is PropertyValue propValue)
            {
                JsonSerializer.Serialize(writer, propValue.ToJson());
            }
        }
    }
}
