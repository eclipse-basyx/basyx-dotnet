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
    public class Operation_V2_0 : SubmodelElementType_V2_0
    {
        [JsonProperty("inputVariable"), JsonConverter(typeof(JsonOperationVariableConverter_V2_0))]
        [XmlElement(ElementName = "inputVariable")]
        public List<OperationVariable_V2_0> InputVariables { get; set; }

        [JsonProperty("outputVariable")]
        [XmlElement(ElementName = "outputVariable"), JsonConverter(typeof(JsonOperationVariableConverter_V2_0))]
        public List<OperationVariable_V2_0> OutputVariables { get; set; }

        [JsonProperty("inoutputVariable")]
        [XmlElement(ElementName = "inoutputVariable"), JsonConverter(typeof(JsonOperationVariableConverter_V2_0))]
        public List<OperationVariable_V2_0> InOutputVariables { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.Operation;

        public Operation_V2_0() { }
        public Operation_V2_0(SubmodelElementType_V2_0 submodelElementType) : base(submodelElementType) { }
    }
}
