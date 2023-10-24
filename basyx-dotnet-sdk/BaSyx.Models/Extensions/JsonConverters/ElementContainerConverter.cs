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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BaSyx.Models.Extensions
{
    public class ElementContainerConverter : JsonConverter<IElementContainer<ISubmodelElement>>
    {
        public override IElementContainer<ISubmodelElement> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            while(reader.TokenType != JsonTokenType.StartArray)
                reader.Read();

            ElementContainer<ISubmodelElement> container = new ElementContainer<ISubmodelElement>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    return container;
                
                ISubmodelElement submodelElement = JsonSerializer.Deserialize<ISubmodelElement>(ref reader, options);
                container.Add(submodelElement);
            }
            throw new JsonException("Malformed json");
        }

        public override void Write(Utf8JsonWriter writer, IElementContainer<ISubmodelElement> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var sme in value)
            {
                JsonSerializer.Serialize(writer, sme, options);
            }
            writer.WriteEndArray();
        }
    }
}
