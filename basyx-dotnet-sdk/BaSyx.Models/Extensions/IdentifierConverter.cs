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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Microsoft.Extensions.Logging;

namespace BaSyx.Models.Extensions
{
    public class IdentifierConverter : JsonConverter<Identifier>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<IdentifierConverter>();
    
        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override Identifier ReadJson(JsonReader reader, Type objectType, Identifier existingValue, bool hasExistingValue, JsonSerializer serializer)
        {            
            var token = JToken.Load(reader);
            return new Identifier(token.ToObject<string>());
        }

        public override void WriteJson(JsonWriter writer, Identifier value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.Id);
        }       
    }
}
