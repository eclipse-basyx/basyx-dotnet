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
    public class EnvironmentAssetAdministrationShell_V2_0 : EnvironmentIdentifiable_V2_0, IModelType
    {
        [JsonProperty("derivedFrom")]
        [XmlElement("derivedFrom")]
        public EnvironmentReference_V2_0 DerivedFrom { get; set; }

        [JsonProperty("asset")]
        [XmlElement("assetRef")]
        public EnvironmentReference_V2_0 AssetReference { get; set; }

        [JsonProperty("submodels")]
        [XmlArray("submodelRefs")]
        [XmlArrayItem("submodelRef")]
        public List<EnvironmentReference_V2_0> SubmodelReferences { get; set; }

        [JsonProperty("views")]
        [XmlArray("views")]
        [XmlArrayItem("view")]
        public List<View_V2_0> Views { get; set; }

        [JsonProperty("conceptDictionaries")]
        [XmlArray("conceptDictionaries")]
        [XmlArrayItem("conceptDictionary")]
        public List<ConceptDictionary_V2_0> ConceptDictionaries { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public ModelType ModelType => ModelType.AssetAdministrationShell;

        public bool ShouldSerializeConceptDictionaries()
        {
            if (ConceptDictionaries == null || ConceptDictionaries.Count == 0)
                return false;
            else
                return true;
        }

        public bool ShouldSerializeViews()
        {
            if (Views == null || Views.Count == 0)
                return false;
            else
                return true;
        }

        public bool ShouldSerializeSubmodelReferences()
        {
            if (SubmodelReferences == null || SubmodelReferences.Count == 0)
                return false;
            else
                return true;
        }
    }

    public class ConceptDictionary_V2_0 : EnvironmentReferable_V2_0, IModelType
    {
        [JsonProperty("conceptDescriptions")]
        [XmlArray("conceptDescriptionRefs")]
        [XmlArrayItem("conceptDescriptionRef")]
        public List<EnvironmentReference_V2_0> ConceptDescriptionsRefs {get; set; }

        [XmlIgnore]
        public ModelType ModelType => ModelType.ConceptDictionary;

        public bool ShouldSerializeConceptDescriptionsRefs()
        {
            if (ConceptDescriptionsRefs == null || ConceptDescriptionsRefs.Count == 0)
                return false;
            else
                return true;
        }
    }

    public class View_V2_0 : EnvironmentReferable_V2_0, IModelType
    {
        [JsonProperty("semanticId")]
        [XmlElement(ElementName = "semanticId")]
        public EnvironmentReference_V2_0 SemanticId { get; set; }

        [JsonProperty("containedElements")]
        [XmlArray("containedElements")]
        [XmlArrayItem(ElementName = "containedElementRef")]
        public List<EnvironmentReference_V2_0> ContainedElements { get; set; }

        [XmlIgnore]
        public ModelType ModelType => ModelType.View;

        public bool ShouldSerializeContainedElements()
        {
            if (ContainedElements == null || ContainedElements.Count == 0)
                return false;
            else
                return true;
        }
    }
}
