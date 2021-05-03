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
using Newtonsoft.Json;
using NLog;
using System;

namespace BaSyx.Utils.JsonHandling
{
    public class RawValueConverter : JsonConverter
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            dynamic rawValue = (value as dynamic)?.Value;
            writer.WriteValue(rawValue);
        }
    }
}
