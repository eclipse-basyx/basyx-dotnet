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

using BaSyx.Models.Core.AssetAdministrationShell;
using BaSyx.Models.Core.Common;
using BaSyx.Models.Export.Converter;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    public class EnvironmentSubmodel_V1_0 : EnvironmentIdentifiable_V1_0, IModelType
    {
        [JsonProperty("kind")]
        [XmlElement("kind")]
        public ModelingKind Kind { get; set; }

        [JsonProperty("semanticId")]
        [XmlElement("semanticId")]
        public EnvironmentReference_V1_0 SemanticId { get; set; }        

        [JsonProperty("constraints")]
        [XmlArray("qualifier")]
        [XmlArrayItem("qualifier")]
        public List<EnvironmentQualifier_V1_0> Qualifier { get; set; }

        [JsonProperty("submodelElements"), JsonConverter(typeof(JsonSubmodelElementConverter_V1_0))]
        [XmlArray("submodelElements")]
        [XmlArrayItem("submodelElement")]
        public List<EnvironmentSubmodelElement_V1_0> SubmodelElements { get; set; }

        [JsonProperty("modelType")]
        [XmlIgnore]
        public ModelType ModelType => ModelType.Submodel;

        public bool ShouldSerializeSemanticId()
        {
            if (SemanticId == null)
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

        public bool ShouldSerializeSubmodelElements()
        {
            if (SubmodelElements == null || SubmodelElements.Count == 0)
                return false;
            else
                return true;
        }
    }
}