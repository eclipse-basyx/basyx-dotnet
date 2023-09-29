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
    [XmlType(TypeName = "submodelElement")]
    public class EnvironmentSubmodelElement_V2_0
    {
        [XmlElement(ElementName = "property", Type = typeof(Property_V2_0))]
        [XmlElement(ElementName = "multiLanguageProperty", Type = typeof(MultiLanguageProperty_V2_0))]
        [XmlElement(ElementName = "file", Type = typeof(File_V2_0))]
        [XmlElement(ElementName = "blob", Type = typeof(Blob_V2_0))]
        [XmlElement(ElementName = "capability", Type = typeof(Capability_V2_0))]
        [XmlElement(ElementName = "event", Type = typeof(Event_V2_0))]
        [XmlElement(ElementName = "basicEvent", Type = typeof(BasicEvent_V2_0))]
        [XmlElement(ElementName = "range", Type = typeof(Range_V2_0))]
        [XmlElement(ElementName = "entity", Type = typeof(Entity_V2_0))]
        [XmlElement(ElementName = "referenceElement", Type = typeof(ReferenceElement_V2_0))]
        [XmlElement(ElementName = "annotatedRelationshipElement", Type = typeof(AnnotatedRelationshipElement_V2_0))]
        [XmlElement(ElementName = "relationshipElement", Type = typeof(RelationshipElement_V2_0))]
        [XmlElement(ElementName = "submodelElementCollection", Type = typeof(SubmodelElementCollection_V2_0))]
        [XmlElement(ElementName = "operation", Type = typeof(Operation_V2_0))]
        public SubmodelElementType_V2_0 submodelElement;
    }

    public class SubmodelElementType_V2_0 : EnvironmentReferable_V2_0, IModelType
    {
        [JsonProperty("kind")]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("kind")]
        public ModelingKind Kind { get; set; }

        [JsonProperty("semanticId")]
        [XmlElement("semanticId")]
        public EnvironmentReference_V2_0 SemanticId { get; set; }

        [JsonProperty("qualifiers"), JsonConverter(typeof(JsonQualifierConverter_V2_0))]
        [XmlElement(ElementName = "qualifier")]
        public List<EnvironmentConstraint_V2_0> Qualifier { get; set; } = new List<EnvironmentConstraint_V2_0>();

        [XmlIgnore]
        public virtual ModelType ModelType { get; }

        public SubmodelElementType_V2_0() { }
        public SubmodelElementType_V2_0(SubmodelElementType_V2_0 submodelElementType)
        {
            this.Category = submodelElementType.Category;
            this.Description = submodelElementType.Description;
            this.IdShort = submodelElementType.IdShort;
            this.Kind = submodelElementType.Kind;
            this.Parent = submodelElementType.Parent;
            this.Qualifier = submodelElementType.Qualifier;
            this.SemanticId = submodelElementType.SemanticId;
        }

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
    }
}
