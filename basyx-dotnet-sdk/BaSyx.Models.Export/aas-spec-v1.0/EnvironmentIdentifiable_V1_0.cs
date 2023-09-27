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
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentIdentifiable_V1_0 : EnvironmentReferable_V1_0
    {
        [JsonProperty("identification", Order = -2)]
        [XmlElement("identification")]
        public Identifier Identification { get; set; }

        [JsonProperty("administration", Order = -1)]
        [XmlElement("administration")]
        public AdministrativeInformation Administration { get; set; }
    }
}
