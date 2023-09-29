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
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentSubmodel_V2_0 : EnvironmentIdentifiable_V2_0, IModelType
    {
        [JsonProperty("kind")]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("kind")]
        public ModelingKind Kind { get; set; }

        [JsonProperty("semanticId")]
        [XmlElement("semanticId")]
        public EnvironmentReference_V2_0 SemanticId { get; set; }

        [JsonProperty("qualifiers"), JsonConverter(typeof(JsonQualifierConverter_V2_0))]
        [XmlArray("qualifier")]
        [XmlArrayItem("qualifier")]
        public List<EnvironmentQualifier_V2_0> Qualifier { get; set; }

        [JsonProperty("submodelElements"), JsonConverter(typeof(JsonSubmodelElementConverter_V2_0))]
        [XmlArray("submodelElements")]
        [XmlArrayItem("submodelElement")]
        public List<EnvironmentSubmodelElement_V2_0> SubmodelElements { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public ModelType ModelType => ModelType.Submodel;

        public bool ShouldSerializeSemanticId()
        {
            if (SemanticId == null || SemanticId.Keys?.Count == 0)
                return false;
            else
                return true;
        }

        public bool ShouldSerializeQualifier()
        {
            if (Qualifier == null || Qualifier.Count == 0)
                return false;
            else
                return true;
        }
        /*
        public bool ShouldSerializeSubmodelElements()
        {
            if (SubmodelElements == null || SubmodelElements.Count == 0)
                return false;
            else
                return true;
        }
        */
    }
}
