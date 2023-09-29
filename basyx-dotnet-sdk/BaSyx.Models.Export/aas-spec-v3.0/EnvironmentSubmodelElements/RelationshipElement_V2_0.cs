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
    public class RelationshipElement_V2_0 : SubmodelElementType_V2_0
    {
        [JsonProperty("first")]
        [XmlElement("first")]
        public EnvironmentReference_V2_0 First { get; set; }

        [JsonProperty("second")]
        [XmlElement("second")]
        public EnvironmentReference_V2_0 Second { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.RelationshipElement;

        public RelationshipElement_V2_0() { }
        public RelationshipElement_V2_0(SubmodelElementType_V2_0 submodelElementType) : base(submodelElementType) { }
    }
}
