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
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Runtime.Serialization;
using System.Linq;
using System.Xml.Linq;

namespace BaSyx.Models.Extensions
{
    public class ValueScopeConverter : JsonConverter<ValueScope>
    {
        private static readonly ILogger logger = LoggingExtentions.CreateLogger<ValueScope>();
    
        public override bool CanWrite => true;
        public override bool CanRead => false;

        public override ValueScope ReadJson(JsonReader reader, Type objectType, ValueScope existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, ValueScope value, JsonSerializer serializer)
        {
            if(value is PropertyValue propValue)
            {
                serializer.Serialize(writer, propValue.Value.Value);
            }            
        }       
    }
}
