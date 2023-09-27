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
using BaSyx.Models.Export.EnvironmentDataSpecifications;
using BaSyx.Models.Export.Converter;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentConceptDescription_V2_0 : EnvironmentIdentifiable_V2_0, IModelType
    {
        [JsonIgnore]
        [XmlElement("embeddedDataSpecification")]
        public EmbeddedDataSpecification_V2_0 EmbeddedDataSpecification
        {
            get => EmbeddedDataSpecifications?.FirstOrDefault();    
            set
            {
                if (EmbeddedDataSpecifications == null)
                    EmbeddedDataSpecifications = new List<EmbeddedDataSpecification_V2_0>();
                EmbeddedDataSpecifications.Insert(0, value);
            }
        }

        [JsonProperty("embeddedDataSpecifications")]
        [XmlIgnore]
        public List<EmbeddedDataSpecification_V2_0> EmbeddedDataSpecifications { get; set; } = new List<EmbeddedDataSpecification_V2_0>();

        [JsonProperty("isCaseOf")]
        [XmlElement("isCaseOf")]
        public List<EnvironmentReference_V2_0> IsCaseOf { get; set; }

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

    public class EmbeddedDataSpecification_V2_0
    {
        [JsonProperty("dataSpecificationContent"), JsonConverter(typeof(JsonDataSpecificationContentConverter_V2_0))]
        [XmlElement("dataSpecificationContent")]
        public DataSpecificationContent_V2_0 DataSpecificationContent { get; set; }

        [JsonProperty("dataSpecification")]
        [XmlElement("dataSpecification")]
        public EnvironmentReference_V2_0 DataSpecification { get; set; }
    }

    public class DataSpecificationContent_V2_0
    {
        [XmlElement("dataSpecificationIEC61360")]
        public EnvironmentDataSpecificationIEC61360_V2_0 DataSpecificationIEC61360 { get; set; }
    }
}
