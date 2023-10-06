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
    public class FullDataConverter : SubmodelElementConverter
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<FullDataConverter>();

        public override ISubmodelElement ReadJson(JsonReader reader, Type objectType, ISubmodelElement existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return base.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, ISubmodelElement value, JsonSerializer serializer)
        {
            JObject jObject = CreateSerializationObject(value, serializer);
            if (value.Get != null)
            {
                var valueScope = value.GetValueScope().Result;
                if (valueScope != null)
                {
                    if(valueScope is PropertyValue propValue)
                    {
                        string sValue = propValue.Value.ToObject<string>();
                        if (propValue.Value.Value is bool)
                            sValue = sValue.ToLower();

                        JProperty jValue = new JProperty("value", sValue);
                        jObject.Add(jValue);
                    }
                    else if(valueScope is RangeValue rangeValue)
                    {
                        string sValueMin = rangeValue.Min.ToObject<string>();
                        string sValueMax = rangeValue.Max.ToObject<string>();
                        JProperty jValueMin = new JProperty("min", sValueMin);
                        JProperty jValueMax = new JProperty("max", sValueMax);
                        jObject.Add(jValueMin);
                        jObject.Add(jValueMax);
                    }
                }
            }
            serializer.Serialize(writer, jObject);
        }
    }
}
