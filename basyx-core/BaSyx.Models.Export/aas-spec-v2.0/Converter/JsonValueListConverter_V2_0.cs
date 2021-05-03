/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Export.EnvironmentDataSpecifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;

namespace BaSyx.Models.Export.Converter
{
    public class JsonValueListConverter_V2_0 : JsonConverter<List<ValueReferencePair>>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
                logger.Error(e);
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
                logger.Error(e);
            }
        }
    }
}
