/*******************************************************************************
* Copyright (c) 2020, 2021 Robert Bosch GmbH
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