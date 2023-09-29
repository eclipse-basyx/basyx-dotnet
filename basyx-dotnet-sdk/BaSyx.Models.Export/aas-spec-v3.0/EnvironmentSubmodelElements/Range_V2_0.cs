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
