/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the MIT License which is available at
* https://github.com/eclipse-basyx/basyx-dotnet/blob/main/LICENSE
*
* SPDX-License-Identifier: MIT
*******************************************************************************/
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentIdentifiable_V3_0 : EnvironmentReferable_V3_0
    {
        [JsonProperty("id", Order = -2)]
        [XmlElement("id")]
        public string Id { get; set; }

		[JsonProperty("administration", Order = -1)]
        [XmlElement("administration")]
        public EnvironmentAdministrativeInformation_V3_0 Administration { get; set; }
    }
}
