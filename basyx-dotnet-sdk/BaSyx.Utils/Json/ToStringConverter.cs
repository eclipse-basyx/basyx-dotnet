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
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace BaSyx.Utils.Json
{
    public class ToStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;
        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string sValue;
            if(value is double dObject)
            {
                sValue = dObject.ToString(CultureInfo.InvariantCulture);
            }
            else if(value is float fObject)
            {
                sValue = fObject.ToString(CultureInfo.InvariantCulture);
            }
            else if (value is decimal decObject)
            {
                sValue = decObject.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                sValue = value.ToString();
            }
            writer.WriteValue(sValue);
        }
    }
}
