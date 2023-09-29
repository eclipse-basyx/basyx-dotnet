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
using BaSyx.Models.Export.Converter;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class AnnotatedRelationshipElement_V2_0 : RelationshipElement_V2_0
    {
        [JsonProperty("annotation"), JsonConverter(typeof(JsonSubmodelElementConverter_V2_0))]
        [XmlArray("annotations")]
        [XmlArrayItem("dataElement")]
        public List<EnvironmentSubmodelElement_V2_0> Annotations { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.AnnotatedRelationshipElement;

        public AnnotatedRelationshipElement_V2_0() { }
        public AnnotatedRelationshipElement_V2_0(SubmodelElementType_V2_0 submodelElementType) : base(submodelElementType) { }
    }
}
