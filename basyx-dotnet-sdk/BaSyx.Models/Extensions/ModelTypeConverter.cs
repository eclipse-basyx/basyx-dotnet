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
    public class ModelTypeConverter : JsonConverter<ModelType>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<ModelTypeConverter>();       

        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override ModelType ReadJson(JsonReader reader, Type objectType, ModelType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject;

            try
            {
                jObject = JObject.Load(reader);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Unable to load JObject from type ${objectType.Name}");
                return null;
            }

            ModelType modelType = jObject.SelectToken("modelType")?.ToObject<ModelType>(serializer);
            if (modelType == null)
            {
                logger.LogError("ModelType missing: " + jObject.ToString());
                return null;
            }
            
            return modelType;
        }

        public override void WriteJson(JsonWriter writer, ModelType value, JsonSerializer serializer)
        {
            JValue jValue = new JValue(value.Name);
            serializer.Serialize(writer, jValue);
        }
    }
}
