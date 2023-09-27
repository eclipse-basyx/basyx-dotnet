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
    public class SubmodelElementCollection_V1_0 : SubmodelElementType_V1_0, IModelType
    {
        [JsonProperty("allowDuplicates")]
        [XmlElement("allowDuplicates")]
        public bool AllowDuplicates { get; set; }

        [JsonProperty("ordered")]
        [XmlElement("ordered")]
        public bool Ordered { get; set; }

        [JsonProperty("value"), JsonConverter(typeof(JsonSubmodelElementConverter_V1_0))]
        [XmlArray("value")]
        [XmlArrayItem("submodelElement")]
        public List<EnvironmentSubmodelElement_V1_0> Value { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public override ModelType ModelType => ModelType.SubmodelElementCollection;

        public SubmodelElementCollection_V1_0() { }
        public SubmodelElementCollection_V1_0(SubmodelElementType_V1_0 submodelElementType) : base(submodelElementType) { }

        public bool ShouldSerializeValue()
        {
            if (Value == null || Value.Count == 0)
                return false;
            else
                return true;
        }
    }
}