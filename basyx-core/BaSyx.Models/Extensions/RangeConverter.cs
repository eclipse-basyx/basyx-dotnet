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
using BaSyx.Models.Core.AssetAdministrationShell.Generics;
using BaSyx.Models.Core.AssetAdministrationShell.Implementations;
using BaSyx.Models.Core.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;

namespace BaSyx.Models.Extensions
{
    public class RangeConverter : JsonConverter
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            if (objectType is IRange)
                return true;
            else
                return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                JObject jObject = JObject.Load(reader);
                DataType valueType = jObject.SelectToken("valueType")?.ToObject<DataType>(serializer);

                Range range = (Range)SubmodelElementFactory.CreateSubmodelElement(string.Empty, ModelType.Range, valueType);

                string min = jObject.SelectToken("min")?.ToString();
                ElementValue minValue = new ElementValue(min, valueType);
                jObject.Remove("min");

                string max = jObject.SelectToken("max")?.ToString();
                ElementValue maxValue = new ElementValue(max, valueType);
                jObject.Remove("max");

                range.Min = minValue;
                range.Max = maxValue;

                serializer.Populate(jObject.CreateReader(), range);

                return range;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unable to deserialize ${objectType.Name}");
                return null;
            }
           
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
