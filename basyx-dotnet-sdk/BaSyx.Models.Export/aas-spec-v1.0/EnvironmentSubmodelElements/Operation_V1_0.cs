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
    public class Operation_V1_0 : SubmodelElementType_V1_0
    {
        [JsonProperty("in"), JsonConverter(typeof(JsonOperationVariableConverter_V1_0))]
        [XmlElement(ElementName = "in")]
        public List<OperationVariable_V1_0> In { get; set; }

        [JsonProperty("out")]
        [XmlElement(ElementName = "out"), JsonConverter(typeof(JsonOperationVariableConverter_V1_0))]
        public List<OperationVariable_V1_0> Out { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.Operation;

        public Operation_V1_0() { }
        public Operation_V1_0(SubmodelElementType_V1_0 submodelElementType) : base(submodelElementType) { }
    }
}