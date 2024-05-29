﻿/*******************************************************************************
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
    public class SubmodelElementContainerValueOnlyConverter : JsonConverter<IElementContainer<ISubmodelElement>>
    {
        private readonly JsonSerializerOptions _options;
        public SubmodelElementContainerValueOnlyConverter(JsonSerializerOptions jsonOptions) 
        {
            _options = jsonOptions;
        }

        public override IElementContainer<ISubmodelElement> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IElementContainer<ISubmodelElement> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var smElement in value)
            {
                if (smElement.ModelType == ModelType.Operation)
                    continue;

                writer.WritePropertyName(smElement.IdShort);
                var valueScope = smElement.GetValueScope().Result;
                JsonSerializer.Serialize<ValueScope>(writer, valueScope, new JsonSerializerOptions()
                {
                    Converters =
                    {
                        new ValueScopeConverter<ValueScope>(sme: smElement, options: new ValueScopeConverterOptions()
                        {
                            SerializationOption = SerializationOption.ValueOnly
                        }, jsonOptions: _options)
                    }
                });   
            }

            writer.WriteEndArray();
        }
    }
}