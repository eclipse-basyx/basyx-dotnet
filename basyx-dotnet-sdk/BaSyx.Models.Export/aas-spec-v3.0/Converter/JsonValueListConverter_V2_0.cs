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
using System.Collections.Generic;

namespace BaSyx.Models.Export.Converter
{
    public class JsonValueListConverter_V2_0 : JsonConverter<List<ValueReferencePair>>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<JsonValueListConverter_V2_0>();

        public override List<ValueReferencePair> ReadJson(JsonReader reader, Type objectType, List<ValueReferencePair> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                JObject jObject = JObject.Load(reader);
                var valuePairs = jObject.SelectToken("valueReferencePairTypes")?.ToObject<List<ValueReferencePair>>(serializer);
                return valuePairs;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while reading JSON");
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, List<ValueReferencePair> value, JsonSerializer serializer)
        {
            try
            {
                JObject jObject = new JObject();
                JArray jArray = JArray.FromObject(value, serializer);
                jObject.Add(new JProperty("valueReferencePairTypes", jArray));
                jObject.WriteTo(writer);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while writing JSON");
            }
        }
    }
}
