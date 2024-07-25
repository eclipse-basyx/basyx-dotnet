/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
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
    public class ElementContainerConverterOptions
    {
        public RequestLevel RequestLevel { get; set; } = RequestLevel.Deep;
        public RequestExtent RequestExtent { get; set; } = RequestExtent.WithoutBlobValue;
        public int Level { get; set; } = 0;
    }

    public class ElementContainerConverter : JsonConverter<IElementContainer<ISubmodelElement>>
    {
        private ElementContainerConverterOptions _converterOptions;

        public ElementContainerConverter(ElementContainerConverterOptions options = null)
        {
            _converterOptions = options ?? new ElementContainerConverterOptions();
        }

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
            var jsonOptions = new GlobalJsonSerializerOptions().Build();
            jsonOptions.Converters.Add(new SubmodelElementConverter(new SubmodelElementConverterOptions()
            {
                RequestLevel = _converterOptions.RequestLevel
            }));

            writer.WriteStartArray();

            foreach (var sme in value)
            {
                JsonSerializer.Serialize(writer, sme, jsonOptions);
            }

            writer.WriteEndArray();
        }
    }
}
