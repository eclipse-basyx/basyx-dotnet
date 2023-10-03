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
        public SerializationLevel Level { get; set; }
        public SerializationType SerializationType { get; set; }

        public Extent Extent { get; set; }

        public int Limit { get; set; }

        public DefaultJsonSerializerSettings() : base()
        {
            Level = SerializationLevel.Deep;
            Limit = -1;
            Extent = Extent.withoutBlobValue;
            SerializationType = SerializationType.Full;


            NullValueHandling = NullValueHandling.Ignore;
            Formatting = Formatting.Indented;
            DefaultValueHandling = DefaultValueHandling.Include;
            MissingMemberHandling = MissingMemberHandling.Ignore;
            Converters.Add(new StringEnumConverter());
        }
    }

    public enum SerializationLevel
    {
        Deep = 0,
        Core = 1,
    }

    public enum SerializationType
    {
        Full = 0,
        MetaDataOnly = 1,
        ValueOnly = 1,
        PathOnly = 1,
    }

    public enum Extent
    {
        withoutBlobValue = 0,
        withBlobValue = 1,
    }
}
