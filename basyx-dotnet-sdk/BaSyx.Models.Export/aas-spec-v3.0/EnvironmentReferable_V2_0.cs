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

    public class EnvironmentReferable_V2_0
    {
        [JsonProperty("idShort", Order = -6)]
        [XmlElement("idShort")]
        public string IdShort { get; set; }

        [JsonProperty("category", Order = -5)]
        [XmlElement("category")]
        public string Category { get; set; }

        [JsonProperty("description", Order = -4)]
        [XmlArray("description")]
        [XmlArrayItem("langString")]
        public LangStringSet Description { get; set; }

        [JsonProperty("parent", Order = -3)]
        [XmlElement("parent")]
        public EnvironmentReference_V2_0 Parent { get; set; }

        public bool ShouldSerializeDescription()
        {
            if (Description == null || Description.Count == 0)
                return false;
            else
                return true;
        }
    }
}
