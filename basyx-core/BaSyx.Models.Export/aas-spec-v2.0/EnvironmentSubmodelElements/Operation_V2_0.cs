/*******************************************************************************
* Copyright (c) 2020 Robert Bosch GmbH
* Author: Constantin Ziesche (constantin.ziesche@bosch.com)
*
* This program and the accompanying materials are made available under the
* terms of the Eclipse Public License 2.0 which is available at
* http://www.eclipse.org/legal/epl-2.0
*
* SPDX-License-Identifier: EPL-2.0
*******************************************************************************/
using BaSyx.Models.Core.Common;
using BaSyx.Models.Export.Converter;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class Operation_V2_0 : SubmodelElementType_V2_0
    {
        [JsonProperty("inputVariables"), JsonConverter(typeof(JsonOperationVariableConverter_V2_0))]
        [XmlElement(ElementName = "inputVariable")]
        public List<OperationVariable_V2_0> InputVariables { get; set; }

        [JsonProperty("outputVariables")]
        [XmlElement(ElementName = "outputVariable"), JsonConverter(typeof(JsonOperationVariableConverter_V2_0))]
        public List<OperationVariable_V2_0> OutputVariables { get; set; }

        [JsonProperty("inoutputVariables")]
        [XmlElement(ElementName = "inoutputVariable"), JsonConverter(typeof(JsonOperationVariableConverter_V2_0))]
        public List<OperationVariable_V2_0> InOutputVariables { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.Operation;

        public Operation_V2_0() { }
        public Operation_V2_0(SubmodelElementType_V2_0 submodelElementType) : base(submodelElementType) { }
    }
}
