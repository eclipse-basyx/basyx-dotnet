/*******************************************************************************
* Copyright (c) 2023 Bosch Rexroth AG
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
    public class EnvironmentSubmodel_V3_0 : EnvironmentIdentifiable_V3_0, IModelType
    {
        [JsonProperty("kind")]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("kind")]
        public ModelingKind Kind { get; set; }

        [JsonProperty("semanticId")]
        [XmlElement("semanticId")]
        public EnvironmentReference_V3_0 SemanticId { get; set; }

        [JsonProperty("qualifiers"), JsonConverter(typeof(JsonQualifierConverter_V3_0))]
        [XmlArray("qualifier")]
        [XmlArrayItem("qualifier")]
        public List<EnvironmentQualifier_V3_0> Qualifier { get; set; }

        [JsonProperty("submodelElements"), JsonConverter(typeof(JsonSubmodelElementConverter_V3_0))]
        [XmlArray("submodelElements")]
		[XmlArrayItem(ElementName = "property", Type = typeof(Property_V3_0))]
		[XmlArrayItem(ElementName = "multiLanguageProperty", Type = typeof(MultiLanguageProperty_V3_0))]
		[XmlArrayItem(ElementName = "file", Type = typeof(File_V3_0))]
		[XmlArrayItem(ElementName = "blob", Type = typeof(Blob_V3_0))]
		[XmlArrayItem(ElementName = "capability", Type = typeof(Capability_V3_0))]
		[XmlArrayItem(ElementName = "event", Type = typeof(Event_V3_0))]
		[XmlArrayItem(ElementName = "basicEvent", Type = typeof(BasicEvent_V3_0))]
		[XmlArrayItem(ElementName = "range", Type = typeof(Range_V3_0))]
		[XmlArrayItem(ElementName = "entity", Type = typeof(Entity_V3_0))]
		[XmlArrayItem(ElementName = "referenceElement", Type = typeof(ReferenceElement_V3_0))]
		[XmlArrayItem(ElementName = "annotatedRelationshipElement", Type = typeof(AnnotatedRelationshipElement_V3_0))]
		[XmlArrayItem(ElementName = "relationshipElement", Type = typeof(RelationshipElement_V3_0))]
		[XmlArrayItem(ElementName = "submodelElementCollection", Type = typeof(SubmodelElementCollection_V3_0))]
		[XmlArrayItem(ElementName = "operation", Type = typeof(Operation_V3_0))]
		public List<SubmodelElementType_V3_0> SubmodelElements { get; set; }

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
