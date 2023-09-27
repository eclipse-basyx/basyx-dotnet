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
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaSyx.Models.Export
{
    [XmlType(TypeName = "submodelElement")]
    public class EnvironmentSubmodelElement_V1_0
    {
        [XmlElement(ElementName = "property", Type = typeof(Property_V1_0))]
        [XmlElement(ElementName = "file", Type = typeof(File_V1_0))]
        [XmlElement(ElementName = "blob", Type = typeof(Blob_V1_0))]
        [XmlElement(ElementName = "event", Type = typeof(Event_V1_0))]
        [XmlElement(ElementName = "referenceElement", Type = typeof(ReferenceElement_V1_0))]
        [XmlElement(ElementName = "relationshipElement", Type = typeof(RelationshipElement_V1_0))]
        [XmlElement(ElementName = "submodelElementCollection", Type = typeof(SubmodelElementCollection_V1_0))]
        [XmlElement(ElementName = "operation", Type = typeof(Operation_V1_0))]
        public SubmodelElementType_V1_0 submodelElement;
    }

    public class SubmodelElementType_V1_0 : EnvironmentReferable_V1_0, IModelType
    {
        [JsonProperty("semanticId")]
        [XmlElement("semanticId")]
        public EnvironmentReference_V1_0 SemanticId { get; set; }

        [JsonProperty("kind")]
        [JsonConverter(typeof(StringEnumConverter))]
        [XmlElement("kind")]
        public ModelingKind Kind { get; set; }

        [JsonProperty("constraints")]
        [XmlArray("qualifier")]
        [XmlArrayItem("qualifier")]
        public List<EnvironmentQualifier_V1_0> Qualifier { get; set; } = new List<EnvironmentQualifier_V1_0>();

        [XmlIgnore]
        public virtual ModelType ModelType { get; }

        public SubmodelElementType_V1_0() { }
        public SubmodelElementType_V1_0(SubmodelElementType_V1_0 submodelElementType)
        {
            this.Category = submodelElementType.Category;
            this.Description = submodelElementType.Description;
            this.IdShort = submodelElementType.IdShort;
            this.Kind = submodelElementType.Kind;
            this.Parent = submodelElementType.Parent;
            this.Qualifier = submodelElementType.Qualifier;
            this.SemanticId = submodelElementType.SemanticId;
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