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
using BaSyx.Models.Export.EnvironmentDataSpecifications;
using BaSyx.Models.Export.Converter;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentConceptDescription_V3_0 : EnvironmentIdentifiable_V3_0, IModelType
    {
        [JsonProperty("embeddedDataSpecifications")]
        [XmlArray("embeddedDataSpecifications")]
        [XmlArrayItem("embeddedDataSpecification")]
        public List<EmbeddedDataSpecification_V3_0> EmbeddedDataSpecifications { get; set; } = new List<EmbeddedDataSpecification_V3_0>();

        [JsonProperty("isCaseOf")]
        [XmlElement("isCaseOf")]
        public List<EnvironmentReference_V3_0> IsCaseOf { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public ModelType ModelType => ModelType.ConceptDescription;

        public bool ShouldSerializeEmbeddedDataSpecifications()
        {
            if (EmbeddedDataSpecifications == null || EmbeddedDataSpecifications.Count == 0)
                return false;
            else
                return true;
        }

        public bool ShouldSerializeIsCaseOf()
        {
            if (IsCaseOf == null || IsCaseOf.Count == 0)
                return false;
            else
                return true;
        }
    }

    public class EmbeddedDataSpecification_V3_0
    {
        [JsonProperty("dataSpecificationContent"), JsonConverter(typeof(JsonDataSpecificationContentConverter_V3_0))]
        [XmlElement("dataSpecificationContent")]
        public DataSpecificationContent_V3_0 DataSpecificationContent { get; set; }

        [JsonProperty("dataSpecification")]
        [XmlElement("dataSpecification")]
        public EnvironmentReference_V3_0 DataSpecification { get; set; }
    }

    public class DataSpecificationContent_V3_0
    {
        [XmlElement("dataSpecificationIec61360")]
        public EnvironmentDataSpecificationIEC61360_V3_0 DataSpecificationIEC61360 { get; set; }
    }
}
