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
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class Range_V2_0 : SubmodelElementType_V2_0
    {
        [JsonProperty("min")]
        [XmlElement("min")]
        public string Min { get; set; }

        [JsonProperty("max")]
        [XmlElement("max")]
        public string Max { get; set; }

        [JsonProperty("valueType")]
        [XmlElement("valueType")]
        public string ValueType { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.Range;

        public Range_V2_0() { }
        public Range_V2_0(SubmodelElementType_V2_0 submodelElementType) : base(submodelElementType) { }
    }
}
