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
using BaSyx.Models.AdminShell;
using BaSyx.Models.Export.Converter;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class Operation_V3_0 : SubmodelElementType_V3_0
    {
        [JsonProperty("inputVariables"), JsonConverter(typeof(JsonOperationVariableConverter_V3_0))]
        [XmlArray(ElementName = "inputVariables"), XmlArrayItem(ElementName = "operationVariable")]
        public List<OperationVariable_V3_0> InputVariables { get; set; }

        [JsonProperty("outputVariables")]
        [XmlArray(ElementName = "outputVariables"), XmlArrayItem(ElementName = "operationVariable"), JsonConverter(typeof(JsonOperationVariableConverter_V3_0))]
        public List<OperationVariable_V3_0> OutputVariables { get; set; }

        [JsonProperty("inoutputVariables")]
        [XmlArray(ElementName = "inoutputVariables"), XmlArrayItem(ElementName = "operationVariable"), JsonConverter(typeof(JsonOperationVariableConverter_V3_0))]
        public List<OperationVariable_V3_0> InOutputVariables { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.Operation;

        public Operation_V3_0() { }
        public Operation_V3_0(SubmodelElementType_V3_0 submodelElementType) : base(submodelElementType) { }
    }
}
