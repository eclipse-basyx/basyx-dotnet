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

namespace BaSyx.Models.Export.Converter
{
    public class JsonDataSpecificationContentConverter_V2_0 : JsonConverter<DataSpecificationContent_V2_0>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
                logger.Error(e);
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
                logger.Error(e);
            }
        }
    }
}
