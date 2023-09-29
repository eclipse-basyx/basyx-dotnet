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
using BaSyx.Models.Export.EnvironmentDataSpecifications;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace BaSyx.Models.Export.Converter
{
    public class JsonDataSpecificationContentConverter_V2_0 : JsonConverter<DataSpecificationContent_V2_0>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<JsonDataSpecificationContentConverter_V2_0>();

        public override DataSpecificationContent_V2_0 ReadJson(JsonReader reader, Type objectType, DataSpecificationContent_V2_0 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                JObject jObject = JObject.Load(reader);
                var specContent = jObject.ToObject<EnvironmentDataSpecificationIEC61360_V2_0>(serializer);
                DataSpecificationContent_V2_0 content = new DataSpecificationContent_V2_0() { DataSpecificationIEC61360 = specContent };
                return content;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while reading JSON");
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, DataSpecificationContent_V2_0 value, JsonSerializer serializer)
        {
            try
            {
                JObject jObject = JObject.FromObject(value.DataSpecificationIEC61360, serializer);
                jObject.WriteTo(writer);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while writing JSON");
            }
        }
    }
}
