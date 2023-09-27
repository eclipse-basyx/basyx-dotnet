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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{

    public class EnvironmentAssetAdministrationShell_V1_0 : EnvironmentIdentifiable_V1_0, IModelType
    {
        [JsonProperty("asset")]
        [XmlElement("assetRef")]
        public EnvironmentReference_V1_0 AssetReference { get; set; }

        [JsonProperty("submodels")]
        [XmlArray("submodelRefs")]
        [XmlArrayItem("submodelRef")]
        public List<EnvironmentReference_V1_0> SubmodelReferences { get; set; }

        [JsonProperty("conceptDictionaries")]
        [XmlElement("conceptDictionaries")]
        public List<ConceptDictionary_V1_0> ConceptDictionaries { get; set; }

        [JsonProperty("derivedFrom")]
        [XmlElement("derivedFrom")]
        public EnvironmentReference_V1_0 DerivedFrom { get; set; }

        [JsonProperty("views")]
        [XmlArray("views")]
        [XmlArrayItem("view")]
        public List<View_V1_0> Views { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public ModelType ModelType => ModelType.AssetAdministrationShell;
    }

    public class ConceptDictionary_V1_0 : EnvironmentReferable_V1_0
    {
        [JsonProperty("conceptDescriptions")]
        [XmlArray("conceptDescriptions")]
        [XmlArrayItem("conceptDescriptionRefs")]
        public List<EnvironmentReference_V1_0> ConceptDescriptionsRefs {get; set; }
    }

    public class View_V1_0 : EnvironmentReferable_V1_0, IModelType
    {
        [JsonProperty("semanticId")]
        [XmlElement(ElementName = "semanticId")]
        public EnvironmentReference_V1_0 SemanticId { get; set; }

        [JsonProperty("containedElements")]
        [XmlArray("containedElements")]
        [XmlArrayItem(ElementName = "containedElementRef")]
        public List<EnvironmentReference_V1_0> ContainedElements { get; set; }

        [XmlIgnore]
        public ModelType ModelType => ModelType.View;
    }
}
