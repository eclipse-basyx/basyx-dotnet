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
using Newtonsoft.Json.Converters;

namespace BaSyx.Utils.Json
{
    public class DefaultJsonSerializerSettings : JsonSerializerSettings
    {
        public DefaultJsonSerializerSettings() : base()
        {
            NullValueHandling = NullValueHandling.Ignore;
            Formatting = Formatting.Indented;
            DefaultValueHandling = DefaultValueHandling.Include;
            MissingMemberHandling = MissingMemberHandling.Ignore;
            Converters.Add(new StringEnumConverter());
        }
    }
}
