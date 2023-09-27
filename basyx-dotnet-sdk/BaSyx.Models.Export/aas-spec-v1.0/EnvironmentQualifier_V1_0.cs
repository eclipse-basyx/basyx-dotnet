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
    public class EnvironmentQualifier_V1_0
    {
        [JsonProperty("qualifierType")]
        [XmlElement("qualifierType")]
        public string QualifierType { get; set; }

        [JsonProperty("qualifierValue")]
        [XmlElement("qualifierValue")]
        public string QualifierValue { get; set; }

        [JsonProperty("qualifierValueId")]
        [XmlElement("qualifierValueId")]
        public EnvironmentReference_V1_0 QualifierValueId { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public ModelType ModelType => ModelType.Qualifier;

    }
}