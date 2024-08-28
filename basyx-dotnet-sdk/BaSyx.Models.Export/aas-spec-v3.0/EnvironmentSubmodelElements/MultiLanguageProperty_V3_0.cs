/*******************************************************************************
* Copyright (c) 2024 Bosch Rexroth AG
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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class MultiLanguageProperty_V3_0 : SubmodelElementType_V3_0
    {
        [JsonProperty("value")]
        [XmlArray("value")]
        [XmlArrayItem("langStringTextType")]
        public List<EnvironmentLangString_V3_0> Value { get; set; }

        [JsonProperty("valueId")]
        [XmlElement("valueId")]
        public EnvironmentReference_V3_0 ValueId { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.MultiLanguageProperty;

        public MultiLanguageProperty_V3_0() { }
        public MultiLanguageProperty_V3_0(SubmodelElementType_V3_0 submodelElementType) : base(submodelElementType) { }
    }
}
