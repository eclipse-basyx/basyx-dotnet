/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{

    public class EnvironmentReference_V3_0
    {
		[JsonProperty("type")]
		[XmlElement("type")]
		public ReferenceType Type { get; set; }

        [JsonProperty("keys")]
        [XmlArray("keys")] 
        [XmlArrayItem("key")]
        public List<EnvironmentKey_V3_0> Keys { get; set; }

        public bool ShouldSerializeKeys()
        {
            if (Keys == null || Keys.Count == 0)
                return false;
            else
                return true;
        }
    }
}
