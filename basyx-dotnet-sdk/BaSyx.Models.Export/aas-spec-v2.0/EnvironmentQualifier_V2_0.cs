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
    public class EnvironmentQualifier_V2_0 : ConstraintType_V2_0
    {
        [JsonProperty("type")]
        [XmlElement("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        [XmlElement("value")]
        public string Value { get; set; }

        [JsonProperty("valueType")]
        [XmlElement("valueType")]
        public string ValueType { get; set; }

        [JsonProperty("valueId")]
        [XmlElement("valueId")]
        public EnvironmentReference_V2_0 ValueId { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.Qualifier;
    }
}
